using Abp.Domain.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bwr.Exchange.ExchangeCurrencies.Services.Interfaces
{
    public interface IExchangeCurrencyManager : IDomainService
    {
        Task<ExchangeCurrency> CreateAsync(ExchangeCurrency input);
        Task<ExchangeCurrency> UpdateAsync(ExchangeCurrency input);
        Task<ExchangeCurrency> GetByIdAsync(int id);
        Task DeleteAsync(ExchangeCurrency input);
        IList<ExchangeCurrency> Get(Dictionary<string, object> dic);
        Task<bool> DeleteCashFlowAsync(ExchangeCurrency input);
        int GetLastNumber();
    }
}
