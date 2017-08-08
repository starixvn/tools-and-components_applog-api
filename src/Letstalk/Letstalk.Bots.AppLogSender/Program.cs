using System;
using System.ServiceProcess;
using Fanex.Logging;

namespace Letstalk.Bots.AppLogSender
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static void Main()
        {
            try
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                new AppLogSenderService()
                };
                ServiceBase.Run(ServicesToRun);
            }
            catch (Exception ex)
            {
                LogService.Error(ex.Message);
            }
        }
    }
}