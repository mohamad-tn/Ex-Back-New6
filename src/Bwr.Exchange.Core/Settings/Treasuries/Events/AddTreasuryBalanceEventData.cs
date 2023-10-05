using Bwr.Exchange.Shared.Balances;

namespace Bwr.Exchange.Settings.Treasurys.Events
{
    public class AddTreasuryBalanceEventData : BalanceEventData
    {
        public AddTreasuryBalanceEventData(int currencyId, int treasuryId) : base(currencyId)
        {
            TreasuryId = treasuryId;
        }

        public int TreasuryId { get; set; }
    }
}
