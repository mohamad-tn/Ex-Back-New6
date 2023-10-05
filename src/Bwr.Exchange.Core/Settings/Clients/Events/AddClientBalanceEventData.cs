using Bwr.Exchange.Shared.Balances;

namespace Bwr.Exchange.Settings.Clients.Events
{
    public class AddClientBalanceEventData : BalanceEventData
    {
        public AddClientBalanceEventData(int currencyId, int branchId) : base(currencyId)
        {
            BranchId = branchId;
        }
        public int BranchId { get; set; }

}
}
