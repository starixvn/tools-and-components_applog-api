using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Timers;
using Letstalk.Bots.AppLogSender.Controllers;
using Letstalk.Bots.AppLogSender.Services;

namespace Letstalk.Bots.AppLogSender
{
    public partial class AppLogSenderService : ServiceBase
    {
        private readonly AppLogSenderController appLogSenderController;
        public AppLogSenderService()
        {
            InitializeComponent();
            appLogSenderController = new AppLogSenderController();

        }

        protected override void OnStart(string[] args)
        {
            var timer = new Timer();
            timer.Interval = AppSettingService.Interval * 1000;
            timer.Elapsed += this.Ontimer;
            timer.Start();
        }

        private void Ontimer(object sender, ElapsedEventArgs e)
        {
            appLogSenderController.Send();
        }

        protected override void OnStop()
        {
            // Method intentionally left empty.
        }
    }
}