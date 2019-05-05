using System;
using System.Threading.Tasks;

namespace CamundaUtils.Stubs {
    public class TaskProjectionStub : ITaskProjectionService {
        public async Task<string> GetVersion(Guid processId) {
            return Guid.NewGuid().ToString();
        }
    }
}
