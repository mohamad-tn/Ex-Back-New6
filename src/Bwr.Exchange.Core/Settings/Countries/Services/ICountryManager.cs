using Abp.Domain.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bwr.Exchange.Settings.Countries.Services
{
    public interface ICountryManager : IDomainService
    {
        Task<IList<Country>> GetAllAsync();
        IList<Country> GetAllWithDetail();
        Country GetByIdWithDetail(int id);
        Task<Country> GetByIdAsync(int id);
        IList<Country> GetAll();
        Task<Country> InsertAndGetAsync(Country country);
        Task<Country> UpdateAndGetAsync(Country country);
        Task DeleteAsync(int id);
        bool CheckIfNameAlreadyExist(string name);
        bool CheckIfNameAlreadyExist(int id, string name);
    }
}
