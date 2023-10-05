using System.Threading.Tasks;
using Abp.Application.Services;
using Bwr.Exchange.Authorization.Accounts.Dto;

namespace Bwr.Exchange.Authorization.Accounts
{
    public interface IAccountAppService : IApplicationService
    {
        Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

        Task<RegisterOutput> Register(RegisterInput input);
    }
}
