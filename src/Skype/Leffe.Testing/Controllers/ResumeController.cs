using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Corona.Models;
using Newtonsoft.Json;
using Skype.Bot.Services;

namespace Leffe
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
        public HttpResponseMessage PostMessage([FromBody] MessageModel messageModel)
        {
            var conversationModel = conversationService.Conversation(messageModel.ConversationId);
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            resp.Content = new StringContent("Message is sent at" + DateTime.Now);
            if (conversationModel != null)
            {
                var result = conversationService.Resume(conversationModel, messageModel.Message);

                if (!result)
                {
                    resp = new HttpResponseMessage(HttpStatusCode.ExpectationFailed);
                    resp.Content = new StringContent("Message is sent failed at" + DateTime.Now);
                }
            }

            return resp;
        }

        [HttpPost]
        public HttpResponseMessage SendMessage(string message, IList<ConversationModel> conversations)
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
        public HttpResponseMessage SendMessage([FromBody]RequestModel model)
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);

            var eventConversations = conversationEventService.ConversationEvents
                .Where(x => x.EventId == model.Event.Id);

            if (eventConversations.Any())
            {
                
                var conversations = conversationService.Conversations.Where(x => eventConversations.Any(a => a.ConversationId == x.Id));

                if (conversations.Any())
                {
                    conversations.ToList().ForEach(x =>
                    {
                        conversationService.Resume(x, model.Event.Message);
                    });

                    if (model.IsUpdateData)
                    {
                        UpdateEventData(model.Event);
                    }

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

        private void UpdateEventData(EventModel model)
        {
            switch (model.KeyName)
            {
                case EventKeyModel.UMNotified:
                case EventKeyModel.UMExtended:

                    {
                        eventService.UpdateMessageSent(model.Id, model.IsSent);
                        scheduleService.UpdateSchedule(model.Schedule.Id, model.Schedule.StartDate, model.Schedule.EndDate);
                        break;
                    }
                case EventKeyModel.UMStarted:
                case EventKeyModel.UMRebootServer:
                case EventKeyModel.UMDisabledSQL:
                case EventKeyModel.UMEnabledSQL:
                case EventKeyModel.UMRebootServerProcessing:
                case EventKeyModel.UMPingServerProcessing:
                    {
                        eventService.UpdateMessageSent(model.Id, model.IsSent);
                        break;
                    }
                case EventKeyModel.UMEnded:
                    {
                        var events = eventService.EventsList().Where(x => x.Functions.Id == model.Functions.Id);

                        events.ToList().ForEach(x =>
                        {
                            eventService.UpdateMessageSent(x.Id, false);
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