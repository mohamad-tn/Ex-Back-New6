using System;

namespace Bwr.Exchange.CashFlows.TreasuryCashFlows.Dto
{
    public class GetTreasuryCashFlowInput
    {
        public int TreasuryId { get; set; }
        public int CurrencyId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}
