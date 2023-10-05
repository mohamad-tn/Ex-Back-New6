using Bwr.Exchange.Shared.Balances;

namespace Bwr.Exchange.Settings.Treasuries.Events
{
    public class AddTreasuryBalanceForBranchEventData : BalanceEventData
    {
        public AddTreasuryBalanceForBranchEventData(int currencyId, int treasuryId) : base(currencyId)
        {
            TreasuryId = treasuryId;
        }
        public int TreasuryId { get; private set; }
    }
}
