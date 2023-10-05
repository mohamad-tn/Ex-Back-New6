using Abp.Threading;
using Abp.UI;
using Bwr.Exchange.CashFlows.Shared.Dto;
using Bwr.Exchange.CashFlows.TreasuryCashFlows.Dto;
using Bwr.Exchange.CashFlows.TreasuryCashFlows.Services;
using Bwr.Exchange.Settings.Companies;
using Bwr.Exchange.Settings.Currencies;
using Bwr.Exchange.Settings.Currencies.Dto;
using Bwr.Exchange.Settings.Currencies.Services;
using Bwr.Exchange.Settings.Treasuries;
using Bwr.Exchange.Settings.Treasuries.Services;
using Bwr.Exchange.Shared.DataManagerRequests;
using Bwr.Exchange.Shared.Dto;
using Microsoft.AspNetCore.Mvc;
using Syncfusion.EJ2.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bwr.Exchange.CashFlows.TreasuryCashFlows
{
    public class TreasuryCashFlowAppService : ExchangeAppServiceBase, ITreasuryCashFlowAppService
    {
        private readonly ITreasuryCashFlowManager _treasuryCashFlowManager;
        private readonly ITreasuryManager _treasuryManager;
        private readonly ICurrencyManager _currencyManager;

        public TreasuryCashFlowAppService(
            ITreasuryCashFlowManager treasuryCashFlowManager, 
            ITreasuryManager treasuryManager, 
            ICurrencyManager currencyManager)
        {
            _treasuryCashFlowManager = treasuryCashFlowManager;
            _treasuryManager = treasuryManager;
            _currencyManager = currencyManager;
        }

        public IList<TreasuryCashFlowDto> Get(GetTreasuryCashFlowInput input)
        {
            var treasury = AsyncHelper.RunSync(_treasuryManager.GetTreasuryAsync);
            var treasuryCashFlows = _treasuryCashFlowManager.Get(input.TreasuryId, input.CurrencyId, input.FromDate, input.ToDate);
            var treasuryCashFlowsDto = ObjectMapper.Map<List<TreasuryCashFlowDto>>(treasuryCashFlows);
            return treasuryCashFlowsDto;
        }

        [HttpPost]
        public ReadGrudDto GetForGrid([FromBody] TreasuryCashFlowDataManagerRequest dm)
        {
            Treasury treasury = new Treasury();
            var user = UserManager.GetUserById(dm.userId);

            if (user.BranchId != null)
            {
                treasury = _treasuryManager.GetByBranchId((int)user.BranchId);
            }else
            {
                throw new UserFriendlyException(L(ValidationResultMessage.CurrentUserIsNotAssociatedWithBranchAndTreasury));
            }

             var fromDate = DateTime.Now;
            var toDate = DateTime.Now;

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

            var treasuryCashFlowsDto = new List<TreasuryCashFlowDto>();
            if (treasury != null && dm.currencyId != 0)
            {
                //Add Previous Balance
                var previousBalance = _treasuryCashFlowManager.GetPreviousBalance2(treasury.Id, dm.currencyId, fromDate);
                treasuryCashFlowsDto.Add(new TreasuryCashFlowDto
                {
                    TreasuryId = treasury.Id,
                    CurrencyId = dm.currencyId,
                    CurrentBalance = previousBalance,
                    Type = TransactionConst.PreviousBalance
                });

                //Get Treasury Cash Flows
                var treasuryCashFlows = _treasuryCashFlowManager.Get(treasury.Id, dm.currencyId, fromDate, toDate);
                treasuryCashFlowsDto.AddRange(ObjectMapper.Map<List<TreasuryCashFlowDto>>(treasuryCashFlows));

                InitialBalance(treasuryCashFlowsDto);
            }

            IEnumerable<TreasuryCashFlowDto> data = treasuryCashFlowsDto;

            var operations = new DataOperations();

            IEnumerable groupDs = new List<TreasuryCashFlowDto>();
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

            return new ReadGrudDto() { result = data, count = 0, groupDs = groupDs };

        }

        private void InitialBalance(List<TreasuryCashFlowDto> treasuryCashFlows)
        {
            double value = 0;
            for(int i = 0; i< treasuryCashFlows.Count; i++)
            {
                value += treasuryCashFlows[i].CurrentBalance;
                treasuryCashFlows[i].Balance = value;
            }
        }

        public async Task<IList<SummaryCashFlowDto>> Summary(string date)
        {
            var toDate = DateTime.Now;
            if (!string.IsNullOrEmpty(date))
            {
                toDate = DateTime.Parse(date);
            }
            toDate = new DateTime(toDate.Year, toDate.Month, toDate.Day, 23, 59, 59);

            var dtos = new List<SummaryCashFlowDto>();

            IList<Treasury> treasuries = new List<Treasury>();
            IList<Currency> currencies = new List<Currency>();

            var user = AsyncHelper.RunSync(GetCurrentUserAsync);

            if (user.BranchId != null)
            {
                var treasuries1 = await _treasuryManager.GetAllAsync();
                treasuries = treasuries1.Where(x => x.BranchId == user.BranchId).ToList();
                currencies = _currencyManager.GetAll().Where(x => x.BranchId == user.BranchId).ToList();
            }
            else
            {
                treasuries = await _treasuryManager.GetAllAsync();
                currencies = _currencyManager.GetAll();
            }


            foreach (var currency in currencies)
            {
                var dto = new SummaryCashFlowDto()
                {
                    Currency = ObjectMapper.Map<CurrencyDto>(currency),
                };
                foreach (var treasury in treasuries)
                {
                    var balance = _treasuryCashFlowManager.GetLastBalance(treasury.Id, currency.Id, toDate);
                    dto.TotalBalance += balance;
                }

                dtos.Add(dto);
            }

            return dtos;
        }

        //public async Task<TreasuryCashFlowMatchingDto> MatchAsync(TreasuryCashFlowMatchingDto input)
        //{
        //    var fromDate = DateTime.Now;
        //    var toDate = DateTime.Now;
        //    if (!string.IsNullOrEmpty(input.FromDate))
        //    {
        //        fromDate = DateTime.Parse(input.FromDate);
        //    }

        //    if (!string.IsNullOrEmpty(input.ToDate))
        //    {
        //        toDate = DateTime.Parse(input.ToDate);
        //    }

        //    var matching = new CashFlowMatching()
        //    {
        //        MatchingBy = input.MatchingBy,
        //        MatchingWith = input.MatchingWith,
        //        Description = input.Description
        //    };

        //    await _treasuryCashFlowManager.MatchAsync(matching, input.TreasuryId, input.CurrencyId, fromDate, toDate);

        //    return input;
        //}

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

    }
}
