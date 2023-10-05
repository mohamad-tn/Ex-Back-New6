using AutoMapper;
using Bwr.Exchange.CashFlows.CompanyCashFlows.Dto;

namespace Bwr.Exchange.CashFlows.CompanyCashFlows.Map
{
    public class CompanyCashFlowMapProfile : Profile
    {
        public CompanyCashFlowMapProfile()
        {
            CreateMap<CompanyCashFlow, CompanyCashFlowDto>();
        }
    }
}
