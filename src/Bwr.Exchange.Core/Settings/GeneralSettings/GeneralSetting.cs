using Abp.Domain.Entities.Auditing;

namespace Bwr.Exchange.Settings.GeneralSettings
{
    public class GeneralSetting : FullAuditedEntity
    {
        public string CompanyName { get; set; }
        public string DbSuffex { get; set; }
        public string EditPassword { get; set; }

    }
}
