using Abp.Domain.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bwr.Exchange.Settings.Commisions.Services
{
    public interface ICommisionManager : IDomainService
    {
        Task<IList<Commision>> GetAllAsync();
        Task<Commision> GetByIdAsync(int id);
        IList<Commision> GetAll();
        IList<Commision> GetAllWithDetails();
        Task<Commision> InsertAndGetAsync(Commision income);
        Task<Commision> UpdateAndGetAsync(Commision income);
        Task DeleteAsync(int id);
        bool CheckIfCommisionAlreadyExist(int currencyId);
        bool CheckIfCommisionAlreadyExist(int commisionId, int currencyId);
    }
}
