using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bwr.Exchange.Settings.Clients.Services
{
    public class ClientManager : IClientManager
    {
        private readonly IRepository<Client> _clientRepository;
        private readonly IRepository<ClientPhone> _clientPhoneRepository;
        private readonly IRepository<ClientBalance> _clientBalanceRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        public ClientManager(
            IRepository<Client> clientRepository,
            IRepository<ClientPhone> clientPhoneRepository,
            IRepository<ClientBalance> clientBalanceRepository,
            IUnitOfWorkManager unitOfWorkManager
            )
        {
            _clientRepository = clientRepository;
            _clientPhoneRepository = clientPhoneRepository;
            _clientBalanceRepository = clientBalanceRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public bool CheckIfNameAlreadyExist(string name, int branchId)
        {
            var client = _clientRepository.FirstOrDefault(x => x.Name.Trim().Equals(name.Trim()) && x.BranchId == branchId);
            return client != null ? true : false;
        }

        public bool CheckIfNameAlreadyExist(int id, string name, int branchId)
        {
            var client = _clientRepository.FirstOrDefault(x => x.Id != id && x.Name.Trim().Equals(name.Trim()) && x.BranchId == branchId);
            return client != null ? true : false;
        }

        public async Task DeleteAsync(int id)
        {
            var client = await GetByIdAsync(id);
            if (client != null)
                await _clientRepository.DeleteAsync(client);
        }

        public IList<Client> GetAll()
        {
            return _clientRepository.GetAll().ToList();
        }

        public async Task<IList<Client>> GetAllForCurrentBranchAsync(int branchId)
        {
            var clients = await _clientRepository.GetAllListAsync();
            return clients.Where(x => x.BranchId == branchId).ToList();
        }

        public async Task<IList<Client>> GetAllAsync()
        {
            return await _clientRepository.GetAllListAsync();
        }

        public IList<Client> GetAllWithDetail()
        {
            var clients = _clientRepository.GetAllIncluding(
                ph => ph.ClientPhones,
                b => b.ClientBalances,
                pr => pr.Province
                );
            return clients.ToList();
        }

        public Client GetByIdWithDetail(int id)
        {
            var client = GetAllWithDetail().FirstOrDefault(x => x.Id == id);
            return client;
        }

        public async Task<Client> GetByIdAsync(int id)
        {
            return await _clientRepository.GetAsync(id);
        }

        public async Task<Client> InsertAndGetAsync(Client client)
        {
            int clientId = 0;
            using (var unitOfWork = _unitOfWorkManager.Begin())
            {
                clientId = await _clientRepository.InsertAndGetIdAsync(client);

                //Update client phones
                var clientPhones = client.ClientPhones.ToList();//Don't remove ToList()
                await RemoveClientPhones(clientId, clientPhones);
                await AddNewClientPhones(clientId, clientPhones);

                ////Update client balances
                var clientBalances = client.ClientBalances.ToList();//Don't remove ToList()
                //await RemoveClientBalances(clientId, clientBalances);
                await AddNewClientBalances(clientId, clientBalances);

                unitOfWork.Complete();
            }
            return await GetByIdAsync(clientId);
        }

        public async Task<Client> UpdateAndGetAsync(Client client)
        {
            using (var unitOfWork = _unitOfWorkManager.Begin())
            {
                var updatedClient = await _clientRepository.UpdateAsync(client);

                //Update client phones
                var clientPhones = client.ClientPhones.ToList();//Don't remove ToList()
                await RemoveClientPhones(updatedClient.Id, clientPhones);
                await AddNewClientPhones(updatedClient.Id, clientPhones);

                //Update client balances
                var clientBalances = client.ClientBalances.ToList();//Don't remove ToList()
                //await RemoveClientBalances(updatedClient.Id, clientBalances);
                //await UpdateClientBalances(updatedClient.Id, clientBalances);

                unitOfWork.Complete();
            }
            return await GetByIdAsync(client.Id);
        }

        public ClientBalance GetClientBalance(int clientId, int currencyId)
        {
            ClientBalance clientBalance = null;
            var client = GetAllWithDetail().FirstOrDefault(x => x.Id == clientId);
            if (client != null)
            {
                clientBalance = client.ClientBalances.FirstOrDefault(x => x.CurrencyId == currencyId);
            }

            return clientBalance;
        }

        public IList<ClientBalance> GetClientBalances()
        {
            return _clientBalanceRepository.GetAllIncluding(x => x.Currency).ToList();
        }

        #region Helper Methods
        private async Task RemoveClientPhones(int clientId, IList<ClientPhone> newClientPhones)
        {
            var oldClientPhones = await _clientPhoneRepository.GetAllListAsync(x => x.ClientId == clientId);

            foreach (var oldClientPhone in oldClientPhones)
            {
                var isExist = newClientPhones.Any(x => x.PhoneNumber == oldClientPhone.PhoneNumber.Trim());
                if (!isExist)
                {
                    await _clientPhoneRepository.DeleteAsync(oldClientPhone);
                }
            }
        }

        private async Task AddNewClientPhones(int clientId, IList<ClientPhone> newClientPhones)
        {
            var oldClientPhones = await _clientPhoneRepository.GetAllListAsync(x => x.ClientId == clientId);
            foreach (var newClientPhone in newClientPhones)
            {
                var isExist = oldClientPhones.Any(x => x.PhoneNumber == newClientPhone.PhoneNumber.Trim());
                if (!isExist)
                {
                    await _clientPhoneRepository.InsertAsync(newClientPhone);
                }
            }
        }

        private async Task RemoveClientBalances(int clientId, IList<ClientBalance> newClientBalances)
        {
            var oldClientBalances = await _clientBalanceRepository.GetAllListAsync(x => x.ClientId == clientId);

            foreach (var oldClientBalance in oldClientBalances)
            {
                var isExist = newClientBalances.Any(x => x.CurrencyId == oldClientBalance.CurrencyId);
                if (!isExist)
                {
                    await _clientBalanceRepository.DeleteAsync(oldClientBalance);
                }
            }
        }

        private async Task AddNewClientBalances(int clientId, IList<ClientBalance> newClientBalances)
        {
            var oldClientBalances = await _clientBalanceRepository.GetAllListAsync(x => x.ClientId == clientId);
            foreach (var newClientBalance in newClientBalances)
            {
                var isExist = oldClientBalances.Any(x => x.CurrencyId == newClientBalance.CurrencyId);
                if (!isExist)
                {
                    await _clientBalanceRepository.InsertAsync(newClientBalance);
                }
            }
        }

        private async Task UpdateClientBalances(int clientId, IList<ClientBalance> newClientBalances)
        {
            var clientBalances = await _clientBalanceRepository.GetAllListAsync(x => x.ClientId == clientId);
            foreach (var clientBalance in clientBalances)
            {
                var newClientBalance = newClientBalances.FirstOrDefault(x => x.CurrencyId == clientBalance.CurrencyId);
                if (newClientBalance != null)
                {
                    clientBalance.Balance = newClientBalance.Balance;
                    await _clientBalanceRepository.UpdateAsync(clientBalance);
                }
            }
        }


        #endregion
    }
}
