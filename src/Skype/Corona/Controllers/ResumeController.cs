using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Corona.Models;
using Corona.Services;
using Newtonsoft.Json;

namespace Corona.Controllers
{
    public class ResumeController : ApiController
    {
        readonly IConversationService conversationService;
        readonly IConversationEventService conversationEventService;
        readonly IEventService eventService;
        readonly IScheduleService scheduleService;

        public ResumeController()
        {
            conversationService = new ConversationService();
            conversationEventService = new ConversationEventService();
            eventService = new EventService();
            scheduleService = new ScheduleService();
        }

        [HttpPost]
        public async Task<HttpResponseMessage> SendMessage(string message, int conversationId)
        {
            var conversationModel = conversationService.Conversation(conversationId);
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            resp.Content = new StringContent("Message is sent at" + DateTime.Now);
            if (conversationModel != null)
            {
                var result = conversationService.Resume(conversationModel, message);

                if (!result)
                {
                    resp = new HttpResponseMessage(HttpStatusCode.ExpectationFailed);
                    resp.Content = new StringContent("Message is sent failed at" + DateTime.Now);
                }
            }

            return resp;
        }

        [HttpPost]
        public async Task<HttpResponseMessage> UMMessage([FromBody] UMRequestModel requestModel)
        {
            try
            {
                var resp = new HttpResponseMessage(HttpStatusCode.OK);

                if (requestModel.Events != null)
                {
                    var umEventConversations = conversationEventService.ConversationEvents.Where(x => x.EventId == requestModel.Events.Id).Select(a => a.ConversationId);

                    if (umEventConversations.Any())
                    {
                        var conversations = conversationService.Conversations.Where(x => umEventConversations.Contains(x.Id));

                        var result = await SendMessage(requestModel.Events.Message, conversations.ToList());

                        if (result.StatusCode == HttpStatusCode.OK)
                        {
                            UpdateUMData(requestModel);
                        }

                        return result;
                    }
                }

                resp.Content = new StringContent("Event is not found.");

                return resp;
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.OK);

                resp.Content = new StringContent(ex.Message);

                return resp;
            }
        }

        private void UpdateUMData(UMRequestModel requestModel)
        {
            if (requestModel.IsNotification)
            {
                eventService
                    .Events
                    .Where(x => x.IsUM)
                    .ToList()
                    .ForEach(x => eventService.Update(x.Id, new EventModel
                    {
                        IsSent = requestModel.Events.Id == x.Id,
                        Start = x.Start.HasValue ? requestModel.Events.Start : x.Start,
                        End = x.End.HasValue ? requestModel.Events.End : x.End,
                        IsUM = x.IsUM,
                        Message = x.Message,
                        Repeat = x.Repeat,
                        GMT = x.GMT,
                        MessageFile = x.MessageFile
                    }));
            }
            else
            {
                var umEvent = eventService.Events.Where(x => x.IsUM);
                eventService.UpdateMessageSent(requestModel.Events.Id, true);

                if (!requestModel.IsUM)
                {
                    umEvent
                        .ToList()
                        .ForEach(x => eventService.UpdateMessageSent(x.Id, false));
                }
                else
                {
                    if (requestModel.IsExtend)
                    {
                        umEvent
                            .Where(x => x.Id != requestModel.Events.Id)
                            .ToList()
                            .ForEach(x => eventService.UpdateEvent(x.Id, x.Start, x.End.HasValue ? requestModel.Events.End : x.End));
                    }
                }
            }
        }

        [HttpPost]
        public async Task<HttpResponseMessage> SendMessage(string message, IList<ConversationModel> conversations)
        {
            foreach (var con in conversations)
            {
                conversationService.Resume(con, message);
            }

            var resp = new HttpResponseMessage(HttpStatusCode.OK);

            resp.Content = new StringContent("Message is sent at " + DateTime.Now);

            return resp;
        }

        [HttpPost]
        public async Task<HttpResponseMessage> SendMessage([FromBody]RequestModel model)
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);

            if (model.Events.Any())
            {
                model.Events.ToList().ForEach(e =>
                {
                    var eventConversationIds = conversationEventService.ConversationEvents.Where(x => x.EventId == e.Id).Select(x => x.ConversationId);
                    if (eventConversationIds.Any())
                    {
                        var conversations = conversationService.Conversations.Where(x => eventConversationIds.Contains(x.Id));

                        if (conversations.Any())
                        {
                            conversations.ToList().ForEach(x =>
                            {
                                var result = conversationService.Resume(x, e.Message);

                                if (result)
                                {
                                    eventService.UpdateEvent(e.Id, e.Start, e.End);
                                }
                            });

                            resp.Content = new StringContent("Message is sent at " + DateTime.Now);
                        }
                    }
                });
            }

            return resp;
        }

        [HttpPost]
        public async Task<HttpResponseMessage> SendMessage2([FromBody]RequestModel model)
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);

            var eventConversationIds = conversationEventService.ConversationEvents.Where(x => x.EventId == model.Event2.Id).Select(x => x.ConversationId);
            if (eventConversationIds.Any())
            {
                var conversations = conversationService.Conversations.Where(x => eventConversationIds.Contains(x.Id));

                if (conversations.Any())
                {
                    conversations.ToList().ForEach(x =>
                    {
                        conversationService.Resume(x, model.Event2.Message);
                    });

                    UpdateEventData(model.Event2);

                    resp.Content = new StringContent("Message is sent at " + DateTime.Now);
                }
                else
                {
                    resp.Content = new StringContent("Could not found any conversation.");
                }
            }
            else
            {
                resp.Content = new StringContent("Could not found any event conversation.");
            }

            return resp;
        }

        private void UpdateEventData(EventModel2 model)
        {
            switch (model.KeyName)
            {
                case EventKeyModel.UMNotified:
                case EventKeyModel.UMExtended:
                    {
                        eventService.UpdateMessageSent2(model.Id, model.IsSent);
                        scheduleService.UpdateSchedule(model.Schedule.Id, model.Schedule.StartDate, model.Schedule.EndDate);
                        break;
                    }
                case EventKeyModel.UMStarted:
                    {
                        eventService.UpdateMessageSent2(model.Id, model.IsSent);
                        break;
                    }
                case EventKeyModel.UMEnded:
                    {
                        var events = eventService.Events2List().Where(x => x.Functions.Id == model.Functions.Id);

                        events.ToList().ForEach(x =>
                        {
                            eventService.UpdateMessageSent2(x.Id, false);
                        });
                        break;
                    }
                default:
                    scheduleService.UpdateSchedule(model.Schedule.Id, model.Schedule.StartDate, model.Schedule.EndDate);
                    break;
            }
        }
    }
}