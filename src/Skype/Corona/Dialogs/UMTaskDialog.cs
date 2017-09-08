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
    public class UMTaskDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            var environmentFormDialog = FormDialog.FromForm(this.BuildForm, FormOptions.PromptInStart);

            context.Call(environmentFormDialog, ResumeAfterFormDialog);                        
        }
 
        private IForm<UMTaskModel> BuildForm()
        {
            OnCompletionAsyncDelegate<UMTaskModel> processSelected = async (context, state) =>
            {
                await context.PostAsync($"Ok. You are selected option {state.UMTaskList}");
            };

            return new FormBuilder<UMTaskModel>()                
                .OnCompletion(processSelected)
                .Build();
        }

        private async Task ResumeAfterFormDialog(IDialogContext context, IAwaitable<UMTaskModel> result)
        {
            var selected = await result;

            context.Done<object>("Bye!");
        }
    }
}