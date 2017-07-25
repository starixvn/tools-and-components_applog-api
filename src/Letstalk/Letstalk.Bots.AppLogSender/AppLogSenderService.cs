namespace Letstalk.Bots.AppLogSender
{
    using Fanex.Data;
    using Fanex.Data.OldConnectionStringProvider;
    using Fanex.Logging;
    using Letstalk.Bots.AppLogSender.Services;
    using ONEbook.UM.Services.Impl;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Diagnostics;
    using System.Linq;
    using System.ServiceProcess;
    using System.Text;
    using System.Timers;

    public partial class AppLogSenderService : ServiceBase
    {
        private readonly MessageSender messageSender = new MessageSenderImpl();
        private readonly MaintenanceService maintenanceService = new MaintenanceService();

        public AppLogSenderService()
        {
            InitializeComponent();

            LogService.Info("Service is initializing...");

            AppSettings.Verify();

            DbSettingProviderManager
                .StartNewSession()
                .Use(new OldConnectionStringProvider())
                .UseDefaultDbSettingProvider("~/App_Data")
                .Run();
        }

        protected override void OnStart(string[] args)
        {
            LogService.Info("Service is starting...");

            var timer = new Timer();
            timer.Interval = AppSettings.Interval * 1000;
            timer.Elapsed += this.Ontimer;
            timer.Start();
        }

        private void Ontimer(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (maintenanceService.IsUnderMaintenance())
                {
                    return;
                }

                messageSender.Run();
            }
            catch (Exception ex)
            {
                LogService.Error(ex.Message);
            }
        }

        protected override void OnStop()
        {
            LogService.Info("Service is started...");
        }
    }
}