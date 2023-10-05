using Bwr.Exchange.Settings.Countries.Dto.Provinces;
using System.Collections.Generic;

namespace Bwr.Exchange.Settings.Countries.Dto
{
    public class CreateCountryDto
    {
        public CreateCountryDto()
        {
            Provinces = new List<ProvinceDto>();
        }
        public string Name { get; set; }

        public IList<ProvinceDto> Provinces { get; set; }
    }
}
