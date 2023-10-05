using Abp.Domain.Repositories;
using Abp.Events.Bus;
using Bwr.Exchange.Settings.Branches.Events;
using Bwr.Exchange.Settings.Branches;
using Bwr.Exchange.Settings.Currencies;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bwr.Exchange.Settings.Treasurys.Events;
using Bwr.Exchange.Settings.Treasuries.Events;

namespace Bwr.Exchange.Settings.Treasuries.Services
{
    public class TreasuryManager : ITreasuryManager
    {
        private readonly IRepository<Treasury> _treasuryRepository;
        private readonly IRepository<Currency> _currencyRepository;

        public TreasuryManager(
            IRepository<Treasury> treasuryRepository, 
            IRepository<Currency> currencyRepository)
        {
            _treasuryRepository = treasuryRepository;
            //_treasuryBalanceRepository = treasuryBalanceRepository;
            _currencyRepository = currencyRepository;
        }

        public async Task CreateMainTreasuryAsync(Treasury treasury)
        {
            var currencies = await _currencyRepository.GetAllListAsync();

            var mainTreasury = new Treasury();

            mainTreasury.Name = "الصندوق" + "-" + treasury.Name;
            mainTreasury.BranchId = treasury.BranchId;

            var treasuryId = await _treasuryRepository.InsertAndGetIdAsync(mainTreasury);

            foreach (var currency in currencies)
            {
                await EventBus.Default.TriggerAsync(new AddTreasuryBalanceForBranchEventData(currency.Id, treasuryId));
            }
        }

        public async Task<Treasury> GetByBranchIdAsync(int BranchId)
        {
            return await _treasuryRepository.FirstOrDefaultAsync(x=>x.BranchId == BranchId);
        }

        public Treasury GetByBranchId(int BranchId)
        {
            return _treasuryRepository.FirstOrDefault(x => x.BranchId == BranchId);
        }

        public async Task<IList<Treasury>> GetAllAsync()
        {
            return await _treasuryRepository.GetAllListAsync();
        }

        public async Task<Treasury> GetTreasuryAsync()
        {
            var treasuries = await _treasuryRepository.GetAllListAsync();
            return treasuries != null ? treasuries.FirstOrDefault() : null;
        }

        public async Task DeleteAsync(int id)
        {
            var treasury = await _treasuryRepository.GetAsync(id);
            if (treasury != null)
            {
                EventBus.Default.Trigger(new DeleteTreasuryBalanceEventData(0,treasury.Id));
                await _treasuryRepository.DeleteAsync(treasury);
            }
        }

    }
}
