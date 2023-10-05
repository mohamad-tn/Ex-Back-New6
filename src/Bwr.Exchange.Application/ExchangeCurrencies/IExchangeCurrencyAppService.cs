using Abp.Application.Services;
using Bwr.Exchange.ExchangeCurrencies.Dto;
using Bwr.Exchange.Shared.DataManagerRequests;
using Bwr.Exchange.Shared.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Bwr.Exchange.ExchangeCurrencies
{
    public interface IExchangeCurrencyAppService: IApplicationService
    {
        Task<CreateExchangeCurrencyDto> CreateAsync(CreateExchangeCurrencyDto input);
        Task DeleteAsync(int id);
        Task<UpdateExchangeCurrencyDto> UpdateAsync(UpdateExchangeCurrencyDto input);
        Task<UpdateExchangeCurrencyDto> GetForEditAsync(int exchangeCurrencyId);
        ReadGrudDto GetForGrid([FromBody] ExchangeCurrencyDataManagerRequest dm);
        int GetLastNumber();
    }
}
