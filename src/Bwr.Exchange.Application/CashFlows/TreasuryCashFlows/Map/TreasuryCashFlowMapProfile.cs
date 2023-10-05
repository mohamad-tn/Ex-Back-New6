using AutoMapper;
using Bwr.Exchange.CashFlows.TreasuryCashFlows.Dto;

namespace Bwr.Exchange.CashFlows.TreasuryCashFlows.Map
{
    public class TreasuryCashFlowMapProfile : Profile
    {
        public TreasuryCashFlowMapProfile()
        {
            CreateMap<TreasuryCashFlow, TreasuryCashFlowDto>();
        }
    }
}
