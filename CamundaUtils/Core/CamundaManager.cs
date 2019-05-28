using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Camunda.Api.Client;
using Camunda.Api.Client.Deployment;

namespace CamundaUtils.Core {
    public class CamundaManager {
        private readonly CamundaClient _client;
        private readonly string _deploymentFolder = "D:\\rootFoler";

        private readonly List<string> _filesToDeploy = new List<string> {
            "process.bpmn",
            "someScript.js",
            "anotherScript.js"
        };

        public CamundaManager(CamundaClient client) {
            _client = client;
        }

        public async Task<string> Deploy() {
            var deployment = await _client.Deployments.Create("Deployment name",
                false, false, "CamundaUtils", null,
                CreateResourceData());
            return deployment.Id;
        }

        private ResourceDataContent[] CreateResourceData() {
            return _filesToDeploy.Select(e => new ResourceDataContent(
                File.OpenRead(Path.Combine(_deploymentFolder, e)), e)).ToArray();
        }
    }
}
