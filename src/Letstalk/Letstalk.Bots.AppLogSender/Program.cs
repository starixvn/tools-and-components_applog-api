using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Fanex.Logging;
using Letstalk.Bots.AppLogSender.Services;

namespace Letstalk.Bots.AppLogSender
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
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
