using Abp.Domain.Repositories;
using Bwr.Exchange.Settings.Currencies;
using System.Threading.Tasks;

namespace Bwr.Exchange.Settings.ExchangePrices.Services
{
    public class ExchangePriceManager : IExchangePriceManager
    {
        private readonly IRepository<ExchangePrice> _exchangePriceRepository;
        public ExchangePriceManager(IRepository<ExchangePrice> exchangePriceRepository)
        {
            _exchangePriceRepository = exchangePriceRepository;
        }

        public async Task<ExchangePrice> CreateAsync(ExchangePrice exchangePrice)
        {
            var id = await _exchangePriceRepository.InsertAndGetIdAsync(exchangePrice);
            return await _exchangePriceRepository.GetAsync(id);
        }

        public async Task<ExchangePrice> GetByCurrencyIdAsync(int id)
        {
            var exchangePrice = await _exchangePriceRepository.FirstOrDefaultAsync(x => x.CurrencyId == id);
            return exchangePrice;
        }

        public async Task<ExchangePrice> UpdateAsync(ExchangePrice exchangePrice)
        {
            return await _exchangePriceRepository.UpdateAsync(exchangePrice);
        }
    }
}
