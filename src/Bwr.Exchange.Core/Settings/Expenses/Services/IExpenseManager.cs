using Abp.Domain.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bwr.Exchange.Settings.Expenses.Services
{
    public interface IExpenseManager : IDomainService
    {
        Task<IList<Expense>> GetAllAsync();
        Task<Expense> GetByIdAsync(int id);
        IList<Expense> GetAll();
        Task<IList<Expense>> GetAllForCurrentBranchAsync(int branchId);
        Task<Expense> InsertAndGetAsync(Expense expense);
        Task<Expense> UpdateAndGetAsync(Expense expense);
        Task DeleteAsync(int id);
        bool CheckIfNameAlreadyExist(string name,int branchId);
        bool CheckIfNameAlreadyExist(int id, string name, int branchId);
    }
}
