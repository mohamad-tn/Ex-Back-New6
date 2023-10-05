using Abp.Application.Services;
using Bwr.Exchange.Settings.GeneralSettings.Dto;
using System.Threading.Tasks;

namespace Bwr.Exchange.Settings.GeneralSettings
{
    public interface IGeneralSettingAppService: IApplicationService
    {
        Task<GeneralSettingDto> Update(GeneralSettingDto generalSetting);
        Task<GeneralSettingDto> Get();
        Task<CheckPasswordOutput> CheckPassword(CheckPasswordInput input);
    }
}
