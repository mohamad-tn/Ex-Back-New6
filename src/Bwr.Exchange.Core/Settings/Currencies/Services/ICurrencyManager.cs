using Abp.Domain.Services;
using Bwr.Exchange.Settings.Clients;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bwr.Exchange.Settings.Currencies.Services
{
    public interface ICurrencyManager : IDomainService
    {
        Task<IList<Currency>> GetAllAsync();
        Task<Currency> GetByIdAsync(int id);
        IList<Currency> GetAll();
        Task<IList<Currency>> GetAllForCurrentBranchAsync(int branchId);
        Task<Currency> InsertAndGetAsync(Currency country, int treasuryId, int branchId);
        Task<Currency> UpdateAndGetAsync(Currency country);
        Task DeleteAsync(int id);
        bool CheckIfNameAlreadyExist(string name, int branchId);
        bool CheckIfNameAlreadyExist(int id, string name, int branchId);
    }
}
