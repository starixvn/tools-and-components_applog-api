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
    public class UpdateConversationEventDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            var formDialog = FormDialog.FromForm(this.BuildForm, FormOptions.PromptInStart);

            context.Call(formDialog, ResumeAfterFormDialog);
        }

        private IForm<ConversationEventSelectorModel> BuildForm()
        {
            var conversationService = new ConversationService();
            var eventService = new EventService();
            var conversationEvent = new ConversationEventService();

            OnCompletionAsyncDelegate<ConversationEventSelectorModel> processSelected = async (context, state) =>
            {
                if (!conversationEvent.Existing(state.ConversationId, state.EventId))
                {
                    conversationEvent.Insert(new ConversationEventModel
                    {
                        ConversationId = state.ConversationId,
                        EventId = state.EventId,
                        Name = state.Name
                    });

                    await context.PostAsync($"Event is setted the schedule.");
                }
                else
                {
                    await context.PostAsync($"The schedule is setted already. So you cannot add more schedule.");
                }
            };

            return new FormBuilder<ConversationEventSelectorModel>()
                .Field(nameof(ConversationEventSelectorModel.Name))
                .Field(nameof(ConversationEventSelectorModel.ConversationId), validate: async (state, value) =>
                {
                    ValidateResult validateResult = new ValidateResult()
                    {
                        IsValid = true,
                        Value = value
                    };

                    if (!conversationService.Existing(value.ToString()))
                    {
                        validateResult.IsValid = false;
                        validateResult.Feedback = "Not found the Conversation Id. Please input other one.";
                    }

                    return validateResult;

                })
                .Field(nameof(ConversationEventSelectorModel.EventId), validate: async (state, value) =>
                {
                    ValidateResult validateResult = new ValidateResult()
                    {
                        IsValid = true,
                        Value = value
                    };

                    if (!eventService.Existing(Convert.ToInt32(value)))
                    {
                        validateResult.IsValid = false;
                        validateResult.Feedback = "Not found the Event Id. Please input other one.";
                    }

                    return validateResult;
                })
                .Confirm("Are you sure you want to set the schedule?")
                .OnCompletion(processSelected)
                .Build();
        }

        private async Task ResumeAfterFormDialog(IDialogContext context, IAwaitable<ConversationEventSelectorModel> result)
        {
            var selected = await result;

            context.Done<object>("Bye!");
        }
    }
}