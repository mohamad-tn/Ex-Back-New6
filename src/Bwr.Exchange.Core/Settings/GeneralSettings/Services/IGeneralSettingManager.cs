using Abp.Domain.Services;
using System.Threading.Tasks;

namespace Bwr.Exchange.Settings.GeneralSettings.Services
{
    public interface IGeneralSettingManager: IDomainService
    {
        Task<GeneralSetting> Update(GeneralSetting generalSetting);
        Task<GeneralSetting> Get();
        Task<bool> CheckPassword(string password);
    }
}
