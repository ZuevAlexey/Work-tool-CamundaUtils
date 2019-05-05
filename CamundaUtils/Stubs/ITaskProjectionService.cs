using System;
using System.Threading.Tasks;

namespace CamundaUtils.Stubs {
    public interface ITaskProjectionService {
        Task<string> GetVersion(Guid processId);
    }
}
