using System;
using System.Threading.Tasks;

namespace CamundaUtils.Stubs {
    public interface IBpmnFacadeService {
       Task<Guid> LaunchVersionTask(object info, int? processVersion);
        
       Task FireNamedEvent(EventData eventData);
    }
}