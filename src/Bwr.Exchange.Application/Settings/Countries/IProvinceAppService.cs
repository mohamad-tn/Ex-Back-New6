using Abp.Application.Services;
using Bwr.Exchange.Settings.Countries.Dto.Provinces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bwr.Exchange.Settings.Countries
{
    public interface IProvinceAppService : IApplicationService
    {
        IList<ProvinceForDropdownDto> GetAllForDropdown();
        IList<ProvinceGroupDto> GetProvinceGroup();
    }
}
