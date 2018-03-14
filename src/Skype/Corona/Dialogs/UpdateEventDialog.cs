using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Corona.Models;
using Corona.Services;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.FormFlow.Advanced;
using Microsoft.Bot.Connector;

namespace Corona.Dialogs
{
    [Serializable]
    public class UpdateEventDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            throw new NotImplementedException();
        }

        //public async Task StartAsync(IDialogContext context)
        //{
        //    var formDialog = FormDialog.FromForm(this.BuildForm, FormOptions.PromptInStart);

        //    context.Call(formDialog, ResumeAfterFormDialog);
        //}

        //private IForm<EventSelectorModel> BuildForm()
        //{
        //    var conversationService = new ConversationService();
        //    var eventService = new EventService();
        //    var conversationEvent = new ConversationEventService();

        //    OnCompletionAsyncDelegate<EventSelectorModel> processSelected = async (context, state) =>
        //    {                
        //        var convesation = conversationService.Conversations.First(x => x.Name == state.Conversation);
        //        var eventId = eventService.Insert(new EventModel
        //        {
        //            Message = state.Message,
        //            Repeat = state.Repeat.Value,
        //            Start = state.StartDate
        //        });

        //        conversationEvent.Insert(new ConversationEventModel
        //        {
        //            ConversationId = convesation.Id,
        //            EventId = eventId,
        //            Name = state.EventName
        //        });

        //        await context.PostAsync($"Event is updated to database.");
        //    };

        //    return new FormBuilder<EventSelectorModel>()
        //        .Field(nameof(EventSelectorModel.EventName))
        //        .Field(nameof(EventSelectorModel.Conversation), validate: async (state, value) =>
        //        {
        //            ValidateResult validateResult = new ValidateResult()
        //            {
        //                IsValid = true,
        //                Value = value
        //            };
        //            var isConversation = false;

        //            if (conversationService.Conversations.Any())
        //            {
        //                var isExisting = conversationService.Conversations.Where(x => x.Name == value.ToString());

        //                isConversation = isExisting != null;
        //            }

        //            if (isConversation == false)
        //            {
        //                validateResult.IsValid = false;
        //                validateResult.Feedback = $"Not found the conversation name {value}. Please input again";
        //            }

        //            return validateResult;

        //        })
        //        .Field(nameof(EventSelectorModel.StartDate), validate: async (state, value) =>
        //        {
        //            ValidateResult validateResult = new ValidateResult()
        //            {
        //                IsValid = true,
        //                Value = value.ToString().ToLower() == "now" ? DateTime.Now : value
        //            };

        //            return validateResult;
        //        })
        //        .Field(nameof(EventSelectorModel.Message))
        //        .Field(nameof(EventSelectorModel.Repeat))
        //        .Confirm("Are the information correct?" +
        //        "\n\rEvent Name: {EventName}" +
        //        "\n\rConversation Name: {Conversation}" +
        //        "\n\rStart date: {StartDate}" +
        //        "\n\rMessage: {Message}" +
        //        "\n\rSchedule: {Repeat}")
        //        .OnCompletion(processSelected)
        //        .Build();
        //}

        private async Task ResumeAfterFormDialog(IDialogContext context, IAwaitable<EventSelectorModel> result)
        {
            ////var selected = await result;

            context.Done<object>("Bye!");
        }
    }
}