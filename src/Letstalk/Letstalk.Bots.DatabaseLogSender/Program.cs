using System.ServiceProcess;

namespace Letstalk.Bots.DatabaseLogSender
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new DatabaseLogSenderService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}