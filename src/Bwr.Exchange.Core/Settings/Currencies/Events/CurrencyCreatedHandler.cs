using Abp.Dependency;
using Abp.Events.Bus.Handlers;
using Abp.Threading;
using Bwr.Exchange.Settings.Treasuries;
using Bwr.Exchange.Settings.Treasuries.Services;
using System.Threading.Tasks;

namespace Bwr.Exchange.Settings.Currencies.Events
{
    public class CurrencyCreatedHandler : IAsyncEventHandler<CurrencyCreatedData>, ITransientDependency
    {
        private readonly ITreasuryBalanceManager _treasuryBalanceManager;
        private readonly ITreasuryManager _treasuryManager;

        public CurrencyCreatedHandler(ITreasuryBalanceManager treasuryBalanceManager, ITreasuryManager treasuryManager)
        {
            _treasuryBalanceManager = treasuryBalanceManager;
            _treasuryManager = treasuryManager;
        }

        public async Task HandleEventAsync(CurrencyCreatedData eventData)
        {
            var treasury =await  _treasuryManager.GetTreasuryAsync();
            if (treasury != null)
            {
                var treasuryBalance = new TreasuryBalance(0, eventData.CurrencyId, treasury.Id);
                await _treasuryBalanceManager.InsertAndGetAsync(treasuryBalance);
            }
        }
    }
}
