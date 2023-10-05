using AutoMapper;
using Bwr.Exchange.Settings.Clients;
using Bwr.Exchange.Settings.Clients.Dto;

namespace Bwr.Exchange.Settings.Countries.Map
{
    public class ClientMapProfile:Profile
    {
        public ClientMapProfile()
        {
            CreateMap<Client, ClientDto>();
            CreateMap<Client, ReadClientDto>();
            CreateMap<Client, CreateClientDto>();
            CreateMap<CreateClientDto, Client>();
            CreateMap<Client, UpdateClientDto>();
            CreateMap<UpdateClientDto, Client>();
        }
    }
}
