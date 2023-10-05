using Abp.Threading;
using Bwr.Exchange.CashFlows.CashFlowMatchings;
using Bwr.Exchange.CashFlows.CompanyCashFlow.Dto;
using Bwr.Exchange.CashFlows.CompanyCashFlows.Dto;
using Bwr.Exchange.CashFlows.CompanyCashFlows.Services;
using Bwr.Exchange.Settings.Companies.Dto;
using Bwr.Exchange.Settings.Companies.Services;
using Bwr.Exchange.Settings.Currencies.Dto;
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
using Bwr.Exchange.Reflection.Extensions;
using Bwr.Exchange.CashFlows.Shared.Dto;
using Bwr.Exchange.Settings.Companies;
using Bwr.Exchange.Settings.Currencies;

namespace Bwr.Exchange.CashFlows.CompanyCashFlows
{
    public class CompanyCashFlowAppService : ExchangeAppServiceBase, ICompanyCashFlowAppService
    {
        private readonly ICompanyCashFlowManager _companyCashFlowManager;
        private readonly ICurrencyManager _currencyManager;
        private readonly ICompanyManager _companyManager;

        public CompanyCashFlowAppService(
            ICompanyCashFlowManager companyCashFlowManager, 
            ICurrencyManager currencyManager, 
            ICompanyManager companyManager)
        {
            _companyCashFlowManager = companyCashFlowManager;
            _currencyManager = currencyManager;
            _companyManager = companyManager;
        }

        public IList<CompanyCashFlowDto> Get(GetCompanyCashFlowInput input)
        {
            var companyCashFlows = _companyCashFlowManager.Get(input.CompanyId);
            var companyCashFlowsDto = ObjectMapper.Map<List<CompanyCashFlowDto>>(companyCashFlows);
            return companyCashFlowsDto;
        }

        public IList<CompanyCashFlowTotalDto> GetCompanysBalances(string date)
        {
            IList<Company> companys = new List<Company>();
            IList<Currency> currencies = new List<Currency>();

            var user = AsyncHelper.RunSync(GetCurrentUserAsync);
            if (user.BranchId != null)
            {
                companys = _companyManager.GetAll().Where(x => x.BranchId == user.BranchId).ToList();
                currencies = _currencyManager.GetAll().Where(x => x.BranchId == user.BranchId).ToList();
            }
            else
            {
                companys = _companyManager.GetAll();
                currencies = _currencyManager.GetAll();
            }

            var toDate = DateTime.Now;
            if (!string.IsNullOrEmpty(date))
            {
                toDate = DateTime.Parse(date);
            }
            toDate = new DateTime(toDate.Year, toDate.Month, toDate.Day, 23, 59, 59);

            var dtos = new List<CompanyCashFlowTotalDto>();

            
            foreach (var company in companys)
            {
                var dto = new CompanyCashFlowTotalDto()
                {
                    CompanyId = company.Id,
                    CompanyName = company.Name
                };

                foreach (var currency in currencies)
                {
                    var balance = _companyCashFlowManager.GetLastBalance(company.Id, currency.Id, toDate);
                    var currencyBalance = new CompanyCashFlowTotalDetailDto()
                    {
                        CurrencyId = currency.Id,
                        CurrencyName = currency.Name,
                        CurrentBalance = balance
                    };

                    //التحقق في حال كانت الشركة نشطة اليوم
                    dto.IsActiveToday = _companyCashFlowManager.CheckIsActiveToday(company.Id, toDate);
                    dto.IsMatching = _companyCashFlowManager.CheckIsIfMatching(company.Id);
                    dto.CurrencyBalances.Add(currencyBalance);
                }

                
                dtos.Add(dto);
            }

            return dtos;
        }

        public IList<SummaryCashFlowDto> Summary(string date)
        {
            var toDate = DateTime.Now;
            if (!string.IsNullOrEmpty(date))
            {
                toDate = DateTime.Parse(date);
            }
            toDate = new DateTime(toDate.Year, toDate.Month, toDate.Day, 23, 59, 59);

            var dtos = new List<SummaryCashFlowDto>();

            IList<Company> companys = new List<Company>();
            IList<Currency> currencies = new List<Currency>();
            var user = AsyncHelper.RunSync(GetCurrentUserAsync);

            if (user.BranchId != null)
            {
                companys = _companyManager.GetAll().Where(x => x.BranchId == user.BranchId).ToList();
                currencies = _currencyManager.GetAll().Where(x => x.BranchId == user.BranchId).ToList();
            }
            else
            {
                companys = _companyManager.GetAll();
                currencies = _currencyManager.GetAll();
            }            

            foreach (var currency in currencies)
            {
                var dto = new SummaryCashFlowDto()
                {
                    Currency = ObjectMapper.Map<CurrencyDto>(currency),
                };
                foreach (var company in companys)
                {
                    var balance = _companyCashFlowManager.GetLastBalance(company.Id, currency.Id, toDate);
                    dto.TotalBalance += balance;
                }

                dtos.Add(dto);
            }

            return dtos;
        }

