using Abp.Events.Bus;

namespace Bwr.Exchange.Shared.Balances
{
    public class BalanceEventData : EventData
    {
        public BalanceEventData(int currencyId)
        {
            CurrencyId = currencyId;
        }

        public int CurrencyId { get; set; }
    }
}
