using Abp.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bwr.Exchange.Settings.Treasuries.Services
{
    public class TreasuryBalanceManager : ITreasuryBalanceManager
    {
        private readonly IRepository<TreasuryBalance> _treasuryBalanceRepository;
        public TreasuryBalanceManager(IRepository<TreasuryBalance> treasuryBalanceRepository)
        {
            _treasuryBalanceRepository = treasuryBalanceRepository;
        }

        public async Task<TreasuryBalance> GetAsync(int currencyId, int treasuryId)
        {
            return await _treasuryBalanceRepository.FirstOrDefaultAsync(x => x.TreasuryId == treasuryId && x.CurrencyId == currencyId);
        }

        public IList<TreasuryBalance> GetAllWithDetails()
        {
            return _treasuryBalanceRepository.GetAllIncluding(
                cu => cu.Currency,
                t => t.Treasury).ToList();
        }

        public async Task<TreasuryBalance> GetByIdAsync(int id)
        {
            return await _treasuryBalanceRepository.FirstOrDefaultAsync(id);
        }

        public TreasuryBalance GetByTreasuryIdAndCurrency(int treasuryId, int currencyId)
        {
            return GetAllWithDetails().FirstOrDefault(x => x.TreasuryId == treasuryId && x.CurrencyId == currencyId);
        }

        public async Task<TreasuryBalance> InsertAndGetAsync(TreasuryBalance treasuryBalance)
        {
            
            var id = await _treasuryBalanceRepository.InsertAndGetIdAsync(treasuryBalance);
            return await _treasuryBalanceRepository.FirstOrDefaultAsync(id);
        }

        public async Task<TreasuryBalance> UpdateAndGetAsync(TreasuryBalance treasuryBalance)
        {
            return await _treasuryBalanceRepository.UpdateAsync(treasuryBalance);
        }
    }
}
