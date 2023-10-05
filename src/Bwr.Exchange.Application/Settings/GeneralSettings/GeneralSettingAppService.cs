using Bwr.Exchange.Settings.GeneralSettings.Dto;
using Bwr.Exchange.Settings.GeneralSettings.Services;
using System.Threading.Tasks;

namespace Bwr.Exchange.Settings.GeneralSettings
{
    public class GeneralSettingAppService : ExchangeAppServiceBase, IGeneralSettingAppService
    {
        private readonly IGeneralSettingManager _generalSettingManager;

        public GeneralSettingAppService(IGeneralSettingManager generalSettingManager)
        {
            _generalSettingManager = generalSettingManager;
        }

        public async Task<CheckPasswordOutput> CheckPassword(CheckPasswordInput input)
        {
            var success = await _generalSettingManager.CheckPassword(input.password);
            return new CheckPasswordOutput(success);
        }

        public async Task<GeneralSettingDto> Get()
        {
            var setting = await _generalSettingManager.Get();
            if (setting != null)
            {
                return new GeneralSettingDto()
                {
                    CompanyName = setting.CompanyName,
                    DbSuffex = setting.DbSuffex
                };
            }
            else
            {
                return null;
            }
        }

        public async Task<GeneralSettingDto> Update(GeneralSettingDto input)
        {
            var generalSetting = new GeneralSetting()
            {
                CompanyName = input.CompanyName,
                DbSuffex = input.DbSuffex,
                EditPassword = input.EditPassword
            };
            var setting = await _generalSettingManager.Update(generalSetting);
            return new GeneralSettingDto()
            {
                CompanyName = setting.CompanyName,
                DbSuffex = setting.DbSuffex
            };
        }
    }
}
