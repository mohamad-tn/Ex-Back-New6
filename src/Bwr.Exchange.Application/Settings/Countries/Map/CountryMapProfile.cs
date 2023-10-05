using AutoMapper;
using Bwr.Exchange.Settings.Countries.Dto;

namespace Bwr.Exchange.Settings.Countries.Map
{
    public class CountryMapProfile:Profile
    {
        public CountryMapProfile()
        {
            CreateMap<Country, CountryDto>();
            CreateMap<Country, ReadCountryDto>();
            CreateMap<Country, CreateCountryDto>();
            CreateMap<CreateCountryDto, Country>();
            CreateMap<Country, UpdateCountryDto>();
            CreateMap<UpdateCountryDto, Country>();
        }
    }
}
