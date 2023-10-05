using AutoMapper;
using Bwr.Exchange.Settings.Companies.Dto.CompanyBalances;

namespace Bwr.Exchange.Settings.Companies.Map
{
    public class CompanyBalanceMapProfile : Profile
    {
        public CompanyBalanceMapProfile()
        {
            CreateMap<CompanyBalance, CompanyBalanceDto>();
            CreateMap<CompanyBalanceDto, CompanyBalance>();
        }
    }
}
