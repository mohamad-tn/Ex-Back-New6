using Abp.Dependency;
using Abp.Events.Bus.Handlers;
using Bwr.Exchange.Settings.Treasuries.Services;
using System.Threading.Tasks;

namespace Bwr.Exchange.Settings.Treasuries.Events
{
    public class AddTreasuryBalanceForBranchEventHandler : IAsyncEventHandler<AddTreasuryBalanceForBranchEventData>, ITransientDependency
    {
        private readonly ITreasuryBalanceManager _treasuryBalanceManager;

        public AddTreasuryBalanceForBranchEventHandler(
            ITreasuryBalanceManager treasuryBalanceManager)
        {
            _treasuryBalanceManager = treasuryBalanceManager;
        }

        public async Task HandleEventAsync(AddTreasuryBalanceForBranchEventData eventData)
        {
            var treasuryBalance = new TreasuryBalance(0, eventData.CurrencyId, eventData.TreasuryId);

            await _treasuryBalanceManager.InsertAndGetAsync(treasuryBalance);

        }
    }
}
