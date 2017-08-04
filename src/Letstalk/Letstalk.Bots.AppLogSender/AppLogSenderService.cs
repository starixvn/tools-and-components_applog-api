namespace Letstalk.Bots.AppLogSender
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Configuration;
    using System.Data;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.ServiceProcess;
    using System.Text;
    using System.Timers;
    using Fanex.Data;
    using Fanex.Data.OldConnectionStringProvider;
    using Fanex.Logging;
    using Letstalk.Bots.AppLogSender.Services;
    using ONEbook.UM.Services.Impl;

    public partial class AppLogSenderService : ServiceBase
    {
        private readonly MessageSender messageSender;
        private readonly MaintenanceService maintenanceService;

        public AppLogSenderService()
        {
            InitializeComponent();

            try
            {
                LogService.Info("Service is initializing at " + DateTime.Now);

                DbSettingProviderManager
                    .StartNewSession()
                    .Use(new OldConnectionStringProvider())
                    .UseDefaultDbSettingProvider("~/App_Data")
                    .Run();

                AppSettings.SetupCurrentBaseDirectory();
                AppSettings.SetupConfigurationValues();
                AppSettings.Verify();

                messageSender = new MessageSenderImpl();
                maintenanceService = new MaintenanceService();

            }
            catch (Exception ex)
            {
                LogService.Error("initializing: " + ex.Message);
            }
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
                if (!maintenanceService.IsUnderMaintenance())
                {
                    if (AppSettings.IsProduction)
                    {
                        messageSender
                            .Run(DateTime.Now.AddSeconds(-AppSettings.Interval), true);
                    }
                    else
                    {
                        messageSender
                        .Run(DateTime.Now.AddDays(-5), false);
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex.Message);
            }
        }

        protected override void OnStop()
        {
            LogService.Info("Service is stopped " + DateTime.Now);
        }
    }
}