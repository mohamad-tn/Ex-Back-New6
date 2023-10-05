
using Abp.Application.Services;
using Bwr.Exchange.Settings.Incomes.Dto;
using Bwr.Exchange.Shared.Interfaces;

namespace Bwr.Exchange.Settings.Incomes
{
    public interface IIncomeAppService : IApplicationService, ICrudEjAppService<IncomeDto, CreateIncomeDto, UpdateIncomeDto>
    {

    }
}
