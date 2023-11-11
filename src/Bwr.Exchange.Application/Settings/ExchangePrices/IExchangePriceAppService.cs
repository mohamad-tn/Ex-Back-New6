using Abp.Application.Services;
using Bwr.Exchange.Settings.ExchangePrices.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bwr.Exchange.Settings.ExchangePrices
{
    public interface IExchangePriceAppService: IApplicationService
    {
        Task<ExchangePriceDto> UpdateAsync(UpdateExchangePriceDto input);
        Task<ExchangePriceDto> GetById(int id);
        Task<IList<ExchangePriceDto>> GetAllAsync();
    }
}
