namespace AppLogApi.Models.AppLog
{
    public class Machine
    {
        public int MachineId { get; set; }

        public string MachineName { get; set; }

        public string MachineIP { get; set; }

        public bool IsProduction { get; set; }
    }
}
