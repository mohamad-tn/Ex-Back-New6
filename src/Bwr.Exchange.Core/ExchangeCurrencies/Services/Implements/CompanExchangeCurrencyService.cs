using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Events.Bus;
using Bwr.Exchange.CashFlows.CompanyCashFlows.Events;
using Bwr.Exchange.ExchangeCurrencies.Services.Interfaces;
using Bwr.Exchange.Settings.ExchangePrices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bwr.Exchange.ExchangeCurrencies.Services.Implements
{
    public class CompanExchangeCurrencyService : IExchangeCurrencyDomainService
    {
        private readonly IRepository<ExchangeCurrency> _exchangeCurrencyRepository;
        private readonly IRepository<ExchangePrice> _exchangePriceRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public CompanExchangeCurrencyService(IRepository<ExchangeCurrency> exchangeCurrencyRepository, IRepository<ExchangePrice> exchangePriceRepository, IUnitOfWorkManager unitOfWorkManager)
        {
            _exchangeCurrencyRepository = exchangeCurrencyRepository;
            _exchangePriceRepository = exchangePriceRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public async Task<ExchangeCurrency> CreateAsync(ExchangeCurrency exchangeCurrency)
        {
            ExchangeCurrency result;
            using (var unitOfWork = _unitOfWorkManager.Begin())
            {
                //History
                if (exchangeCurrency.FirstCurrencyId != null)
                {
                    var firstExchangePrice = await _exchangePriceRepository
                        .FirstOrDefaultAsync(x => x.CurrencyId == exchangeCurrency.FirstCurrencyId.Value);
                    //exchangeCurrency.FirstIsMainCurrency = exchangeCurrency.FirstCurrency.IsMainCurrency;
                    exchangeCurrency.FirstMainPrice = firstExchangePrice?.MainPrice;
                    exchangeCurrency.FirstSellingPrice = firstExchangePrice?.SellingPrice;
                    exchangeCurrency.FirstPurchasingPrice = firstExchangePrice?.PurchasingPrice;
                }

                if (exchangeCurrency.SecondCurrencyId != null)
                {
                    var secondExchangePrice = await _exchangePriceRepository
                        .FirstOrDefaultAsync(x => x.CurrencyId == exchangeCurrency.SecondCurrencyId.Value);

                    //exchangeCurrency.SecondIsMainCurrency = exchangeCurrency.SecondCurrency.IsMainCurrency;
                    exchangeCurrency.SecondMainPrice = secondExchangePrice?.MainPrice;
                    exchangeCurrency.SecondSellingPrice = secondExchangePrice?.SellingPrice;
                    exchangeCurrency.SecondPurchasingPrice = secondExchangePrice?.PurchasingPrice;
                }

                var commision = CalculatCommision(exchangeCurrency);
                exchangeCurrency.Commission = (double)commision;

                int id = await _exchangeCurrencyRepository.InsertAndGetIdAsync(exchangeCurrency);

                result = GetByIdWithDetail(id);

                var secondCurrencyName = result.SecondCurrency != null ? result.SecondCurrency.Name : "";
                var firstCurrencyName = result.FirstCurrency != null ? result.FirstCurrency.Name : "";
                
                var type = result.ActionType == ActionType.Sell ? 
                    (TransactionConst.Sell + " " + firstCurrencyName) : (TransactionConst.Buy + " " + firstCurrencyName);

                EventBus.Default.Trigger(
                    new CompanyCashFlowCreatedEventData()
                    {
                        Date = result.Date,
                        CurrencyId = result.FirstCurrencyId,
                        CompanyId = result.CompanyId != null ? result.CompanyId : null,
                        TransactionId = id,
                        TransactionType = CashFlows.TransactionType.Exchange,
                        Amount = result.ActionType == ActionType.Sell ? (result.AmountOfFirstCurrency * -1) : (result.AmountOfFirstCurrency),
                        Type = type,
                        Commission = 0.0,
                        InstrumentNo = null,
                        CompanyCommission = 0.0,
                        Note = "عن مبلغ " + result.AmoutOfSecondCurrency + secondCurrencyName,
                        Beneficiary = "",
                        Sender = "",
                        Destination = ""
                    });

                EventBus.Default.Trigger(
                    new CompanyCashFlowCreatedEventData()
                    {
                        Date = result.Date,
                        CurrencyId = result.SecondCurrencyId,
                        CompanyId = result.CompanyId != null ? result.CompanyId : null,
                        TransactionId = id,
                        TransactionType = CashFlows.TransactionType.Exchange,
                        Amount = result.ActionType == ActionType.Sell ? (result.AmoutOfSecondCurrency) : (result.AmoutOfSecondCurrency * -1),
                        Type = type,
                        Commission = 0.0,
                        InstrumentNo = null,
                        CompanyCommission = 0.0,
                        Note = "عن مبلغ " + result.AmountOfFirstCurrency + firstCurrencyName,
                        Beneficiary = "",
                        Sender = "",
                        Destination = ""
                    });

                //var history = await _exchangeCurrencyHistoryManager.CreateAsync(result);

                unitOfWork.Complete();
            }
            return result;
        }

        public async Task<ExchangeCurrency> UpdateAsync(ExchangeCurrency exchangeCurrency)
        {
            ExchangeCurrency result;
            using (var unitOfWork = _unitOfWorkManager.Begin())
            {
                var commision = CalculatCommision(exchangeCurrency);
                exchangeCurrency.Commission = (double)commision;

                result = await _exchangeCurrencyRepository.UpdateAsync(exchangeCurrency);

                await _exchangeCurrencyRepository.EnsurePropertyLoadedAsync(result, x => x.Client);
                await _exchangeCurrencyRepository.EnsurePropertyLoadedAsync(result, x => x.Company);
                await _exchangeCurrencyRepository.EnsurePropertyLoadedAsync(result, x => x.FirstCurrency);
                await _exchangeCurrencyRepository.EnsurePropertyLoadedAsync(result, x => x.SecondCurrency);
                await _exchangeCurrencyRepository.EnsurePropertyLoadedAsync(result, x => x.MainCurrency);

                var secondCurrencyName = result.SecondCurrency != null ? result.SecondCurrency.Name : "";
                var firstCurrencyName = result.FirstCurrency != null ? result.FirstCurrency.Name : "";

                var type = result.ActionType == ActionType.Sell ?
                    (TransactionConst.Sell + " " + firstCurrencyName) : (TransactionConst.Buy + " " + firstCurrencyName);

                EventBus.Default.Trigger(
                    new CompanyCashFlowCreatedEventData()
                    {
                        Date = result.Date,
                        CurrencyId = result.FirstCurrencyId,
                        CompanyId = result.CompanyId != null ? result.CompanyId : null,
                        TransactionId = result.Id,
                        TransactionType = CashFlows.TransactionType.Exchange,
                        Amount = result.ActionType == ActionType.Sell ? (result.AmountOfFirstCurrency * -1) : (result.AmountOfFirstCurrency),
                        Type = type,
                        Commission = 0.0,
                        InstrumentNo = null,
                        CompanyCommission = 0.0,
                        Note = "عن مبلغ " + result.AmoutOfSecondCurrency + secondCurrencyName,
                        Beneficiary = "",
                        Sender = "",
                        Destination = ""
                    });

                EventBus.Default.Trigger(
                    new CompanyCashFlowCreatedEventData()
                    {
                        Date = result.Date,
                        CurrencyId = result.SecondCurrencyId,
                        CompanyId = result.CompanyId != null ? result.CompanyId : null,
                        TransactionId = result.Id,
                        TransactionType = CashFlows.TransactionType.Exchange,
                        Amount = result.ActionType == ActionType.Sell ? (result.AmoutOfSecondCurrency) : (result.AmoutOfSecondCurrency * -1),
                        Type = type,
                        Commission = 0.0,
                        InstrumentNo = null,
                        CompanyCommission = 0.0,
                        Note = "عن مبلغ " + result.AmountOfFirstCurrency + firstCurrencyName,
                        Beneficiary = "",
                        Sender = "",
                        Destination = ""
                    });

                //var history = await _exchangeCurrencyHistoryManager.CreateAsync(result);

                unitOfWork.Complete();
            }
            return result;
        }

        private ExchangeCurrency GetByIdWithDetail(int id)
        {
            return _exchangeCurrencyRepository.GetAllIncluding(
                f => f.FirstCurrency,
                s => s.SecondCurrency,
                m => m.MainCurrency,
                cl => cl.Client,
                co => co.Company).FirstOrDefault(x => x.Id == id);
        }

        public decimal CalculatCommision(ExchangeCurrency exchangeCurrency)
        {
            decimal commision = 0;
            var amountOfFirstCurrency = Convert.ToDecimal(exchangeCurrency.AmountOfFirstCurrency);
            if (exchangeCurrency.ActionType == ActionType.Sell)
            {
                if (exchangeCurrency.SecondSellingPrice != null && exchangeCurrency.SecondMainPrice != null)
                {

                    commision = amountOfFirstCurrency * (exchangeCurrency.SecondSellingPrice.Value - exchangeCurrency.SecondMainPrice.Value);
                    commision = commision > 0 ? Math.Round(commision, 1) : 0;//لتلافي إدخال قيم خاطئة في سعر الصرف
                }
            }
            else
            {
                if (exchangeCurrency.SecondPurchasingPrice != null && exchangeCurrency.SecondMainPrice != null)
                {
                    commision = amountOfFirstCurrency * (exchangeCurrency.SecondMainPrice.Value - exchangeCurrency.SecondPurchasingPrice.Value);
                    commision = commision > 0 ? Math.Round(commision, 1) : 0;
                    commision = (-1) * commision;
                }
            }

            return commision;
        }
    }
}
