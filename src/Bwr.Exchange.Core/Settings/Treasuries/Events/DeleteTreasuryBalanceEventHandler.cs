using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Events.Bus.Handlers;
using Bwr.Exchange.Settings.Treasuries;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bwr.Exchange.Settings.Treasurys.Events
{
    public class DeleteTreasuryBalanceEventHandler : IAsyncEventHandler<DeleteTreasuryBalanceEventData>, ITransientDependency
    {
        private readonly IRepository<TreasuryBalance> _treasuryBalanceRepository;

        public DeleteTreasuryBalanceEventHandler(IRepository<TreasuryBalance> treasuryBalanceRepository)
        {
            _treasuryBalanceRepository = treasuryBalanceRepository;
        }

        public async Task HandleEventAsync(DeleteTreasuryBalanceEventData eventData)
        {
            var treasuryBalances = new List<TreasuryBalance>();
            if (eventData.TreasuryId == 0)
                treasuryBalances = await _treasuryBalanceRepository.GetAllListAsync(x => x.CurrencyId == eventData.CurrencyId);
            else if (eventData.CurrencyId == 0)
                treasuryBalances = await _treasuryBalanceRepository.GetAllListAsync(x => x.TreasuryId == eventData.TreasuryId);

            if (treasuryBalances.Any())
            {
                foreach (var treasuryBalance in treasuryBalances)
                {
                    await _treasuryBalanceRepository.DeleteAsync(treasuryBalance);
                }
            }
        }
    }
}
