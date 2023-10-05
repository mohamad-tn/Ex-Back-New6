using Abp.Domain.Services;
using Bwr.Exchange.Settings.Expenses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bwr.Exchange.Settings.Incomes.Services
{
    public interface IIncomeManager : IDomainService
    {
        Task<IList<Income>> GetAllAsync();
        Task<Income> GetByIdAsync(int id);
        IList<Income> GetAll();
        Task<IList<Income>> GetAllForCurrentBranchAsync(int branchId);
        Task<Income> InsertAndGetAsync(Income income);
        Task<Income> UpdateAndGetAsync(Income income);
        Task DeleteAsync(int id);
        bool CheckIfNameAlreadyExist(string name, int branchId);
        bool CheckIfNameAlreadyExist(int id, string name, int branchId);
    }
}
