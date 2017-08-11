namespace Letstalk.Bots.AppLogSender
{
    using System;
    using System.ServiceProcess;
    using System.Timers;
    using Fanex.Data;
    using Fanex.Data.OldConnectionStringProvider;
    using Fanex.Logging;
    using Letstalk.Bots.AppLogSender.Services;
    using ONEbook.UM.Services.Impl;

    public partial class AppLogSenderService : ServiceBase
    {
        private readonly IMessageSender messageSender;
        private readonly MaintenanceService maintenanceService;

        public AppLogSenderService()
        {
            InitializeComponent();

            LogService.Info("Service is initializing at " + DateTime.Now);

            AppSettings.SetupCurrentBaseDirectory();
            AppSettings.SetupConfigurationValues();
            AppSettings.Verify();

            DbSettingProviderManager
                .StartNewSession()
                .Use(new OldConnectionStringProvider())
                .UseDefaultDbSettingProvider(AppSettings.FanexDbSettingPath)
                .Run();            

            messageSender = new MessageSenderImpl();
            maintenanceService = new MaintenanceService();
        }

        protected override void OnStart(string[] args)
        {
            LogService.Info("Service is started at " + DateTime.Now);

            var timer = new Timer
            {
                Interval = AppSettings.Interval * 1000
            };
            timer.Elapsed += Ontimer;
            timer.Start();
        }

        private void Ontimer(object sender, ElapsedEventArgs e)
        {
            if (!maintenanceService.IsUnderMaintenance())
            {
                messageSender
                        .Run(AppSettings.IsProduction ? DateTime.Now.AddSeconds(-AppSettings.Interval) : DateTime.Now.AddDays(-5), AppSettings.IsProduction);
            }
        }

        protected override void OnStop()
        {
            LogService.Info("Service is stopped " + DateTime.Now);
        }
    }
}