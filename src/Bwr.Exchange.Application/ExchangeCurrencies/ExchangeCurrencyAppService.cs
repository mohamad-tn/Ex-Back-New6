using Abp.Threading;
using Abp.UI;
using Bwr.Exchange.ExchangeCurrencies.Dto;
using Bwr.Exchange.ExchangeCurrencies.Services.Interfaces;
using Bwr.Exchange.Shared.DataManagerRequests;
using Bwr.Exchange.Shared.Dto;
using Microsoft.AspNetCore.Mvc;
using Syncfusion.EJ2.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bwr.Exchange.ExchangeCurrencies
{
    public class ExchangeCurrencyAppService : ExchangeAppServiceBase, IExchangeCurrencyAppService
    {
        private readonly IExchangeCurrencyManager _exchangeCurrencyManager;
        private readonly IExchangeCurrencyHistoryManager _exchangeCurrencyHistoryManager;

        public ExchangeCurrencyAppService(
            IExchangeCurrencyManager exchangeCurrencyManager, 
            IExchangeCurrencyHistoryManager exchangeCurrencyHistoryManager)
        {
            _exchangeCurrencyManager = exchangeCurrencyManager;
            _exchangeCurrencyHistoryManager = exchangeCurrencyHistoryManager;
        }

        public async Task<CreateExchangeCurrencyDto> CreateAsync(CreateExchangeCurrencyDto input)
        {
            var branch = await GetCurrentBranch();
            var exchangeCurrency = ObjectMapper.Map<ExchangeCurrency>(input);
            exchangeCurrency.BranchId = branch.Id;
            var createdExchangeCurrency = await _exchangeCurrencyManager.CreateAsync(exchangeCurrency);
            return ObjectMapper.Map<CreateExchangeCurrencyDto>(createdExchangeCurrency);
        }

        public async Task<UpdateExchangeCurrencyDto> UpdateAsync(UpdateExchangeCurrencyDto input)
        {
            await GetCurrentBranch();
            var exchangeCurrency = await _exchangeCurrencyManager.GetByIdAsync(input.Id);
            var date = DateTime.Parse(input.Date);
            date = new DateTime
                    (
                        date.Year,
                        date.Month,
                        date.Day,
                        exchangeCurrency.Date.Hour,
                        exchangeCurrency.Date.Minute,
                        exchangeCurrency.Date.Second
                    );
            var cashFlowDeleted = await _exchangeCurrencyManager.DeleteCashFlowAsync(exchangeCurrency);
            if (cashFlowDeleted)
            {
                ObjectMapper.Map<UpdateExchangeCurrencyDto, ExchangeCurrency>(input, exchangeCurrency);
                exchangeCurrency.Date = date;

                var updatedTreasuryAction = await _exchangeCurrencyManager.UpdateAsync(exchangeCurrency);
                return ObjectMapper.Map<UpdateExchangeCurrencyDto>(updatedTreasuryAction);
            }
            else
            {
                throw new UserFriendlyException("Exception Message");
            }
        }

        public async Task<UpdateExchangeCurrencyDto> GetForEditAsync(int exchangeCurrencyId)
        {
            ExchangeCurrency exchangeCurrency = await _exchangeCurrencyManager.GetByIdAsync(exchangeCurrencyId);
            return ObjectMapper.Map<UpdateExchangeCurrencyDto>(exchangeCurrency);
        }

        public int GetLastNumber()
        {
            return _exchangeCurrencyManager.GetLastNumber();
        }

        [HttpPost]
        public ReadGrudDto GetForGrid([FromBody] ExchangeCurrencyDataManagerRequest dm)
        {
            IList<ExchangeCurrency> exchangeCurrencies = new List<ExchangeCurrency>();

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

            var dic = new Dictionary<string, object>()
            {
                { "ActionType" , dm.actionType},
                { "PaymentType" , dm.paymentType},
                { "FromDate" , fromDate},
                { "ToDate" , toDate},
                { "CurrencyId" , dm.currencyId}
            };


            var user = UserManager.GetUserById(dm.userId);
            if (user.BranchId == null)
            {
                exchangeCurrencies = _exchangeCurrencyManager.Get(dic);
            }
            else
            {
                exchangeCurrencies = _exchangeCurrencyManager.Get(dic)
                    .Where(x => x.BranchId == user.BranchId).ToList();
            }


            IEnumerable<ExchangeCurrencyDto> data = ObjectMapper.Map<List<ExchangeCurrencyDto>>(exchangeCurrencies);
            switch (dm.paymentType)
            {
                case 1:
                    {
                        if (dm.clientId != null)
                            data = data.Where(x => x.ClientId == dm.clientId);
                    }
                    break;
                case 2:
                    {
                        if (dm.companyId != null)
                            data = data.Where(x => x.CompanyId == dm.companyId);
                    }
                    break;
                
                default: break;
            }

            var operations = new DataOperations();


            var count = data.Count();

            if (dm.Skip != 0)
            {
                data = operations.PerformSkip(data, dm.Skip);
            }

            if (dm.Take != 0)
            {
                data = operations.PerformTake(data, dm.Take);
            }

            return new ReadGrudDto() { result = data, count = count, groupDs = null };
        }

        public async Task DeleteAsync(int id)
        {
            var exchangeCurrency = await _exchangeCurrencyManager.GetByIdAsync(id);
            if (exchangeCurrency != null)
            {
                await _exchangeCurrencyManager.DeleteAsync(exchangeCurrency);
            }
        }
    }
}
