using Abp.Domain.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bwr.Exchange.Settings.Clients.Services
{
    public interface IClientManager : IDomainService
    {
        Task<IList<Client>> GetAllAsync();
        IList<Client> GetAllWithDetail();
        Client GetByIdWithDetail(int id);
        Task<Client> GetByIdAsync(int id);
        IList<Client> GetAll();
        ClientBalance GetClientBalance(int clientId, int currencyId);
        IList<ClientBalance> GetClientBalances();
        Task<Client> InsertAndGetAsync(Client country);
        Task<Client> UpdateAndGetAsync(Client country);
        Task DeleteAsync(int id);
        bool CheckIfNameAlreadyExist(string name, int branchId);
        bool CheckIfNameAlreadyExist(int id, string name, int branchId);
        Task<IList<Client>> GetAllForCurrentBranchAsync(int branchId);
        string GetClientNameById(int id);
    }
}
