using Abp.Domain.Entities.Auditing;

namespace Bwr.Exchange.Shared
{
    public class PhoneBase : FullAuditedEntity
    {
        public string PhoneNumber { get; set; }
    }
}
