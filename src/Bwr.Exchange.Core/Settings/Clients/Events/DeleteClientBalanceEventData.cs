using Bwr.Exchange.Shared.Balances;

namespace Bwr.Exchange.Settings.Clients.Events
{
    public class DeleteClientBalanceEventData : BalanceEventData
    {
        public DeleteClientBalanceEventData(int currencyId) : base(currencyId)
        {
        }

    }
}
