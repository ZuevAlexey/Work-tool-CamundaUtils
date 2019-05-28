using System;
using System.Threading.Tasks;
using CamundaUtils.Stubs;
using Newtonsoft.Json;

namespace CamundaUtils.Core {
    public class CamundaConnector {
        private readonly IBpmnFacadeService _bpmn;
        private readonly Settings.Settings _settings;
        private readonly ITaskProjectionService _taskProjectionService;

        public CamundaConnector(Settings.Settings settings,
            IBpmnFacadeService bpmn,
            ITaskProjectionService taskProjectionService) {
            _settings = settings;
            _bpmn = bpmn;
            _taskProjectionService = taskProjectionService;
        }

        public async Task<Guid> LaunchTask() {
            return await _bpmn.LaunchVersionTask(new {
                TaskID = "e41a1c64-eb53-41dd-b4e5-892024451ea1",
                DeadLines = new {
                    Deadline = "2019-05-01T11=05=00+03=00"
                },
                Source = "Test",
                SourceTaskId = "e41a1c64-eb53-41dd-b4e5-892024451ea8",
                Initiator = "It's me",
                TaskInfo = new {
                    Title = "Test",
                    Description = "It's my task",
                    Domain = "domain.zone"
                },
                Type = "0a19432c-21b9-4d77-ad6b-19da5ec36da0",
                Price = new {
                    Cost = "1900",
                    Currency = "RUR"
                },
                Attachments = (object) null
            }, (int?) _settings.ProcessVersion);
        }

        public async Task SetExecutor() {
            await _bpmn.FireNamedEvent(await CreateEventData("ExecutorSet", new {
                ExecutorID = _settings.ExecutorId,
                Message = "ExecutorSet",
                Deadline = "2019-04-24T15:40:00Z"
            }));
        }

        public async Task RejectTask() {
            await _bpmn.FireNamedEvent(await CreateEventData("TaskRejected", new {
                Message = "Bad",
                CommentId = "e41a1c64-eb53-41dd-b4e5-892024451ea1"
            }));
        }

        public async Task AcceptTask() {
            await _bpmn.FireNamedEvent(await CreateEventData("TaskAccepted", new {}));
        }

        public async Task CompleteTask() {
            await _bpmn.FireNamedEvent(await CreateEventData("TaskComplete", new {
                CommentId = "e41a1c64-eb53-41dd-b4e5-892024451ea1",
                Message = "All done!"
            }));
        }

        public async Task QualityControlFinished() {
            await _bpmn.FireNamedEvent(await CreateEventData("QualityControlFinished", new {
                Result = (int) _settings.QualityControlResult,
                QaComment = "Quality control comment",
                QaAttachments = new [] {
                    new {
                        AttachmentUrl = "aUrl",
                        AttachmentName = "aName"
                    }
                }
            }));
        }

        public async Task CancelTask() {
            await _bpmn.FireNamedEvent(await CreateEventData("TaskCanceled", new {}));
        }

        public async Task ChangeData() {
            await _bpmn.FireNamedEvent(await CreateEventData("ChangeData", new {
                SomeProperty = "someValue"
            }));
        }

        public async Task SendComment() {
            await _bpmn.FireNamedEvent(await CreateEventData("Comment", new {
                Comments = new [] {
                    new {
                        Author = "6CB60B0C-67C4-64B3-8AFE-F9205481479F",
                        Message = "My comment"
                    }
                }
            }));
        }

        public async Task LikeComment() {
            await _bpmn.FireNamedEvent(await CreateEventData("CommentLike", new {}));
        }
        
        public async Task DislikeComment() {
            await _bpmn.FireNamedEvent(await CreateEventData("CommentDislike", new {}));
        }

        public async Task<string> GetProjection() {
            return JsonConvert.SerializeObject(
                await _taskProjectionService.GetProjection(_settings.ProcessId), Formatting.Indented);
        }

        public async Task<string> GetCurrentVersion() {
            return await _taskProjectionService.GetVersion(_settings.ProcessId);
        }

        private async Task<EventData> CreateEventData(string name, object parameters) {
            return new EventData {
                Name = name,
                ProcessId = _settings.ProcessId,
                Version = await GetCurrentVersion(),
                Params = parameters
            };
        }
    }
}
