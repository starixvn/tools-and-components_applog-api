using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Corona.Model;
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
        readonly IChannelAccountService channelAccountService;

        public ResumeController()
        {
            conversationService = new ConversationService();
            conversationEventService = new ConversationEventService();
            eventService = new EventService();
            channelAccountService = new ChannelAccountService();
        }

        [HttpGet]
        public IList<ConversationModel> Conversations() => conversationService.Conversations;

        [HttpGet]
        public IEnumerable<ChannelAccountModel> ChannelAccount() => channelAccountService.ChannelAccounts;

        [HttpGet]
        public IList<EventModel> Events() => eventService.Events;

        [HttpGet]
        public IList<EventModel> UMEvents() => eventService.Events.Where(x => x.IsUM).ToList();

        [HttpPost]
        public void UpdateEvent(EventModel model) => eventService.Update(model.Id, model);

        [HttpPost]
        public void UpdateMessageSent(int id, bool isSent) => eventService.UpdateMessageSent(id, isSent);

        [HttpGet]
        public IList<ConversationEventModel> ConversationEvents() => conversationEventService.ConversationEvents;

        [HttpGet]
        [HttpPost]
        public async Task<HttpResponseMessage> SendMessage(string message, string conversationId)
        {
            var conversationModel = conversationService.Conversation(conversationId);
            if (conversationModel != null)
            {
                await conversationService.Resume(conversationModel, message);
            }

            var resp = new HttpResponseMessage(HttpStatusCode.OK);

            resp.Content = new StringContent("Message is sent at" + DateTime.Now);

            return resp;
        }

        [HttpPost]
        [HttpGet]
        public async Task<HttpResponseMessage> UMMessage([FromBody] RequestModel requestModel)
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
                            if (requestModel.IsNotification)
                            {
                                eventService.Events.Where(x => x.IsUM).ToList().ForEach(x => eventService.Update(x.Id, new EventModel
                                {
                                    IsSent = requestModel.Events.Id == x.Id,
                                    Start = x.Start.HasValue ? requestModel.Events.Start : x.Start,
                                    End = x.End.HasValue ? requestModel.Events.End : x.End,
                                    IsUM = x.IsUM,
                                    Message = x.Message,
                                    Repeat = x.Repeat
                                }));
                            }
                            else
                            {
                                eventService.UpdateMessageSent(requestModel.Events.Id, true);
                                if(requestModel.Events.IsUM)
                                {
                                    UMEvents().ToList().ForEach(x => eventService.UpdateMessageSent(x.Id, false));
                                }
                            }
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

        [HttpGet]
        [HttpPost]
        public async Task<HttpResponseMessage> SendMessage(string message, IList<ConversationModel> conversations)
        {
            foreach (var con in conversations)
            {
                await conversationService.Resume(con, message);
            }

            var resp = new HttpResponseMessage(HttpStatusCode.OK);

            resp.Content = new StringContent("Message is sent at " + DateTime.Now);

            return resp;
        }
    }
}