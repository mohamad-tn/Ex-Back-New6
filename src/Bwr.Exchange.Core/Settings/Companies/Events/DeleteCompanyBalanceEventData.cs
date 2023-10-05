using Abp.Events.Bus;
using Bwr.Exchange.Shared.Balances;

namespace Bwr.Exchange.Settings.Companys.Events
{
    public class DeleteCompanyBalanceEventData : BalanceEventData
    {
        public DeleteCompanyBalanceEventData(int currencyId) : base(currencyId)
        {
        }

    }
}