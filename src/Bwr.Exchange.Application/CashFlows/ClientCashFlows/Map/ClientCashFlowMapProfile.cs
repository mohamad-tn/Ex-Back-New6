using AutoMapper;
using Bwr.Exchange.CashFlows.ClientCashFlows.Dto;

namespace Bwr.Exchange.CashFlows.ClientCashFlows.Map
{
    public class ClientCashFlowMapProfile : Profile
    {
        public ClientCashFlowMapProfile()
        {
            CreateMap<ClientCashFlow, ClientCashFlowDto>();
            CreateMap<ClientCashFlow, ClientCashFlowTotalDto>();
        }
    }
}
