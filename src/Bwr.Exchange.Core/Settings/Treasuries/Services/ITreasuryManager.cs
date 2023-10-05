using Abp.Domain.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bwr.Exchange.Settings.Treasuries.Services
{
    public interface ITreasuryManager : IDomainService
    {
        Task<Treasury> GetTreasuryAsync();
        Task<Treasury> GetByBranchIdAsync(int BranchId);
        Treasury GetByBranchId(int BranchId);
        Task<IList<Treasury>> GetAllAsync();
        Task CreateMainTreasuryAsync(Treasury treasury);
        Task DeleteAsync(int id);
    }
}
