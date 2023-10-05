using System.Threading.Tasks;
using Bwr.Exchange.Configuration.Dto;

namespace Bwr.Exchange.Configuration
{
    public interface IConfigurationAppService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);
    }
}
