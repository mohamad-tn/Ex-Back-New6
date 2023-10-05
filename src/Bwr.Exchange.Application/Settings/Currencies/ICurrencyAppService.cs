
using Abp.Application.Services;
using Bwr.Exchange.Settings.Currencies.Dto;
using Bwr.Exchange.Shared.Interfaces;

namespace Bwr.Exchange.Settings.Currencies
{
    public interface ICurrencyAppService : IApplicationService, ICrudEjAppService<CurrencyDto, CreateCurrencyDto, UpdateCurrencyDto>
    {

    }
}
