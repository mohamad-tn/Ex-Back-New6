using Bwr.Exchange.Settings.Currencies.Dto;

namespace Bwr.Exchange.CashFlows.Shared.Dto
{
    public class SummaryCashFlowDto
    {
        public CurrencyDto Currency { get; set; }
        public double TotalBalance { get; set; }
    }
}
