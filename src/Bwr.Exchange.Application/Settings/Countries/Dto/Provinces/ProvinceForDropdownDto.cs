using Abp.Application.Services.Dto;

namespace Bwr.Exchange.Settings.Countries.Dto.Provinces
{
    public class ProvinceForDropdownDto : EntityDto
    {
        public string Name { get; set; }

        public string CountryName { get; set; }
    }
}
