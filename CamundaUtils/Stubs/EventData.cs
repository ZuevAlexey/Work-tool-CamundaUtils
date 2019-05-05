using System;

namespace CamundaUtils.Stubs {
    public class EventData {
        public object Params { get; set; }
        public Guid ProcessId { get; set; }
        public string Version { get; set; }
        public string Name { get; set; }
        public string Source { get; set; }
        public string Status { get; set; }
    }
}
