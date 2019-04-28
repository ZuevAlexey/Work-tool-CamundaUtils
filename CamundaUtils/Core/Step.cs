using System;
using System.Threading.Tasks;

namespace CamundaUtils.Core {
    public class Step {
        public string Name { get; }
        public Func<Task> Action { get; }

        public Step(string name, Func<Task> action) {
            Name = name;
            Action = action;
        }
    }
}
