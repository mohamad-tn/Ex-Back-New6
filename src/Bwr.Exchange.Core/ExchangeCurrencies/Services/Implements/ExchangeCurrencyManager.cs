using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Events.Bus;
using Abp.Threading;
using Bwr.Exchange.CashFlows.ClientCashFlows.Events;
using Bwr.Exchange.CashFlows.ClientCashFlows.Services;
using Bwr.Exchange.CashFlows.CompanyCashFlows.Events;
using Bwr.Exchange.CashFlows.CompanyCashFlows.Services;
using Bwr.Exchange.CashFlows.TreasuryCashFlows.Events;
using Bwr.Exchange.CashFlows.TreasuryCashFlows.Events.TreasuryCashFlowDeletedMulti;
using Bwr.Exchange.CashFlows.TreasuryCashFlows.Services;
using Bwr.Exchange.ExchangeCurrencies.Factories;
using Bwr.Exchange.ExchangeCurrencies.Services.Interfaces;
using Bwr.Exchange.Settings.ExchangePrices;
using Bwr.Exchange.Transfers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bwr.Exchange.ExchangeCurrencies.Services.Implements
{
    public class ExchangeCurrencyManager : IExchangeCurrencyManager
    {
        
        private readonly IRepository<ExchangeCurrency> _exchangeCurrencyRepository;
        private readonly IExchangeCurrencyFactory _exchangeCurrencyFactory;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly ICompanyCashFlowManager _companyCashFlowManager;
        private readonly IClientCashFlowManager _clientCashFlowManager;
        private readonly ITreasuryCashFlowManager _treasuryCashFlowManager;

        public ExchangeCurrencyManager(
            IRepository<ExchangeCurrency> exchangeCurrencyRepository, 
            IExchangeCurrencyFactory exchangeCurrencyFactory, 
            IUnitOfWorkManager unitOfWorkManager, 
            ICompanyCashFlowManager companyCashFlowManager, 
            IClientCashFlowManager clientCashFlowManager, 
            ITreasuryCashFlowManager treasuryCashFlowManager)
        {
            _exchangeCurrencyRepository = exchangeCurrencyRepository;
            _exchangeCurrencyFactory = exchangeCurrencyFactory;
            _unitOfWorkManager = unitOfWorkManager;
            _companyCashFlowManager = companyCashFlowManager;
            _clientCashFlowManager = clientCashFlowManager;
            _treasuryCashFlowManager = treasuryCashFlowManager;
        }

        public async Task<ExchangeCurrency> CreateAsync(ExchangeCurrency input)
        {
            //Date and time
            var currentDate = DateTime.Now;
            input.Date = new DateTime(
                input.Date.Year,
                input.Date.Month,
                input.Date.Day,
                currentDate.Hour,
                currentDate.Minute,
                currentDate.Second
                );

            IExchangeCurrencyDomainService service = _exchangeCurrencyFactory.CreateService(input.PaymentType);
            var exchangeCurrency = await service.CreateAsync(input);

            //var history = new ExchangeCurrencyHistory()
            //{
            //    ExchangeCurrencyId = exchangeCurrency.Id
            //};

            //if(exchangeCurrency.FirstCurrencyId != null)
            //{
            //    var firstExchangePrice = await _exchangePriceRepository.GetAsync(exchangeCurrency.FirstCurrencyId.Value);
            //    history.FirstMainPrice = firstExchangePrice.MainPrice;
            //    history.FirstSellingPrice = firstExchangePrice.SellingPrice;
            //    history.FirstPurchasingPrice = firstExchangePrice.PurchasingPrice;
            //}

            //if (exchangeCurrency.SecondCurrencyId != null)
            //{
            //    var secondExchangePrice = await _exchangePriceRepository.GetAsync(exchangeCurrency.SecondCurrencyId.Value);
            //    history.SecondMainPrice = secondExchangePrice.MainPrice;
            //    history.SecondSellingPrice = secondExchangePrice.SellingPrice;
            //    history.SecondPurchasingPrice = secondExchangePrice.PurchasingPrice;
            //}

            //await _exchangeCurrencyHistoryRepository.InsertAsync(history);

            return exchangeCurrency;
        }

        public async Task<ExchangeCurrency> GetByIdAsync(int id)
        {
            var exchangeCurrency = await _exchangeCurrencyRepository.GetAsync(id);
            await _exchangeCurrencyRepository.EnsurePropertyLoadedAsync(exchangeCurrency, x => x.Client);
            await _exchangeCurrencyRepository.EnsurePropertyLoadedAsync(exchangeCurrency, x => x.Company);
            await _exchangeCurrencyRepository.EnsurePropertyLoadedAsync(exchangeCurrency, x => x.FirstCurrency);
            await _exchangeCurrencyRepository.EnsurePropertyLoadedAsync(exchangeCurrency, x => x.SecondCurrency);
            await _exchangeCurrencyRepository.EnsurePropertyLoadedAsync(exchangeCurrency, x => x.MainCurrency);

            return exchangeCurrency;
        }

        public int GetLastNumber()
        {
            var last = _exchangeCurrencyRepository.GetAll().OrderByDescending(x => x.Number).FirstOrDefault();
            return last == null ? 0 : last.Number;
        }

        public async Task<ExchangeCurrency> UpdateAsync(ExchangeCurrency input)
        {
            IExchangeCurrencyDomainService service = _exchangeCurrencyFactory.CreateService(input.PaymentType);
            var exchangeCurrency = await service.UpdateAsync(input);
            return exchangeCurrency;
        }

        public IList<ExchangeCurrency> Get(Dictionary<string, object> dic)
        {
            var fromDate = DateTime.Parse(dic["FromDate"].ToString());
            var toDate = DateTime.Parse(dic["ToDate"].ToString());
            IQueryable<ExchangeCurrency> exchangeCurrencies = _exchangeCurrencyRepository.GetAllIncluding(
                co => co.Company,
                cl => cl.Client,
                f => f.FirstCurrency,
                s => s.SecondCurrency
                ).Where(x => x.Date >= fromDate && x.Date <= toDate);

            if (exchangeCurrencies != null && exchangeCurrencies.Any())
            {
                if (dic["CurrencyId"] != null)
                {
                    var currencyId = int.Parse(dic["CurrencyId"].ToString());
                    exchangeCurrencies = exchangeCurrencies
                        .Where(x => x.FirstCurrencyId == currencyId || x.SecondCurrencyId == currencyId);
                }

                if (dic["ActionType"] != null)
                {
                    exchangeCurrencies = exchangeCurrencies
                        .Where(x => x.ActionType == (ActionType)int.Parse(dic["ActionType"].ToString()));
                }

                if (dic["PaymentType"] != null)
                {
                    exchangeCurrencies = exchangeCurrencies
                        .Where(x => x.PaymentType == (PaymentType)int.Parse(dic["PaymentType"].ToString()));
                }

            }

            return exchangeCurrencies.ToList();
        }

        public async Task DeleteAsync(ExchangeCurrency input)
        {
            using (var unitOfWork = _unitOfWorkManager.Begin())
            {
                await _exchangeCurrencyRepository.DeleteAsync(input);
                var cashFlowDeleted = await DeleteCashFlowAsync(input);
                if (cashFlowDeleted)
                {
                    await _exchangeCurrencyRepository.DeleteAsync(input);
                }
                _unitOfWorkManager.Current.SaveChanges();

                unitOfWork.Complete();
            }
        }

        public async Task<bool> DeleteCashFlowAsync(ExchangeCurrency input)
        {
            try
            {
                using (var unitOfWork = _unitOfWorkManager.Begin())
                {
                    await _companyCashFlowManager.DeleteAsync(new CompanyCashFlowDeletedMultiEventData(input.Id, CashFlows.TransactionType.Exchange, string.Empty));
                    await _clientCashFlowManager.DeleteAsync(new ClientCashFlowDeletedMultiEventData(input.Id, CashFlows.TransactionType.Exchange, string.Empty));
                    await _treasuryCashFlowManager.DeleteAsync(new TreasuryCashFlowDeletedMultiEventData(input.Id, CashFlows.TransactionType.Exchange, input.FirstCurrencyId, string.Empty));
                    await _treasuryCashFlowManager.DeleteAsync(new TreasuryCashFlowDeletedMultiEventData(input.Id, CashFlows.TransactionType.Exchange, input.SecondCurrencyId, string.Empty));

                    await _unitOfWorkManager.Current.SaveChangesAsync();
                    await unitOfWork.CompleteAsync();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
        
    }
}
