using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Letstalk.Bots.DatabaseLogSender
{
    public partial class DatabaseLogSenderService : ServiceBase
    {
        public DatabaseLogSenderService()
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