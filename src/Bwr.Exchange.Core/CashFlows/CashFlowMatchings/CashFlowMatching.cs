using Abp.Domain.Entities.Auditing;

namespace Bwr.Exchange.CashFlows.CashFlowMatchings
{
    public class CashFlowMatching : FullAuditedEntity
    {
        public string MatchingBy{ get; set; }
        public string MatchingWith { get; set; }
        public string Description { get; set; }
    }
}
