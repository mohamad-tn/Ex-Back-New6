using System;

namespace Bwr.Exchange.CashFlows.ClientCashFlows.Dto
{
    public class GetClientCashFlowInput
    {
        public int ClientId { get; set; }
        public int CurrencyId { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
    }
}
