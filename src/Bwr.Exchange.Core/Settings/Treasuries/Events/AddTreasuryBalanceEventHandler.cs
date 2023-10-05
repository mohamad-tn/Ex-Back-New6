using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Events.Bus.Handlers;
using Bwr.Exchange.Settings.Treasuries;
using Bwr.Exchange.Settings.Treasuries.Services;
using System.Linq;
using System.Threading.Tasks;

namespace Bwr.Exchange.Settings.Treasurys.Events
{
    public class AddTreasuryBalanceEventHandler : IAsyncEventHandler<AddTreasuryBalanceEventData>, ITransientDependency
    {
        private readonly IRepository<Treasury> _treasuryRepository;
        private readonly ITreasuryBalanceManager _treasuryBalanceManager;

        public AddTreasuryBalanceEventHandler(
            IRepository<Treasury> treasuryRepository,
            ITreasuryBalanceManager treasuryBalanceManager)
        {
            _treasuryRepository = treasuryRepository;
            _treasuryBalanceManager = treasuryBalanceManager;
        }

        public async Task HandleEventAsync(AddTreasuryBalanceEventData eventData)
        {
            var treasuries = await _treasuryRepository.GetAsync(eventData.TreasuryId);

            var treasuryBalance = new TreasuryBalance(0, eventData.CurrencyId, eventData.TreasuryId);

            await _treasuryBalanceManager.InsertAndGetAsync(treasuryBalance);
        }
    }
}
