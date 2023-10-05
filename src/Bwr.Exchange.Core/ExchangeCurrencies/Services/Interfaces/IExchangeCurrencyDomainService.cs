using Abp.Domain.Services;
using System.Threading.Tasks;

namespace Bwr.Exchange.ExchangeCurrencies.Services.Interfaces
{
    public interface IExchangeCurrencyDomainService : IDomainService
    {
        Task<ExchangeCurrency> CreateAsync(ExchangeCurrency exchangeCurrency);
        Task<ExchangeCurrency> UpdateAsync(ExchangeCurrency exchangeCurrency);
    }
}
