using System.ServiceProcess;

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