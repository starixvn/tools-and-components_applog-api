using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Timers;
using Letstalk.Bots.AppLogSender.Services;

namespace Letstalk.Bots.AppLogSender
{
    public partial class AppLogSenderService : ServiceBase
    {
        private readonly MessageSender messageSender = new MessageSenderImpl();

        public AppLogSenderService()
        {
            InitializeComponent();

            AppSettings.Verify();
        }

        protected override void OnStart(string[] args)
        {
            var timer = new Timer();
            timer.Interval = AppSettings.Interval * 1000;
            timer.Elapsed += this.Ontimer;
            timer.Start();
        }

        private void Ontimer(object sender, ElapsedEventArgs e)
        {
            messageSender.Run();
        }

        protected override void OnStop()
        {
            // Method intentionally left empty.
            // TODO: write message to msite, service is stopped.
        }
    }
}