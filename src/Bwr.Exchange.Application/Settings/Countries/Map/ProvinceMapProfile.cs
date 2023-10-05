using AutoMapper;
using Bwr.Exchange.Settings.Countries.Dto.Provinces;

namespace Bwr.Exchange.Settings.Countries.Map
{
    public class ProvinceMapProfile : Profile
    {
        public ProvinceMapProfile()
        {
            CreateMap<Province, ReadProvinceDto>();
            CreateMap<Province, ProvinceDto>();
            CreateMap<ProvinceDto, Province>();
        }
    }
}
