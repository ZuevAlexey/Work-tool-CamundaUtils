using System;
using System.Threading.Tasks;

namespace CamundaUtils.Connectors {
    public class CamundaConnector {
        private Settings.Settings _settings;

        public CamundaConnector(Settings.Settings settings) {
            _settings = settings;
        }

        public async Task<string> LaunchTask() {
            await Task.Delay(1000);
            return Guid.NewGuid().ToString();
        }
        
        public async Task<string> SetExecutor() {
            await Task.Delay(1000);
            return Guid.NewGuid().ToString();
        }
        
        public async Task<string> RejectTask() {
            await Task.Delay(1000);
            return Guid.NewGuid().ToString();
        }
        
        public async Task<string> AcceptTask() {
            await Task.Delay(1000);
            return Guid.NewGuid().ToString();
        }
        
        public async Task<string> CompleteTask() {
            await Task.Delay(1000);
            return Guid.NewGuid().ToString();
        }
        
        public async Task<string> QualityControlFinished() {
            await Task.Delay(1000);
            return Guid.NewGuid().ToString();
        }
        
        public async Task<string> CancelTask() {
            await Task.Delay(1000);
            return Guid.NewGuid().ToString();
        }
        
        public async Task<string> ChangeData() {
            await Task.Delay(1000);
            return Guid.NewGuid().ToString();
        }
        
        public async Task<string> SendComment() {
            await Task.Delay(1000);
            return Guid.NewGuid().ToString();
        }
    }
}
