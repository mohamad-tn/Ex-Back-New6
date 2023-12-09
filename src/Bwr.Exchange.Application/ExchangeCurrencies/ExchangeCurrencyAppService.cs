using Abp.Events.Bus;
using Abp.Runtime.Session;
using Abp.Threading;
using Abp.UI;
using Bwr.Exchange.CashFlows.ManagementStatement.Events;
using Bwr.Exchange.ExchangeCurrencies.Dto;
using Bwr.Exchange.ExchangeCurrencies.Services.Interfaces;
using Bwr.Exchange.Settings.Clients.Services;
using Bwr.Exchange.Settings.Companies.Services;
using Bwr.Exchange.Settings.Currencies.Services;
using Bwr.Exchange.Shared.DataManagerRequests;
using Bwr.Exchange.Shared.Dto;
using Bwr.Exchange.Transfers;
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
        private readonly ICompanyManager _companyManager;
        private readonly IClientManager _clientManager;
        private readonly ICurrencyManager _currencyManager;
        private readonly IExchangeCurrencyHistoryManager _exchangeCurrencyHistoryManager;

        public ExchangeCurrencyAppService(
            IExchangeCurrencyManager exchangeCurrencyManager,
            IExchangeCurrencyHistoryManager exchangeCurrencyHistoryManager,
            ICompanyManager companyManager,
            IClientManager clientManager,
            ICurrencyManager currencyManager)
        {
            _exchangeCurrencyManager = exchangeCurrencyManager;
            _exchangeCurrencyHistoryManager = exchangeCurrencyHistoryManager;
            _companyManager = companyManager;
            _clientManager = clientManager;
            _currencyManager = currencyManager;
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
            var branch = await GetCurrentBranch();
            var exchangeCurrency = await _exchangeCurrencyManager.GetByIdAsync(input.Id);

            string before = "";
            string after = "";

            #region Before & After
            if (exchangeCurrency.Note != input.Note)
            {
                before = L("Note") + " : " + exchangeCurrency.Note;
                after = L("Note") + " : " + input.Note;
            }

            if (exchangeCurrency.Number != input.Number)
            {
                before = before + " - " + L("Number") + " : " + exchangeCurrency.Number;
                after = after + " - " + L("Number") + " : " + input.Number;
            }

            if (exchangeCurrency.AmountOfFirstCurrency != input.AmountOfFirstCurrency)
            {
                before = before + " - " + L("AmountOfFirstCurrency") + " : " + exchangeCurrency.AmountOfFirstCurrency;
                after = after + " - " + L("AmountOfFirstCurrency") + " : " + input.AmountOfFirstCurrency;
            }

            if (exchangeCurrency.AmoutOfSecondCurrency != input.AmoutOfSecondCurrency)
            {
                before = before + " - " + L("AmoutOfSecondCurrency") + " : " + exchangeCurrency.AmoutOfSecondCurrency;
                after = after + " - " + L("AmoutOfSecondCurrency") + " : " + input.AmoutOfSecondCurrency;
            }

            if (exchangeCurrency.PaidAmountOfFirstCurrency != input.PaidAmountOfFirstCurrency)
            {
                before = before + " - " + L("PaidAmountOfFirstCurrency") + " : " + exchangeCurrency.PaidAmountOfFirstCurrency;
                after = after + " - " + L("PaidAmountOfFirstCurrency") + " : " + input.PaidAmountOfFirstCurrency;
            }

            if (exchangeCurrency.ReceivedAmountOfFirstCurrency != input.ReceivedAmountOfFirstCurrency)
            {
                before = before + " - " + L("ReceivedAmountOfFirstCurrency") + " : " + exchangeCurrency.ReceivedAmountOfFirstCurrency;
                after = after + " - " + L("ReceivedAmountOfFirstCurrency") + " : " + input.ReceivedAmountOfFirstCurrency;
            }

            if (exchangeCurrency.PaidAmountOfSecondCurrency != input.PaidAmountOfSecondCurrency)
            {
                before = before + " - " + L("PaidAmountOfSecondCurrency") + " : " + exchangeCurrency.PaidAmountOfSecondCurrency;
                after = after + " - " + L("PaidAmountOfSecondCurrency") + " : " + input.PaidAmountOfSecondCurrency;
            }

            if (exchangeCurrency.ReceivedAmountOfSecondCurrency != input.ReceivedAmountOfSecondCurrency)
            {
                before = before + " - " + L("ReceivedAmountOfSecondCurrency") + " : " + exchangeCurrency.ReceivedAmountOfSecondCurrency;
                after = after + " - " + L("ReceivedAmountOfSecondCurrency") + " : " + input.ReceivedAmountOfSecondCurrency;
            }

            if (exchangeCurrency.ExchangePrice != input.ExchangePrice)
            {
                before = before + " - " + L("ExchangePrice") + " : " + exchangeCurrency.ExchangePrice;
                after = after + " - " + L("ExchangePrice") + " : " + input.ExchangePrice;
            }

            if ((int)exchangeCurrency.PaymentType != input.PaymentType)
            {
                before = before + " - " + L("PaymentType") + " : " + ((PaymentType)exchangeCurrency.PaymentType);
                after = after + " - " + L("PaymentType") + " : " + ((PaymentType)input.PaymentType);
            }

            if ((int)exchangeCurrency.ActionType != input.ActionType)
            {
                before = before + " - " + L("ActionType") + " : " + ((ActionType)exchangeCurrency.ActionType);
                after = after + " - " + L("ActionType") + " : " + ((ActionType)input.ActionType);
            }

            if (exchangeCurrency.FirstCurrencyId != input.FirstCurrencyId)
            {
                before = before + " - " + L("FirstCurrency") + " : " + (exchangeCurrency.FirstCurrencyId != null ? _currencyManager.GetCurrencyNameById((int)exchangeCurrency.FirstCurrencyId) : " ");
                after = after + " - " + L("FirstCurrency") + " : " + (input.FirstCurrencyId != null ? _currencyManager.GetCurrencyNameById((int)input.FirstCurrencyId) : " ");
            }

            if (exchangeCurrency.SecondCurrencyId != input.SecondCurrencyId)
            {
                before = before + " - " + L("SecondCurrency") + " : " + (exchangeCurrency.SecondCurrencyId != null ? _currencyManager.GetCurrencyNameById((int)exchangeCurrency.SecondCurrencyId) : " ");
                after = after + " - " + L("SecondCurrency") + " : " + (input.SecondCurrencyId != null ? _currencyManager.GetCurrencyNameById((int)input.SecondCurrencyId) : " ");
            }

            if (exchangeCurrency.MainCurrencyId != input.MainCurrencyId)
            {
                before = before + " - " + L("MainCurrency") + " : " + (exchangeCurrency.MainCurrencyId != null ? _currencyManager.GetCurrencyNameById((int)exchangeCurrency.MainCurrencyId) : " ");
                after = after + " - " + L("MainCurrency") + " : " + (input.MainCurrencyId != null ? _currencyManager.GetCurrencyNameById((int)input.MainCurrencyId) : " ");
            }

            if (exchangeCurrency.ClientId != input.ClientId)
            {
                before = before + " - " + L("Client") + " : " + (exchangeCurrency.ClientId != null ? _clientManager.GetClientNameById((int)exchangeCurrency.ClientId) : " ");
                after = after + " - " + L("Client") + " : " + (input.ClientId != null ? _clientManager.GetClientNameById((int)input.ClientId) : " ");
            }

            if (exchangeCurrency.CompanyId != input.CompanyId)
            {
                before = before + " - " + L("Company") + " : " + (exchangeCurrency.CompanyId != null ? _companyManager.GetCompanyNameById((int)exchangeCurrency.CompanyId) : " ");
                after = after + " - " + L("Company") + " : " + (input.CompanyId != null ? _companyManager.GetCompanyNameById((int)input.CompanyId) : " ");
            }
            #endregion

            EventBus.Default.Trigger(
                new CreateManagementEventData(
                    3, null, exchangeCurrency.Date, (int?)exchangeCurrency.PaymentType, DateTime.Now, 0, exchangeCurrency.Number,
                    (int?)exchangeCurrency.ActionType, null, before, after, exchangeCurrency.AmountOfFirstCurrency,
                    exchangeCurrency.AmoutOfSecondCurrency, exchangeCurrency.PaidAmountOfFirstCurrency,
                    exchangeCurrency.ReceivedAmountOfFirstCurrency, exchangeCurrency.PaidAmountOfSecondCurrency,
                    exchangeCurrency.ReceivedAmountOfSecondCurrency, exchangeCurrency.Commission,
                    exchangeCurrency.FirstCurrencyId, exchangeCurrency.SecondCurrencyId, null, exchangeCurrency.ClientId,
                    AbpSession.GetUserId(), exchangeCurrency.CompanyId, null, null, null, (int?)exchangeCurrency.ActionType,branch.Id
                    )
                );


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
            var branch = await GetCurrentBranch();

            var exchangeCurrency = await _exchangeCurrencyManager.GetByIdAsync(id);
            if (exchangeCurrency != null)
            {
                await _exchangeCurrencyManager.DeleteAsync(exchangeCurrency);

                EventBus.Default.Trigger(
                new CreateManagementEventData(
                    3, null, exchangeCurrency.Date, (int?)exchangeCurrency.PaymentType, DateTime.Now, 1, exchangeCurrency.Number,
                    (int?)exchangeCurrency.ActionType, null, null, null, exchangeCurrency.AmountOfFirstCurrency,
                    exchangeCurrency.AmoutOfSecondCurrency, exchangeCurrency.PaidAmountOfFirstCurrency,
                    exchangeCurrency.ReceivedAmountOfFirstCurrency, exchangeCurrency.PaidAmountOfSecondCurrency,
                    exchangeCurrency.ReceivedAmountOfSecondCurrency, exchangeCurrency.Commission,
                    exchangeCurrency.FirstCurrencyId, exchangeCurrency.SecondCurrencyId, null, exchangeCurrency.ClientId,
                    AbpSession.GetUserId(), exchangeCurrency.CompanyId, null, null, null, (int?)exchangeCurrency.ActionType,branch.Id
                    )
                );
            }
        }
    }
}
