
using Abp.Application.Services;
using Bwr.Exchange.Settings.Commisions.Dto;
using Bwr.Exchange.Shared.Interfaces;

namespace Bwr.Exchange.Settings.Commisions
{
    public interface ICommisionAppService : IApplicationService, ICrudEjAppService<CommisionDto, CreateCommisionDto, UpdateCommisionDto>
    {

    }
}
