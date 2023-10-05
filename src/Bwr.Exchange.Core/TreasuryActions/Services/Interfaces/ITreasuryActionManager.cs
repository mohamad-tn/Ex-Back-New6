using Abp.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bwr.Exchange.TreasuryActions.Services
{
    public interface ITreasuryActionManager : IDomainService
    {
        Task<TreasuryAction> CreateAsync(TreasuryAction input);
        Task<TreasuryAction> UpdateAsync(TreasuryAction treasuryAction);
        Task DeleteAsync(TreasuryAction input);
        Task<bool> DeleteCashFlowAsync(TreasuryAction input);
        Task<TreasuryAction> GetByIdAsync(int id);
        Task<IList<TreasuryAction>> GetAsync(Dictionary<string, object> dic);
        IList<TreasuryAction> GetForStatment(Dictionary<string, object> dic);
        IList<TreasuryAction> Get(Dictionary<string, object> dic);
        IList<TreasuryAction> GetByIncomeDetailsIds(List<int> ids);
        int GetLastNumber();
    }
}
