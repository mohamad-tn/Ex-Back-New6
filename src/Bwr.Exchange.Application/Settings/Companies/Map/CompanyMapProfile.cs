using AutoMapper;
using Bwr.Exchange.Settings.Companies.Dto;

namespace Bwr.Exchange.Settings.Companies.Map
{
    public class CompanyMapProfile:Profile
    {
        public CompanyMapProfile()
        {
            CreateMap<Company, CompanyDto>();
            CreateMap<Company, ReadCompanyDto>();
            CreateMap<Company, CreateCompanyDto>();
            CreateMap<CreateCompanyDto, Company>();
            CreateMap<Company, UpdateCompanyDto>();
            CreateMap<UpdateCompanyDto, Company>();
        }
    }
}
