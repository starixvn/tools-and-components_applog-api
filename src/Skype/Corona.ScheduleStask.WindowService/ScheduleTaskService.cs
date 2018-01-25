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
            timer.Start();
        }

        protected override void OnStop()
        {

        }
        private void Ontimer(object sender, ElapsedEventArgs e)
        {
            try
            {
                senderService.Run2();                
            }
            catch (Exception ex)
            {
                //var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data\\log.txt");
                //using (StreamWriter sw = File.AppendText(path))
                //{
                //    sw.WriteLine(ex.Message);
                //}

                Fanex.Logging.LogService.Error(ex.Message);
            }
        }
    }
}
