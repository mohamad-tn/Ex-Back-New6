using Abp.Application.Services.Dto;

namespace Bwr.Exchange.Settings.Countries.Dto.Provinces
{
    public class ProvinceGroupDto : EntityDto
    {
        public string ProvinceId { get; set; }
        public string Name { get; set; }
        public string Group { get; set; }
        public int CountryId { get; set; }
    }
}
