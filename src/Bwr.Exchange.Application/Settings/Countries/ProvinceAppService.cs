using Abp.Domain.Repositories;
using Bwr.Exchange.Settings.Clients;
using Bwr.Exchange.Settings.Countries.Dto.Provinces;
using Bwr.Exchange.Settings.Countries.Services;
using Bwr.Exchange.TreasuryActions.Dto;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bwr.Exchange.Settings.Countries
{
    public class ProvinceAppService : ExchangeAppServiceBase, IProvinceAppService
    {
        private readonly ICountryManager _countryManager;

        public ProvinceAppService(ICountryManager countryManager)
        {
            _countryManager = countryManager;
        }

        public IList<ProvinceForDropdownDto> GetAllForDropdown()
        {
            var provinces = _countryManager.GetAllWithDetail().ToList().SelectMany(x => x.Provinces);
            return (from province in provinces
                    select new ProvinceForDropdownDto()
                    {
                        CountryName = province.Country != null ? province.Country.Name : string.Empty,
                        Name = province.Name,
                        Id = province.Id
                    }).ToList();
        }

        public IList<ProvinceGroupDto> GetProvinceGroup()
        {
            var provinceGroup = new List<ProvinceGroupDto>();
            var countries = _countryManager.GetAllWithDetail();

            foreach (var country in countries)
            {
                var provs = country.Provinces;
                provinceGroup.AddRange((from e in provs
                                        select new ProvinceGroupDto
                                        {
                                            Id = e.Id,
                                            Name = e.Name,
                                            Group = e.Country.Name,
                                            ProvinceId = $"co{e.Id}",
                                            CountryId = e.CountryId
                                        }).ToList());
            }

            return provinceGroup;

        }
    }
}
