using System;
using System.Threading.Tasks;

namespace CamundaUtils.Stubs {
    public class TaskProjectionStub: ITaskProjectionService {
        public async Task<string> GetVersion(Guid processId) {
            return Guid.NewGuid().ToString();
        }

        public async Task<object> GetProjection(Guid processId) {
            return new {
                id = Guid.NewGuid(),
                time = DateTime.Now,
                data = new {
                    prop = 1,
                    prop2 = "value"
                }
            };
        }
    }
}
