using Abp.Domain.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bwr.Exchange.Settings.Companies.Services
{
    public interface ICompanyManager : IDomainService
    {
        Task<IList<Company>> GetAllAsync();
        IList<Company> GetAllWithDetail();
        Company GetByIdWithDetail(int id);
        Task<Company> GetByIdAsync(int id);
        IList<Company> GetAll();
        CompanyBalance GetCompanyBalance(int companyId, int currencyId);
        Task<Company> InsertAndGetAsync(Company company);
        Task<Company> UpdateAndGetAsync(Company company);
        Task DeleteAsync(int id);
        bool CheckIfNameAlreadyExist(string name ,int branchId);
        bool CheckIfNameAlreadyExist(int id, string name, int branchId);
        Task<IList<Company>> GetAllForCurrentBranchAsync(int branchId);
    }
}
