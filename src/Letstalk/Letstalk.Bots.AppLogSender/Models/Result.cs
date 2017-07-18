using System;

namespace Letstalk.Bots.AppLogSender.Models
{
    public class Result
    {
        private readonly Func<bool> func;

        public Result(Func<bool> func)
        {
            this.func = func;
        }

        public bool IsOK => func.Invoke();

        public bool IsNotOK => !IsOK;
    }
}