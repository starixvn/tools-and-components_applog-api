namespace AppLogApi.Criteria
{
    using System;
    using Fanex.Data.Repository;

    public class GetAppLogCriteria : CriteriaBase
    {
        public int? CategoryId { get; set; }

        public int? MachineId { get; set; }

        public string Severity { get; set; }

        public bool IsProduction { get; set; }

        public int ClientTimeZone { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public override string GetSettingKey()
        {
            return "IS_SelAppLog_All";
        }

        public override bool IsValid()
        {
            return FromDate > DateTime.MinValue
                    && ToDate > DateTime.MinValue
                    && ToDate >= FromDate
                    && ToDate <= FromDate.AddDays(1);
        }
    }
}
