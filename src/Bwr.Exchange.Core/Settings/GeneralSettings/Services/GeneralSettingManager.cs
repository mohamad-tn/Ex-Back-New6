using Abp.Domain.Repositories;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Bwr.Exchange.Settings.GeneralSettings.Services
{
    public class GeneralSettingManager : IGeneralSettingManager
    {
        private readonly IRepository<GeneralSetting> _repository;

        public GeneralSettingManager(IRepository<GeneralSetting> repository)
        {
            _repository = repository;
        }

        public async Task<bool> CheckPassword(string password)
        {
            var generalSettings = await _repository.GetAllListAsync();
            if (generalSettings.Any())
            {
                var sha = GetSha(password);
                var generalSetting = generalSettings.FirstOrDefault();
                if(!string.IsNullOrEmpty(generalSetting.EditPassword))
                {
                    return generalSetting.EditPassword == sha;
                }
            }
            return true;
        }

        public async Task<GeneralSetting> Get()
        {
            var result = await _repository.GetAllListAsync();
            return result.Any() ? result.FirstOrDefault() : null;
        }

        public async Task<GeneralSetting> Update(GeneralSetting generalSetting)
        {
            var sha = GetSha(generalSetting.EditPassword);
            generalSetting.EditPassword = sha;

            var generalSettings = await _repository.GetAllListAsync();
            if (generalSettings.Any())
            {
                return await _repository.UpdateAsync(generalSetting);
            }
            else
            {
                var id = await _repository.InsertAndGetIdAsync(generalSetting);
                return await _repository.GetAsync(id);
            }
        }

        private string GetSha(string input)
        {
            var sha = SHA1.Create();
            var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            var sb = new StringBuilder(hash.Length * 2);
            foreach (byte b in hash)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
    }
}
