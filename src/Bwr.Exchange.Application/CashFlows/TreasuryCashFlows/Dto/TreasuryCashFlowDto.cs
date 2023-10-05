using Bwr.Exchange.CashFlows.Shared.Dto;

namespace Bwr.Exchange.CashFlows.TreasuryCashFlows.Dto
{
    public class TreasuryCashFlowDto : CashFlowDto
    {
        public string Name { get; set; }
        public int TreasuryId { get; set; }
        public double Balance { get; set; }
    }
}
