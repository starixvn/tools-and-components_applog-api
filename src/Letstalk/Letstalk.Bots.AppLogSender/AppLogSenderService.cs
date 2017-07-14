using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Letstalk.Bots.AppLogSender
{
    public partial class AppLogSenderService : ServiceBase
    {
        public AppLogSenderService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // Method intentionally left empty.
        }

        protected override void OnStop()
        {
            // Method intentionally left empty.
        }
    }
}