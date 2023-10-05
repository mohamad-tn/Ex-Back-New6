using Abp.Application.Services.Dto;
using Bwr.Exchange.Settings.Countries.Dto.Provinces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bwr.Exchange.Settings.Countries.Dto
{
    public class CountryDto : EntityDto
    {
        public CountryDto()
        {
            Provinces = new List<ProvinceDto>();
        }
        public string Name { get; set; }

        public IList<ProvinceDto> Provinces { get; set; }
    }
}
