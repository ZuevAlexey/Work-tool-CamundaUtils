using System;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;

namespace CamundaUtils.Stubs {
    public class BpmnStub : IBpmnFacadeService {
        public async Task<Guid> LaunchVersionTask(object info, int? processVersion) {
            await Task.Delay(TimeSpan.FromSeconds(2));
            return Guid.NewGuid();
        }

        public async Task FireNamedEvent(EventData eventData) {
            MessageBox.Show(JsonConvert.SerializeObject(eventData, Formatting.Indented), "Event data");
            await Task.Delay(TimeSpan.FromSeconds(2));
        }
    }
}
