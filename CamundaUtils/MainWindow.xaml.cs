using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Camunda.Api.Client;
using CamundaUtils.Core;
using CamundaUtils.Settings;
using CamundaUtils.Stubs;
using Environment = CamundaUtils.Settings.Environment;

namespace CamundaUtils {
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow {
        private readonly CamundaManager _camundaManager;
        private readonly CamundaConnector _connector;
        private readonly Dictionary<string, Func<Task>> _eventSenders = new Dictionary<string, Func<Task>>();
        private readonly Settings.Settings _settings;

        public MainWindow() {
            InitializeComponent();

            _settings = Settings.Settings.FromConfig();
            _camundaManager = new CamundaManager(CamundaClient.Create(_settings.GetCurrentCamundaUrl()));
            _connector = new CamundaConnector(_settings, new BpmnStub(), new TaskProjectionStub());
            InitializeFromSettings(_settings);
        }

        private void InitializeFromSettings(Settings.Settings settings) {
            UrlProdTextBox.Text = settings.CamundaProdUrl;
            UrlTestTextBox.Text = settings.CamundaTestUrl;
            ExecutorIdTextBox.Text = settings.ExecutorId;
            ProcessVersionTextBox.Text = settings.ProcessVersion.ToString();

            switch(settings.Environment) {
                case Environment.Test:
                    Environment_Test_RadioButton.IsChecked = true;
                    break;
                case Environment.Prod:
                    Environment_Prod_RadioButton.IsChecked = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            switch(settings.QualityControlResult) {
                case QualityControlResult.Good:
                    QC_Good_RadioButton.IsChecked = true;
                    break;
                case QualityControlResult.Error:
                    QC_Error_RadioButton.IsChecked = true;
                    break;
                case QualityControlResult.Crit:
                    QC_Crit_RadioButton.IsChecked = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            RegisterEventSenders();
        }

        private void RegisterEventSenders() {
            _eventSenders[nameof(LaunchTaskButton)] = async () => {
                _settings.ProcessId = await _connector.LaunchTask();
                ProcessIdTextBox.Text = _settings.ProcessId.ToString();
            };

            _eventSenders[nameof(ExecutorSetButton)] = async () => { await _connector.SetExecutor(); };
            _eventSenders[nameof(RejectTaskButton)] = async () => { await _connector.RejectTask(); };
            _eventSenders[nameof(AcceptTaskButton)] = async () => { await _connector.AcceptTask(); };
            _eventSenders[nameof(CompleteTaskButton)] = async () => { await _connector.CompleteTask(); };
            _eventSenders[nameof(FinishQualityControlButton)] = async () => {
                await _connector.QualityControlFinished();
            };
            _eventSenders[nameof(TaskCancelButton)] = async () => { await _connector.CancelTask(); };
            _eventSenders[nameof(ChangeDataButton)] = async () => { await _connector.ChangeData(); };
            _eventSenders[nameof(CommentButton)] = async () => { await _connector.SendComment(); };
            _eventSenders[nameof(LikeCommentButton)] = async () => { await _connector.LikeComment(); };
            _eventSenders[nameof(DislikeCommentButton)] = async () => { await _connector.DislikeComment(); };
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e) {
            var tb = CastTo<TextBox>(sender);

            switch(tb.Name) {
                case nameof(ProcessIdTextBox):
                    if(!Guid.TryParse(tb.Text, out var processId)) {
                        ShowError("Enter valid processId", "Parse Error");
                        return;
                    }

                    _settings.ProcessId = processId;
                    break;
                case nameof(UrlProdTextBox):
                    _settings.CamundaProdUrl = tb.Text;
                    break;
                case nameof(UrlTestTextBox):
                    _settings.CamundaTestUrl = tb.Text;
                    break;
                case nameof(ExecutorIdTextBox):
                    _settings.ExecutorId = string.IsNullOrEmpty(tb.Text) ? null : tb.Text;
                    break;
                case nameof(ProcessVersionTextBox) when string.IsNullOrEmpty(tb.Text):
                    _settings.ProcessVersion = null;
                    break;
                case nameof(ProcessVersionTextBox): {
                    if(!uint.TryParse(tb.Text, out var processVersion)) {
                        ShowError("Enter valid version", "Parse Error");
                        return;
                    }

                    _settings.ProcessVersion = processVersion;
                    break;
                }
            }

            _settings.Save();
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e) {
            var btn = CastTo<Button>(sender);

            if(btn == ProcessIdCopyButton) {
                Clipboard.SetText(ProcessIdTextBox.Text);
            }
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e) {
            var rb = CastTo<RadioButton>(sender);

            switch(rb.Name) {
                case nameof(Environment_Test_RadioButton):
                    _settings.Environment = Environment.Test;
                    break;
                case nameof(Environment_Prod_RadioButton):
                    _settings.Environment = Environment.Prod;
                    break;
                case nameof(QC_Good_RadioButton):
                    _settings.QualityControlResult = QualityControlResult.Good;
                    break;
                case nameof(QC_Error_RadioButton):
                    _settings.QualityControlResult = QualityControlResult.Error;
                    break;
                case nameof(QC_Crit_RadioButton):
                    _settings.QualityControlResult = QualityControlResult.Crit;
                    break;
            }
        }

        private async void Event_Button_Click(object sender, RoutedEventArgs e) {
            var btn = CastTo<Button>(sender);

            if(!_eventSenders.TryGetValue(btn.Name, out var action)) {
                throw new ArgumentException($"Для кнопки {btn.Name} нет зарегистрированного действия");
            }

            await BlockAction(btn, action);
        }

        private async void ShowProjectionButton_Click(object sender, RoutedEventArgs e) {
            var btn = CastTo<Button>(sender);

            await BlockAction(btn, async () => {
                var json = await _connector.GetProjection();
                var myDialog = new MyDialog(json);
                myDialog.ShowDialog();
            });
        }

        private async void Deploy_Button_Click(object sender, RoutedEventArgs args) {
            var btn = CastTo<Button>(sender);

            await BlockAction(btn, async () => {
                DeployLogTextBox.Clear();
                var deploymentId = await _camundaManager.Deploy();
                DeployLogTextBox.AppendText("Deploy success");
                DeployLogTextBox.AppendText($"{_settings.GetCurrentCamundaUrl()}/somepath/{deploymentId}");
            }, e => DeployLogTextBox.AppendText(e.ToString()));
        }

        private static async Task BlockAction(UIElement control,
            Func<Task> action,
            Action<Exception> errorHandler = null) {
            control.IsEnabled = false;
            await DoWithErrorLog(action, errorHandler);
            control.IsEnabled = true;
        }

        private static async Task DoWithErrorLog(Func<Task> action, Action<Exception> errorHandler = null) {
            try {
                await action();
            }
            catch(Exception ex) {
                if(errorHandler == null) {
                    ShowError(ex.Message);
                }
                else {
                    errorHandler(ex);
                }
            }
        }

        private static void ShowError(string text, string caption = "Error") {
            MessageBox.Show(text, caption, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private static T CastTo<T>(object sender) {
            if(!(sender is T control)) {
                throw new ArgumentException($"Данный обработчик предназначен для {typeof(T).Name}");
            }

            return control;
        }

        private void OpenProcessButton_Click(object sender, RoutedEventArgs e) {
            Process.Start(_settings.GetCurrentProcessUrl());
        }

        private async void HappyPathButton_Click(object sender, RoutedEventArgs e) {
            var btn = CastTo<Button>(sender);

            PlayersLogTextBox.Clear();

            await BlockAction(btn, async () => {
                await Player.Play(TimeSpan.FromSeconds(3),
                    s => {
                        PlayersLogTextBox.AppendText($"{s}{System.Environment.NewLine}");
                        return Task.CompletedTask;
                    },
                    new Step("LaunchTask", _eventSenders[nameof(LaunchTaskButton)]),
                    new Step("ExecutorSet", _eventSenders[nameof(ExecutorSetButton)]),
                    new Step("TaskAccepted", _eventSenders[nameof(AcceptTaskButton)]),
                    new Step("TaskAccepted 2", _eventSenders[nameof(AcceptTaskButton)]),
                    new Step("TaskComplete", _eventSenders[nameof(CompleteTaskButton)]),
                    new Step("QualityControlFinished", async () => {
                        var oldQC = _settings.QualityControlResult;
                        _settings.QualityControlResult = QualityControlResult.Good;
                        await _eventSenders[nameof(FinishQualityControlButton)]();
                        _settings.QualityControlResult = oldQC;
                    })
                );

                PlayersLogTextBox.AppendText("Happy path успешно пройден!");
            });
        }

        private async void TaskVersionCopyButton_Click(object sender, RoutedEventArgs e) {
            var btn = CastTo<Button>(sender);

            await BlockAction(btn, async () => { Clipboard.SetText(await _connector.GetCurrentVersion()); });

        }
    }
}