        public async Task<CurrentCompanyBalanceDto> GetCurrentBalanceAsync(GetCompanyCashFlowInput input)
        {
            var toDate = DateTime.Now;
            if (!string.IsNullOrEmpty(input.ToDate))
            {
                toDate = DateTime.Parse(input.ToDate);
            }
            toDate = new DateTime(toDate.Year, toDate.Month, toDate.Day, 23, 59, 59);

            var balance = _companyCashFlowManager.GetLastBalance(input.CompanyId, input.CurrencyId, toDate);

            var company = await _companyManager.GetByIdAsync(input.CompanyId);
            var companyDto = ObjectMapper.Map<CompanyDto>(company);

            var currency = await _currencyManager.GetByIdAsync(input.CurrencyId);
            var currencyDto = ObjectMapper.Map<CurrencyDto>(currency);

            return new CurrentCompanyBalanceDto(companyDto, currencyDto, balance);
        }

        [HttpPost]
        public ReadGrudDto GetForGrid([FromBody] CashFlowDataManagerRequest dm)
        {
            DateTime fromDate = new DateTime(), toDate = new DateTime();

            if (!string.IsNullOrEmpty(dm.fromDate))
            {
                DateTime.TryParse(dm.fromDate, out fromDate);
                fromDate = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day, 0, 0, 0);
            }

            if (!string.IsNullOrEmpty(dm.toDate))
            {
                DateTime.TryParse(dm.toDate, out toDate);
                toDate = new DateTime(toDate.Year, toDate.Month, toDate.Day, 23, 59, 59);
            }

            var companyCashFlowsDto = new List<CompanyCashFlowDto>();
            if (dm.id != 0 && dm.currencyId != 0)
            {
                //Add Previous Balance
                var previousBalance = _companyCashFlowManager.GetPreviousBalance2(dm.id, dm.currencyId, fromDate);
                companyCashFlowsDto.Add(new CompanyCashFlowDto
                {
                    CompanyId = dm.id,
                    CurrencyId = dm.currencyId,
                    CurrentBalance = previousBalance,
                    Type = TransactionConst.PreviousBalance
                });

                //Get Company Cash Flows
                var companyCashFlows = _companyCashFlowManager.Get(dm.id, dm.currencyId, fromDate, toDate).ToList();
                
                companyCashFlowsDto.AddRange(ObjectMapper.Map<List<CompanyCashFlowDto>>(companyCashFlows));
                InitialBalance(companyCashFlowsDto);
            }


            IEnumerable<CompanyCashFlowDto> data = companyCashFlowsDto;
            AddCreationUsers(data);

            var operations = new DataOperations();

            IEnumerable groupDs = new List<CompanyCashFlowDto>();
            if (dm.Group != null)
            {
                groupDs = operations.PerformSelect(data, dm.Group);
            }

            var count = data.Count();

            if (dm.Skip != 0)
            {
                data = operations.PerformSkip(data, dm.Skip);
            }

            if (dm.Take != 0)
            {
                data = operations.PerformTake(data, dm.Take);
            }

            

            return new ReadGrudDto() { result = data, count = count, groupDs = groupDs };
        }

        private void AddCreationUsers(IEnumerable<CompanyCashFlowDto> data)
        {
            foreach (var item in data)
            {
                if(item.CreatorUserId != null)
                {
                    var user = AsyncHelper.RunSync(() => UserManager.GetUserByIdAsync(item.CreatorUserId.Value));
                    item.CreatorUser = user?.FullName;
                }
            }
        }

        public async Task<CompanyCashFlowMatchingDto> MatchAsync(CompanyCashFlowMatchingDto input)
        {

            var matching = new CashFlowMatching()
            {
                MatchingBy = input.MatchingBy,
                MatchingWith = input.MatchingWith,
                Description = input.Description
            };

            var listDic = new List<Dictionary<string, object>>();
            foreach (var item in input.Items)
            {
                listDic.Add(item.ToDictionary());
            }
            await _companyCashFlowManager.MatchAsync(matching, listDic);

            return input;
        }

        private WhereFilter GetWhereFilter(List<WhereFilter> filterOptions, string name)
        {
            var filter = filterOptions.FirstOrDefault(x => x.Field == name);
            if (filter != null)
                return filter;

            foreach (var option in filterOptions)
            {
                return GetWhereFilter(option.predicates, name);
            }

            return null;
        }

        private void InitialBalance(List<CompanyCashFlowDto> companyCashFlows)
        {
            double value = 0;
            for (int i = 0; i < companyCashFlows.Count; i++)
            {
                value += companyCashFlows[i].CurrentBalance;
                companyCashFlows[i].Balance = value;
            }
        }
    }
}
