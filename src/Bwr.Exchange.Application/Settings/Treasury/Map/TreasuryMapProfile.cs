using AutoMapper;
using Bwr.Exchange.Settings.Treasury.Dto;

namespace Bwr.Exchange.Settings.Treasury.Map
{
    public class TreasuryMapProfile: Profile
    {
        public TreasuryMapProfile()
        {
            CreateMap<Bwr.Exchange.Settings.Treasuries.Treasury, TreasuryDto>();
            CreateMap<TreasuryDto, Bwr.Exchange.Settings.Treasuries.Treasury>();
        }
    }
}
