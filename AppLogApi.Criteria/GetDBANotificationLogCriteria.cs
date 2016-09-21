namespace AppLogApi.Criteria
{
    using Fanex.Data.Repository;
    public class GetDBANotificationLogCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "dba_getNotification";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
