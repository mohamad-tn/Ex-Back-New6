using Bwr.Exchange.Settings.Currencies;
using Bwr.Exchange.Settings.Currencies.Dto;
using Bwr.Exchange.Settings.Currencies.Services;
using Bwr.Exchange.Settings.ExchangePrices.Dto;
using Bwr.Exchange.Settings.ExchangePrices.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bwr.Exchange.Settings.ExchangePrices
{
    public class ExchangePriceAppService : ExchangeAppServiceBase, IExchangePriceAppService
    {
        private readonly IExchangePriceManager _exchangeManager;
        private readonly ICurrencyManager _currencyManager;

        public ExchangePriceAppService(
            IExchangePriceManager exchangeManager, 
            ICurrencyManager currencyManager)
        {
            _exchangeManager = exchangeManager;
            _currencyManager = currencyManager;
        }

        public async Task<IList<ExchangePriceDto>> GetAllAsync()
        {
            var exchangePrices = new List<ExchangePriceDto>();
            //var currencies = await _currencyManager.GetAllAsync();

            IList<Currency> currencies = new List<Currency>();
            var user = await GetCurrentUserAsync();
            if (user.BranchId != null)
            {
                currencies = await _currencyManager.GetAllForCurrentBranchAsync((int)user.BranchId);
            }
            else
            {
                currencies = await _currencyManager.GetAllAsync();
            }


            foreach (var currency in currencies)
            {
                var exchangePrice = await _exchangeManager.GetByCurrencyIdAsync(currency.Id);
                if(exchangePrice == null)
                {
                    exchangePrices.Add(new ExchangePriceDto()
                    {
                        Currency = ObjectMapper.Map<CurrencyDto>(currency),
                        CurrencyId = currency.Id
                    });
                }
                else
                {
                    exchangePrices.Add(new ExchangePriceDto()
                    {
                        Currency = ObjectMapper.Map<CurrencyDto>(currency),
                        CurrencyId = currency.Id,
                        MainPrice = exchangePrice.MainPrice,
                        PurchasingPrice = exchangePrice.PurchasingPrice,
                        SellingPrice = exchangePrice.SellingPrice
                    });
                }
            }
            return exchangePrices;
        }

        public async Task<ExchangePriceDto> UpdateAsync(UpdateExchangePriceDto input)
        {
            var exchangePrice = await _exchangeManager.GetByCurrencyIdAsync(input.CurrencyId);
            if (exchangePrice == null)
            {
                exchangePrice = ObjectMapper.Map<ExchangePrice>(input);
                var createdExchangePrice = await _exchangeManager.CreateAsync(exchangePrice);
                return ObjectMapper.Map<ExchangePriceDto>(createdExchangePrice);
            }
            else
            {
                exchangePrice.MainPrice = input.MainPrice;
                exchangePrice.PurchasingPrice = input.PurchasingPrice;
                exchangePrice.SellingPrice = input.SellingPrice;
                var updatedExchangePrice = await _exchangeManager.UpdateAsync(exchangePrice);
                return ObjectMapper.Map<ExchangePriceDto>(updatedExchangePrice);
            }
        }
    }
}
