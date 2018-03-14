using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Corona.ScheduleTask.WindowService;

namespace Corona.ScheduleStask.WindowService
{
    public partial class ScheduleTaskService : ServiceBase
    {
        readonly SenderService senderService;
        private object objLockObject = new object();

        public ScheduleTaskService()
        {
            InitializeComponent();
            senderService = new SenderService();
        }

        protected override void OnStart(string[] args)
        {
            var timer = new Timer
            {
                Interval = AppSetting.Interval * 1000
            };

            timer.Elapsed += Ontimer;
            timer.Enabled = true;
        }

        protected override void OnStop()
        {

        }
        private void Ontimer(object sender, ElapsedEventArgs e)
        {
            try
            {
                senderService.Run();
            }
            catch (Exception ex)
            {
                Fanex.Logging.LogService.Error(ex.Message);
            }
        }
    }
}
