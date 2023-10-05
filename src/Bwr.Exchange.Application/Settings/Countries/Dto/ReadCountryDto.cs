using Bwr.Exchange.Settings.Countries.Dto.Provinces;
using System.Collections.Generic;

namespace Bwr.Exchange.Settings.Countries.Dto
{
    public class ReadCountryDto
    {
        public ReadCountryDto()
        {
            Provinces = new List<ReadProvinceDto>();
        }
        public int id { get; set; }
        public string name { get; set; }

        public IList<ReadProvinceDto> Provinces { get; set; }
    }
}
