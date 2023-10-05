using Abp.Application.Services;
using Bwr.Exchange.Settings.Clients.Dto;
using Bwr.Exchange.Settings.Clients.Dto.ClientBalances;
using Bwr.Exchange.Shared.Interfaces;
using System.Threading.Tasks;

namespace Bwr.Exchange.Settings.Clients
{
    public interface IClientAppService : IApplicationService, ICrudEjAppService<ClientDto, CreateClientDto, UpdateClientDto>
    {
        ClientBalanceDto GetCurrentBalance(CurrentBalanceInputDto input);
        Task<ClientBalanceDto> GetBalanceForEdit(ClientBalanceForEditInputDto input);
    }
}
