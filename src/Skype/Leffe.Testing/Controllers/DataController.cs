using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using Corona.Models;
using Skype.Bot.Services;

namespace Leffe
{
    public class DataController : ApiController
    {
        readonly IConversationService conversationService;
        readonly IConversationEventService conversationEventService;
        readonly IEventService eventService;
        readonly IChannelAccountService channelAccountService;
        readonly IFunctionService functionService;
        readonly IScheduleService scheduleService;

        public DataController()
        {
            conversationService = new ConversationService();
            conversationEventService = new ConversationEventService();
            eventService = new EventService();
            channelAccountService = new ChannelAccountService();
            functionService = new FunctionService();
            scheduleService = new ScheduleService();
        }

        [HttpGet]
        public IList<ConversationModel> Conversations() => conversationService.Conversations;

        [HttpGet]
        public IEnumerable<ChannelAccountModel> ChannelAccount() => channelAccountService.ChannelAccounts;

        [HttpPost]
        public void UpdateMessageSent(int id, bool isSent) => eventService.UpdateMessageSent(id, isSent);

        [HttpGet]
        public IList<ConversationEventModel> ConversationEvents() => conversationEventService.ConversationEvents;

        [HttpGet]
        public IList<EventModel> Events() => eventService.Events();

        [HttpGet]
        public EventModel ParentEvent(int ParentId) => eventService.GetDirectParent(ParentId);
        
        [HttpGet]
        public List<FunctionModel> Functions() => functionService.Functions;

        [HttpGet]
        public IList<ScheduleModel> Schedules() => scheduleService.Schedules;
    }
}
