using Abp.Domain.Services;
using System.Threading.Tasks;

namespace Bwr.Exchange.Settings.ExchangePrices.Services
{
    public interface IExchangePriceManager : IDomainService
    {
        Task<ExchangePrice> UpdateAsync(ExchangePrice exchangePrice);
        Task<ExchangePrice> CreateAsync(ExchangePrice exchangePrice);
        Task<ExchangePrice> GetByCurrencyIdAsync(int id);
    }
}
