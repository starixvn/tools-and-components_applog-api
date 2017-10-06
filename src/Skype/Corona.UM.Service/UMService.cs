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
using System.Threading.Tasks;
using System.Timers;
using RestSharp;

namespace Corona.UM.Service
{
    public partial class UMService : ServiceBase
    {
        public UMService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            var timer = new Timer
            {
                Interval = AppSettingService.Interval * 1000
            };

            timer.Elapsed += Ontimer;
            timer.Start();
        }

        protected override void OnStop()
        {
            // Method intentionally left empty.
        }

        private static void Ontimer(object sender, ElapsedEventArgs e)
        {
            try
            {
                UMSenderService.RunService();
            }
            catch (Exception ex)
            {
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data\\log.txt");
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine(ex.Message);                    
                }                
            }
        }
    }
}
