using System.Threading.Tasks;
using Abp.Application.Services;
using Bwr.Exchange.Sessions.Dto;

namespace Bwr.Exchange.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}
