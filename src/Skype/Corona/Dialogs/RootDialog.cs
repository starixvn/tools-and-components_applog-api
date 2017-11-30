using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Corona.Services;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Connector;

namespace Corona.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private const string UMNotification = "I want to run UM tasks.";

        private const string ServerInfo = "I would like to get the server info.";

        private const string SetSchedule = "I want to set schedule for Event.";

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            if (activity.Text.ToLower().Contains("help") || activity.Text.ToLower().Contains("support") || activity.Text.ToLower().Contains("problem"))
            {
                await context.Forward(new SupportDialog(), this.ResumeAfterSupportDialog, activity, CancellationToken.None).ConfigureAwait(false);
            }
            else
            {
                this.ShowOptions(context);
            }
        }

        private async Task ResumeAfterSupportDialog(IDialogContext context, IAwaitable<object> result)
        {
            await context.PostAsync($"Thanks for contacting our support team. We will response to you later. Bye!");

            context.Wait(this.MessageReceivedAsync);
        }

        private void ShowOptions(IDialogContext context)
        {
            var ran = new Random();
            var messages = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "/App_Data/FunnyMessages.txt");
            var messageIndex = ran.Next(0, messages.Length);
            context.PostAsync(messages[messageIndex]);
            // PromptDialog.Choice(context, this.OnOptionSelected, new List<string> { UMNotification, ServerInfo, SetSchedule }, "I'm Corona. May I help you ?", "Sorry, I didn't get what you say.", 3);
        }

        private async Task OnOptionSelected(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                string optionSelected = await result;

                switch (optionSelected)
                {
                    case UMNotification:
                        // context.Call(new UMTaskDialog(), this.ResumeAfterOptionDialog);
                        
                        context.PostAsync("Em đang được phát triển nha mấy anh chị. Em sẽ sớm hoàn thành trong thời gian sớm nhất. Thanks");
                        break;

                    case ServerInfo:
                        // context.Call(new ServerInfoDialog(), this.ResumeAfterOptionDialog);
                        context.PostAsync("Em đang được phát triển nha mấy anh chị. Em sẽ sớm hoàn thiện trong thời gian sớm nhất. Thanks");
                        break;
                    case SetSchedule:
                        {                            
                            if (context.Activity.From.Id == "29:1l9jv1sTy5SGpPfSM0Ksj0GLWxsFv2nx8_8cVzMwKsyU")
                            {
                                context.Call(new UpdateConversationEventDialog(), this.ResumeAfterOptionDialog);
                            }
                            else
                            {
                                context.PostAsync($"Anh chị không phải là anh Terry nên em không cho dùng chức năng này nha. Anh chị thông cảm. Thanks");                               
                            }
                            break;
                        }
                }
            }
            catch (TooManyAttemptsException ex)
            {
                await context.PostAsync($"Ooops! Too many attemps :(. But don't worry, I'm handling that exception and you can try again!").ConfigureAwait(false);

                context.Wait(this.MessageReceivedAsync);
            }
        }

        private async Task ResumeAfterOptionDialog(IDialogContext context, IAwaitable<object> result)
        {
            try
            {
                var message = await result;

                await context.PostAsync(message.ToString()).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await context.PostAsync($"Failed with message: {ex.Message}").ConfigureAwait(false);
            }
            finally
            {
                context.Wait(this.MessageReceivedAsync);
            }
        }
    }
}