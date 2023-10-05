using AutoMapper;
using Bwr.Exchange.Settings.Clients;
using Bwr.Exchange.Settings.Clients.Dto.ClientBalances;

namespace Bwr.Exchange.Settings.Countries.Map
{
    public class ClientBalanceMapProfile : Profile
    {
        public ClientBalanceMapProfile()
        {
            CreateMap<ClientBalance, ReadClientBalanceDto>();
            CreateMap<ClientBalance, ClientBalanceDto>();
            CreateMap<ClientBalanceDto, ClientBalance>();
        }
    }
}
