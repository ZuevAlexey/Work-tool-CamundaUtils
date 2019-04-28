using System;
using System.Threading.Tasks;

namespace CamundaUtils.Core {
    public static class Player {
        public static async Task Play(TimeSpan delay, Func<string, Task> logger, params Step[] steps) {
            for(var i = 0; i < steps.Length; i++) {
                var step = steps[i];
                try {
                    await logger($"Начинаем выполнять шаг {step.Name}");
                    await step.Action();
                    await logger($"Шаг {step.Name} успешно выполнен");
                } catch (Exception ex) {
                    await logger($"Ошибка на шаге {step.Name}");
                    await logger(ex.ToString());
                    return;
                }

                if(i < steps.Length - 1) {
                    await Task.Delay(delay);
                }
            }
        }
    }
}
