using System;
using System.Configuration;

namespace CamundaUtils.Settings {
    public class Settings {
        public string CamundaProdUrl { get; set; }
        public string CamundaTestUrl { get; set; }
        public uint? ProcessVersion { get; set; }

        public string ExecutorId { get; set; }

        public Guid ProcessId { get; set; }
        public Environment Environment { get; set; } = Environment.Test;
        public QualityControlResult QualityControlResult { get; set; } = QualityControlResult.Good;

        public string GetCurrentUrl() {
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
            conf.AppSettings.Settings[nameof(ExecutorId)].Value = ExecutorId;
            if(ProcessVersion.HasValue)
                conf.AppSettings.Settings[nameof(ProcessVersion)].Value = ProcessVersion.Value.ToString();

            conf.Save();
            ConfigurationManager.RefreshSection(conf.AppSettings.SectionInformation.Name);
        }
    }
}
