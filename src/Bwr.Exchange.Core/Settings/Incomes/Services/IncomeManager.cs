using Abp.Domain.Repositories;
using Bwr.Exchange.Settings.Expenses;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bwr.Exchange.Settings.Incomes.Services
{
    public class IncomeManager : IIncomeManager
    {
        private readonly IRepository<Income> _incomeRepository;
        public IncomeManager(IRepository<Income> incomeRepository)
        {
            _incomeRepository = incomeRepository;
        }

        public bool CheckIfNameAlreadyExist(string name, int branchId)
        {
            var income = _incomeRepository.FirstOrDefault(x => x.Name.Trim().Equals(name.Trim()) && x.BranchId == branchId);
            return income != null ? true : false;
        }

        public bool CheckIfNameAlreadyExist(int id, string name, int branchId)
        {
            var income = _incomeRepository.FirstOrDefault(x => x.Id != id && x.Name.Trim().Equals(name.Trim()) && x.BranchId == branchId);
            return income != null ? true : false;
        }

        public async Task DeleteAsync(int id)
        {
            var income = await GetByIdAsync(id);
            if (income != null)
                await _incomeRepository.DeleteAsync(income);
        }

        public IList<Income> GetAll()
        {
            return _incomeRepository.GetAll().ToList();
        }

        public async Task<IList<Income>> GetAllAsync()
        {
            return await _incomeRepository.GetAllListAsync();
        }

        public async Task<Income> GetByIdAsync(int id)
        {
            return await _incomeRepository.GetAsync(id);
        }

        public async Task<IList<Income>> GetAllForCurrentBranchAsync(int branchId)
        {
            var incomes = await _incomeRepository.GetAllListAsync();
            return incomes.Where(x => x.BranchId == branchId).ToList();
        }

        public async Task<Income> InsertAndGetAsync(Income income)
        {
            return await _incomeRepository.InsertAsync(income);
        }

        public async Task<Income> UpdateAndGetAsync(Income income)
        {
            return await _incomeRepository.UpdateAsync(income);
        }

        public string GetIncomeNameById(int id)
        {
            var income = _incomeRepository.Get(id);
            return income.Name;
        }


    }
}
