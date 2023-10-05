using Abp.Domain.Repositories;
using Bwr.Exchange.ExchangeCurrencies.Services.Interfaces;
using Bwr.Exchange.Settings.ExchangePrices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bwr.Exchange.ExchangeCurrencies.Services.Implements
{
    public class ExchangeCurrencyHistoryManager : IExchangeCurrencyHistoryManager
    {
        private readonly IRepository<ExchangePrice> _exchangePriceRepository;
        private readonly IRepository<ExchangeCurrencyHistory> _exchangeCurrencyHistoryRepository;

        public ExchangeCurrencyHistoryManager(IRepository<ExchangePrice> exchangePriceRepository, IRepository<ExchangeCurrencyHistory> exchangeCurrencyHistoryRepository)
        {
            _exchangePriceRepository = exchangePriceRepository;
            _exchangeCurrencyHistoryRepository = exchangeCurrencyHistoryRepository;
        }

        public async Task<ExchangeCurrencyHistory> CreateAsync(ExchangeCurrency exchangeCurrency)
        {
            var history = new ExchangeCurrencyHistory()
            {
                ExchangeCurrencyId = exchangeCurrency.Id
            };

            if (exchangeCurrency.FirstCurrencyId != null)
            {
                var firstExchangePrice = await _exchangePriceRepository
                    .FirstOrDefaultAsync(x=>x.CurrencyId == exchangeCurrency.FirstCurrencyId.Value);
                history.FirstCurrencyId = exchangeCurrency.FirstCurrencyId;
                history.FirstIsMainCurrency = exchangeCurrency.FirstCurrency.IsMainCurrency;
                history.FirstMainPrice = firstExchangePrice?.MainPrice;
                history.FirstSellingPrice = firstExchangePrice?.SellingPrice;
                history.FirstPurchasingPrice = firstExchangePrice?.PurchasingPrice;
            }

            if (exchangeCurrency.SecondCurrencyId != null)
            {
                var secondExchangePrice = await _exchangePriceRepository
                    .FirstOrDefaultAsync(x => x.CurrencyId == exchangeCurrency.SecondCurrencyId.Value);
                history.SecondCurrencyId = exchangeCurrency.SecondCurrencyId;
                history.SecondIsMainCurrency = exchangeCurrency.SecondCurrency.IsMainCurrency;
                history.SecondMainPrice = secondExchangePrice?.MainPrice;
                history.SecondSellingPrice = secondExchangePrice?.SellingPrice;
                history.SecondPurchasingPrice = secondExchangePrice?.PurchasingPrice;
            }

            var historyId = await _exchangeCurrencyHistoryRepository.InsertAndGetIdAsync(history);

            return await _exchangeCurrencyHistoryRepository.GetAsync(historyId);
        }
        
        public async Task<ExchangeCurrencyHistory> GetAsync(int exchangeCurrencyId)
        {
            return await _exchangeCurrencyHistoryRepository
                .FirstOrDefaultAsync(x => x.ExchangeCurrencyId == exchangeCurrencyId);
        }

        
    }
}
