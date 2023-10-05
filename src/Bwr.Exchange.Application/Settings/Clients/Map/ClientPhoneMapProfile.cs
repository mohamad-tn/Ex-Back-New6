using AutoMapper;
using Bwr.Exchange.Settings.Clients;
using Bwr.Exchange.Settings.Clients.Dto.ClientPhones;

namespace Bwr.Exchange.Settings.Countries.Map
{
    public class ClientPhoneMapProfile : Profile
    {
        public ClientPhoneMapProfile()
        {
            CreateMap<ClientPhone, ClientPhoneDto>();
            CreateMap<ClientPhoneDto, ClientPhone>();
        }
    }
}
