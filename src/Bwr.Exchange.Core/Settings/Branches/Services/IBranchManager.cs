using Abp.Domain.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bwr.Exchange.Settings.Branches.Services
{
    public interface IBranchManager: IDomainService
    {
        Task<IList<Branch>> GetAllAsync();
        IList<Branch> GetAllWithDetail();
        Branch GetByIdWithDetail(int id);
        Task<Branch> GetByIdAsync(int id);
        IList<Branch> GetAll();
        Task<Branch> InsertAndGetAsync(Branch branch);
        Task<Branch> UpdateAndGetAsync(Branch branch);
        Task DeleteAsync(int id);
        bool CheckIfNameAlreadyExist(string name);
        bool CheckIfNameAlreadyExist(int id, string name);

        //BranchBalance GetBranchBalance(int BranchId, int currencyId);
        //IList<BranchBalance> GetBranchBalances();
    }
}
