using AutoMapper;
using Bwr.Exchange.TreasuryActions.Dto;

namespace Bwr.Exchange.TreasuryActions.Map
{
    public class TreasuryActionMapProfile : Profile
    {
        public TreasuryActionMapProfile()
        {
            CreateMap<TreasuryAction, TreasuryActionDto>();
            CreateMap<TreasuryAction, TreasuryActionStatementOutputDto>();
            CreateMap<TreasuryAction, ListTreasuryActionDto>();
            CreateMap<TreasuryActionDto, TreasuryAction>();
        }
    }
}
