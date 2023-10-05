using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Runtime.Session;
using Bwr.Exchange.Configuration.Dto;

namespace Bwr.Exchange.Configuration
{
    [AbpAuthorize]
    public class ConfigurationAppService : ExchangeAppServiceBase, IConfigurationAppService
    {
        public async Task ChangeUiTheme(ChangeUiThemeInput input)
        {
            await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
        }
    }
}
