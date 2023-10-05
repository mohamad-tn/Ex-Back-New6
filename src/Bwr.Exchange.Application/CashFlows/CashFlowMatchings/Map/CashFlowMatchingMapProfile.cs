using AutoMapper;
using Bwr.Exchange.CashFlows.CashFlowMatchings.Dto;

namespace Bwr.Exchange.CashFlows.CashFlowMatchings.Map
{
    public class CashFlowMatchingMapProfile : Profile
    {
        public CashFlowMatchingMapProfile()
        {
            CreateMap<CashFlowMatchingDto, CashFlowMatching>();
            CreateMap<CashFlowMatching, CashFlowMatchingDto>();
        }
    }
}
