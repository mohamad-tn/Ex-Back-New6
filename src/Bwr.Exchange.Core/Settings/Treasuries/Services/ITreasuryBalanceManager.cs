using Abp.Domain.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bwr.Exchange.Settings.Treasuries.Services
{
    public interface ITreasuryBalanceManager: IDomainService
    {
        IList<TreasuryBalance> GetAllWithDetails();
        Task<TreasuryBalance> GetByIdAsync(int id);
        Task<TreasuryBalance> GetAsync(int currencyId,int treasuryId);
        TreasuryBalance GetByTreasuryIdAndCurrency(int treasuryId, int currencyId);
        Task<TreasuryBalance> InsertAndGetAsync(TreasuryBalance treasuryBalance);
        Task<TreasuryBalance> UpdateAndGetAsync(TreasuryBalance treasuryBalance);
    }
}
