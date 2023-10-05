using Abp.Domain.Repositories;
using Bwr.Exchange.Settings.Branches;
using Bwr.Exchange.Settings.Clients;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bwr.Exchange.Settings.Expenses.Services
{
    public class ExpenseManager : IExpenseManager
    {
        private readonly IRepository<Expense> _expenseRepository;
        public ExpenseManager(IRepository<Expense> expenseRepository)
        {
            _expenseRepository = expenseRepository;
        }

        public bool CheckIfNameAlreadyExist(string name,int branchId)
        {
            var expense = _expenseRepository.FirstOrDefault(x => x.Name.Trim().Equals(name.Trim()) && x.BranchId == branchId);
            return expense != null ? true : false;
        }

        public bool CheckIfNameAlreadyExist(int id, string name, int branchId)
        {
            var expense = _expenseRepository.FirstOrDefault(x => x.Id != id && x.Name.Trim().Equals(name.Trim()) && x.BranchId == branchId);
            return expense != null ? true : false;
        }

        public async Task DeleteAsync(int id)
        {
            var expense = await GetByIdAsync(id);
            if (expense != null)
                await _expenseRepository.DeleteAsync(expense);
        }

        public IList<Expense> GetAll()
        {
            return _expenseRepository.GetAll().ToList();
        }

        public async Task<IList<Expense>> GetAllAsync()
        {
            return await _expenseRepository.GetAllListAsync();
        }

        public async Task<IList<Expense>> GetAllForCurrentBranchAsync(int branchId)
        {
            var expenses = await _expenseRepository.GetAllListAsync();
            return expenses.Where(x => x.BranchId == branchId).ToList();
        }

        public async Task<Expense> GetByIdAsync(int id)
        {
            return await _expenseRepository.GetAsync(id);
        }

        public async Task<Expense> InsertAndGetAsync(Expense expense)
        {
            return await _expenseRepository.InsertAsync(expense);
        }

        public async Task<Expense> UpdateAndGetAsync(Expense expense)
        {
            return await _expenseRepository.UpdateAsync(expense);
        }


    }
}
