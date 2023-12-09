using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bwr.Exchange.Settings.Companies.Services
{
    public class CompanyManager : ICompanyManager
    {
        private readonly IRepository<Company> _companyRepository;
        private readonly IRepository<CompanyBalance> _companyBalanceRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        public CompanyManager(
            IRepository<Company> companyRepository,
            IRepository<CompanyBalance> companyBalanceRepository,
            IUnitOfWorkManager unitOfWorkManager
            )
        {
            _companyRepository = companyRepository;
            _companyBalanceRepository = companyBalanceRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public bool CheckIfNameAlreadyExist(string name,int branchId)
        {
            var company = _companyRepository.FirstOrDefault(x => x.Name.Trim().Equals(name.Trim()) && x.BranchId == branchId);
            return company != null ? true : false;
        }

        public bool CheckIfNameAlreadyExist(int id, string name ,int branchId)
        {
            var company = _companyRepository.FirstOrDefault(x => x.Id != id && x.Name.Trim().Equals(name.Trim()) && x.BranchId == branchId);
            return company != null ? true : false;
        }

        public async Task DeleteAsync(int id)
        {
            var company = await GetByIdAsync(id);
            if (company != null)
                await _companyRepository.DeleteAsync(company);
        }

        public IList<Company> GetAll()
        {
            return _companyRepository.GetAll().ToList();
        }

        public async Task<IList<Company>> GetAllForCurrentBranchAsync(int branchId)
        {
            var companies = await _companyRepository.GetAllListAsync();
            return companies.Where(x => x.BranchId == branchId).ToList();
        }

        public async Task<IList<Company>> GetAllAsync()
        {
            return await _companyRepository.GetAllListAsync();
        }

        public IList<Company> GetAllWithDetail()
        {
            var companies = _companyRepository.GetAllIncluding(x => x.CompanyBalances);
            return companies != null ? companies.ToList() : null;
        }

        public Company GetByIdWithDetail(int id)
        {
            var company = GetAllWithDetail().FirstOrDefault(x=>x.Id == id);
            return company;
        }

        public async Task<Company> GetByIdAsync(int id)
        {
            return await _companyRepository.GetAsync(id);
        }

        public async Task<Company> InsertAndGetAsync(Company company)
        {
            using (var unitOfWork = _unitOfWorkManager.Begin())
            {
                var companyId = await _companyRepository.InsertAndGetIdAsync(company);

                //Update companyBalances
                //var companyBalances = company.CompanyBalances.ToList();//Don't remove ToList()
                //await RemoveCompanyBalances(companyId, companyBalances);
                //await AddNewCompanyBalances(companyId, companyBalances);

                unitOfWork.Complete();
            }
            return await GetByIdAsync(company.Id);
        }

        public async Task<Company> UpdateAndGetAsync(Company company)
        {
            using (var unitOfWork = _unitOfWorkManager.Begin())
            {
                var updatedCompany = await _companyRepository.UpdateAsync(company);

                //Update companyBalances
                //var companyBalances = company.CompanyBalances.ToList();//Don't remove ToList()
                //await RemoveCompanyBalances(updatedCompany.Id, companyBalances);
                //await AddNewCompanyBalances(updatedCompany.Id, companyBalances);

                unitOfWork.Complete();
            }
            return await GetByIdAsync(company.Id);
        }

        public CompanyBalance GetCompanyBalance(int companyId, int currencyId)
        {
            CompanyBalance clientBalance = null;
            var client = GetAllWithDetail().FirstOrDefault(x => x.Id == companyId);
            if (client != null)
            {
                clientBalance = client.CompanyBalances.FirstOrDefault(x => x.CurrencyId == currencyId);
            }

            return clientBalance;
        }

        public string GetCompanyNameById(int id)
        {
            var company = _companyRepository.Get(id);
            return company.Name;
        }

        #region Helper Methods
        private async Task RemoveCompanyBalances(int companyId, IList<CompanyBalance> newCompanyBalances)
        {
            var oldCompanyBalances = await _companyBalanceRepository.GetAllListAsync(x => x.CompanyId == companyId);

            foreach (var oldCompanyBalance in oldCompanyBalances)
            {
                var isExist = newCompanyBalances.Any(x => x.CurrencyId == oldCompanyBalance.CurrencyId);
                if (!isExist)
                {
                    await _companyBalanceRepository.DeleteAsync(oldCompanyBalance);
                }
            }
        }

        private async Task AddNewCompanyBalances(int companyId, IList<CompanyBalance> newCompanyBalances)
        {
            var oldCompanyBalances = await _companyBalanceRepository.GetAllListAsync(x => x.CompanyId == companyId);
            foreach (var newCompanyBalance in newCompanyBalances)
            {
                var isExist = oldCompanyBalances.Any(x => x.CurrencyId == newCompanyBalance.CurrencyId);
                if (!isExist)
                {
                    await _companyBalanceRepository.InsertAsync(newCompanyBalance);
                }
            }
        }

        #endregion
    }
}
