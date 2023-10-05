using Abp.Domain.Services;
using System.Threading.Tasks;

namespace Bwr.Exchange.ExchangeCurrencies.Services.Interfaces
{
    public interface IExchangeCurrencyHistoryManager: IDomainService
    {
        Task<ExchangeCurrencyHistory> CreateAsync(ExchangeCurrency exchangeCurrency);
        Task<ExchangeCurrencyHistory> GetAsync(int exchangeCurrencyId);
    }
}
