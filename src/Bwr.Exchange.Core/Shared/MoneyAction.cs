using Abp.Domain.Entities.Auditing;
using System;

namespace Bwr.Exchange.Shared
{
    public class MoneyAction : FullAuditedEntity
    {
        public DateTime Date { get; set; }
        public string Note { get; set; }
    }
}
