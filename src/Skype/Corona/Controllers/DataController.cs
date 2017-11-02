using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Corona.Models;
using Corona.Services;

namespace Corona.Controllers
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
        public IList<EventModel2> Events2() => eventService.Events2();

        [HttpGet]
        public EventModel2 ParentEvent2(int ParentId) => eventService.GetDirectParent(ParentId);
        
        [HttpGet]
        public IList<FunctionModel> Functions() => functionService.Functions;

        [HttpGet]
        public IList<ScheduleModel> Schedules() => scheduleService.Schedules;
    }
}
