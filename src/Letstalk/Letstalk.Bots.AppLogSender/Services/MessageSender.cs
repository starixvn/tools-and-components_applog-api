using System;
using Letstalk.Bots.AppLogSender.Models;

namespace Letstalk.Bots.AppLogSender.Services
{
    public interface MessageSender
    {
        MessageSender Send(IContent content);

        void To(IGroup group);
    }

    public class MessageSenderImpl : MessageSender
    {
        public MessageSender Send(IContent content)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            throw new NotImplementedException();
        }

        public void To(IGroup group)
        {
            throw new NotImplementedException();
        }
    }
}