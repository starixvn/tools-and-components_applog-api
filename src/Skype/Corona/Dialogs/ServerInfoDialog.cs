using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Corona.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Connector;

namespace Corona.Dialogs
{
    [Serializable]
    public class ServerInfoDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            var environmentFormDialog = FormDialog.FromForm(this.BuildEnvironmentsForm, FormOptions.PromptInStart);

            context.Call(environmentFormDialog, ResumeAfterEnvironmentFormDialog);
        }

        private IForm<EnvironmentSelectorModel> BuildEnvironmentsForm()
        {
            OnCompletionAsyncDelegate<EnvironmentSelectorModel> processEnvironmentSelected = async (context, state) =>
            {
                await context.PostAsync($"Ok. You are selected {state.Environment}").ConfigureAwait(false);
            };

            return new FormBuilder<EnvironmentSelectorModel>()
                .Field(nameof(EnvironmentSelectorModel.Environment))
                .OnCompletion(processEnvironmentSelected)
                .Build();
        }

        private async Task ResumeAfterEnvironmentFormDialog(IDialogContext context, IAwaitable<EnvironmentSelectorModel> result)
        {
           //// var selected = await result;

            context.Done<object>("Bye!");
        }
    }
}