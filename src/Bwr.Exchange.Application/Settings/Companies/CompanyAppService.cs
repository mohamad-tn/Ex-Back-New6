using Abp.Threading;
using Abp.UI;
using Bwr.Exchange.CashFlows.CompanyCashFlows.Services;
using Bwr.Exchange.Settings.Companies.Dto;
using Bwr.Exchange.Settings.Companies.Dto.CompanyBalance;
using Bwr.Exchange.Settings.Companies.Dto.CompanyBalances;
using Bwr.Exchange.Settings.Companies.Services;
using Bwr.Exchange.Settings.Currencies.Services;
using Bwr.Exchange.Shared.DataManagerRequests;
using Bwr.Exchange.Shared.Dto;
using Microsoft.AspNetCore.Mvc;
using Syncfusion.EJ2.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bwr.Exchange.Settings.Companies
{
    public class CompanyAppService : ExchangeAppServiceBase, ICompanyAppService
    {
        private readonly CompanyManager _companyManager;
        private readonly CurrencyManager _currencyManager;
        private readonly CompanyCashFlowManager _companyCashFlowManager;
        public CompanyAppService(
            CompanyManager companyManager,
            CurrencyManager currencyManager,
            CompanyCashFlowManager companyCashFlowManager
            )
        {
            _companyManager = companyManager;
            _currencyManager = currencyManager;
            _companyCashFlowManager = companyCashFlowManager;
        }

        public async Task<IList<CompanyDto>> GetAllAsync()
        {
            IList<Company> companies = new List<Company>();
            var user = AsyncHelper.RunSync(GetCurrentUserAsync);
            if (user.BranchId != null)
            {
                companies = await _companyManager.GetAllForCurrentBranchAsync((int)user.BranchId);
            }
            else
            {
                companies = await _companyManager.GetAllAsync();
            }

            return ObjectMapper.Map<List<CompanyDto>>(companies);
        }

        public async Task<CompanyBalanceDto> GetBalanceForEdit(CompanyBalanceForEditInputDto input)
        {
            var outgoingTransfer = await _companyCashFlowManager.GetByTransctionInfo(input.Id, input.TransactionType);
            return new CompanyBalanceDto()
            {
                Balance = outgoingTransfer.CurrentBalance,
                CompanyId = outgoingTransfer.CompanyId,
                CurrencyId = outgoingTransfer.CurrencyId
            };
        }

        [HttpPost]
        public ReadGrudDto GetForGrid([FromBody] BWireDataManagerRequest dm)
        {
            IList<Company> data = new List<Company>();
            var user = UserManager.GetUserById(dm.userId);
            if (user.BranchId == null)
            {
                data = _companyManager.GetAll();
            }
            else
            {
                data = _companyManager.GetAll().Where(x=>x.BranchId == user.BranchId).ToList();
            }

            
            IEnumerable<ReadCompanyDto> companies = ObjectMapper.Map<List<ReadCompanyDto>>(data);

            var operations = new DataOperations();
            if (dm.Where != null && dm.Where.Count > 0)
            {
                companies = operations.PerformFiltering(companies, dm.Where, "and");
            }

            if (dm.Sorted != null && dm.Sorted.Count > 0)
            {
                companies = operations.PerformSorting(companies, dm.Sorted);
            }

            IEnumerable groupDs = new List<ReadCompanyDto>();
            if (dm.Group != null)
            {
                groupDs = operations.PerformSelect(companies, dm.Group);
            }

            var count = companies.Count();

            if (dm.Skip != 0)
            {
                companies = operations.PerformSkip(companies, dm.Skip);
            }

            if (dm.Take != 0)
            {
                companies = operations.PerformTake(companies, dm.Take);
            }

            return new ReadGrudDto() { result = companies, count = count, groupDs = groupDs };
        }

        public CompanyBalanceDto GetCurrentBalance(CompanyBalanceInputDto input)
        {
            CompanyBalanceDto companyBalanceDto = new CompanyBalanceDto()
            {
                CompanyId = input.CompanyId,
                CurrencyId = input.CurrencyId
            };
            companyBalanceDto.Balance = _companyCashFlowManager.GetLastBalance(input.CompanyId, input.CurrencyId, DateTime.Now);

            return companyBalanceDto;
        }

        public async Task<IList<CompanyBalanceWithCurrencyDto>> GetAllBalances(int companyId)
        {
            var balances = new List<CompanyBalanceWithCurrencyDto>();
            var currencies = await _currencyManager.GetAllAsync();
            foreach (var currency in currencies)
            {
                var input = new CompanyBalanceInputDto()
                {
                    CompanyId = companyId,
                    CurrencyId = currency.Id
                };

                var balanceDto = GetCurrentBalance(input);
                if(balanceDto != null)
                {
                    balances.Add(new CompanyBalanceWithCurrencyDto(balanceDto.Balance, companyId, currency.Id, currency.Name));
                }
            }

            return balances;
        }

        public UpdateCompanyDto GetForEdit(int id)
        {
            var company =  _companyManager.GetByIdWithDetail(id);
            return ObjectMapper.Map<UpdateCompanyDto>(company);
        }
        public async Task<CompanyDto> CreateAsync(CreateCompanyDto input)
        {
            var currentBranch = await GetCurrentBranch();
            input.BranchId = currentBranch.Id;
            CheckBeforeCreate(input);

            var company = ObjectMapper.Map<Company>(input);

            var createdCompany = await _companyManager.InsertAndGetAsync(company);

            return ObjectMapper.Map<CompanyDto>(createdCompany);
        }
        public async Task<CompanyDto> UpdateAsync(UpdateCompanyDto input)
        {
            await GetCurrentBranch();
            var company = await _companyManager.GetByIdAsync(input.Id);

            CheckBeforeUpdate(input, company.BranchId);

            ObjectMapper.Map<UpdateCompanyDto, Company>(input, company);

            var updatedCompany = await _companyManager.UpdateAndGetAsync(company);

            return ObjectMapper.Map<CompanyDto>(updatedCompany);
        }
        public async Task DeleteAsync(int id)
        {
            await _companyManager.DeleteAsync(id);
        }

        public async Task<List<CompanyDto>> GatAllByBranchId(int branchId)
        {
            return ObjectMapper.Map<List<CompanyDto>>(await _companyManager.GetAllForCurrentBranchAsync(branchId));
        }

        #region Helper methods
        private void CheckBeforeCreate(CreateCompanyDto input)
        {
            var validationResultMessage = string.Empty;

            if (_companyManager.CheckIfNameAlreadyExist(input.Name ,input.BranchId))
            {
                validationResultMessage = L(ValidationResultMessage.NameAleadyExist);
            }

            if (!string.IsNullOrEmpty(validationResultMessage))
                throw new UserFriendlyException(validationResultMessage);
        }
        private void CheckBeforeUpdate(UpdateCompanyDto input, int branchId)
        {
            var validationResultMessage = string.Empty;

            if (_companyManager.CheckIfNameAlreadyExist(input.Id, input.Name, branchId))
            {
                validationResultMessage = L(ValidationResultMessage.NameAleadyExist);
            }

            if (!string.IsNullOrEmpty(validationResultMessage))
                throw new UserFriendlyException(validationResultMessage);
        }

        #endregion
    }
}
