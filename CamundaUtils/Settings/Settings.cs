using System;
using System.Configuration;
using System.Linq;

namespace CamundaUtils.Settings {
    public class Settings {
        public string CamundaProdUrl { get; set; }
        public string CamundaTestUrl { get; set; }
        public string BpmnProdUrl { get; set; }
        public string BpmnTestUrl { get; set; }
        public string TaskProjectionProdUrl { get; set; }
        public string TaskProjectionTestUrl { get; set; }
        public uint? ProcessVersion { get; set; }

        public string ExecutorId { get; set; }

        public Guid ProcessId { get; set; }
        public Environment Environment { get; set; } = Environment.Test;
        public QualityControlResult QualityControlResult { get; set; } = QualityControlResult.Good;

        public string GetCurrentProcessUrl() {
            var url = new Uri(GetCurrentCamundaUrl());
            var withoutLastSegment = url.AbsoluteUri.Remove(url.AbsoluteUri.Length - url.Segments.Last().Length);
            return withoutLastSegment + "app/cockpit/default/#/process-instance/c40aa0b2-6b4f-11e9-b5b6-0a580af4011d";
        }
        
        public string GetCurrentCamundaUrl() {
            switch(Environment) {
                case Environment.Test:
                    return CamundaTestUrl;
                case Environment.Prod:
                    return CamundaProdUrl;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static Settings FromConfig() {
            var result = new Settings {
                CamundaProdUrl = ConfigurationManager.AppSettings[nameof(CamundaProdUrl)],
                CamundaTestUrl = ConfigurationManager.AppSettings[nameof(CamundaTestUrl)],
                BpmnProdUrl = ConfigurationManager.AppSettings[nameof(BpmnProdUrl)],
                BpmnTestUrl = ConfigurationManager.AppSettings[nameof(BpmnTestUrl)],
                TaskProjectionProdUrl = ConfigurationManager.AppSettings[nameof(TaskProjectionProdUrl)],
                TaskProjectionTestUrl = ConfigurationManager.AppSettings[nameof(TaskProjectionTestUrl)],
                ExecutorId = ConfigurationManager.AppSettings[nameof(ExecutorId)],
                ProcessVersion = uint.TryParse(ConfigurationManager.AppSettings[nameof(ProcessVersion)],
                    out var processVersion)
                    ? processVersion
                    : (uint?) null
            };

            return result;
        }

        public void Save() {
            var conf = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            conf.AppSettings.Settings[nameof(CamundaProdUrl)].Value = CamundaProdUrl;
            conf.AppSettings.Settings[nameof(CamundaTestUrl)].Value = CamundaTestUrl;
            conf.AppSettings.Settings[nameof(BpmnProdUrl)].Value = BpmnProdUrl;
            conf.AppSettings.Settings[nameof(BpmnTestUrl)].Value = BpmnTestUrl;
            conf.AppSettings.Settings[nameof(TaskProjectionProdUrl)].Value = TaskProjectionProdUrl;
            conf.AppSettings.Settings[nameof(TaskProjectionTestUrl)].Value = TaskProjectionTestUrl;
            conf.AppSettings.Settings[nameof(ExecutorId)].Value = ExecutorId;
            if(ProcessVersion.HasValue)
                conf.AppSettings.Settings[nameof(ProcessVersion)].Value = ProcessVersion.Value.ToString();

            conf.Save();
            ConfigurationManager.RefreshSection(conf.AppSettings.SectionInformation.Name);
        }
    }
}
