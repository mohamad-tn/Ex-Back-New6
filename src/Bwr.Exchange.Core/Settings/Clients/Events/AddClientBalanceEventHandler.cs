using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Events.Bus.Handlers;
using Bwr.Exchange.Settings.Clients.Services;
using Bwr.Exchange.Settings.Companies;
using System.Linq;
using System.Threading.Tasks;

namespace Bwr.Exchange.Settings.Clients.Events
{
    public class AddClientBalanceEventHandler : IAsyncEventHandler<AddClientBalanceEventData>, ITransientDependency
    {
        private readonly IRepository<ClientBalance> _clientBalanceRepository;
        private readonly IClientManager _clientManager;

        public AddClientBalanceEventHandler(
            IRepository<ClientBalance> clientBalanceRepository, 
            IClientManager clientManager)
        {
            _clientBalanceRepository = clientBalanceRepository;
            _clientManager = clientManager;
        }

        public async Task HandleEventAsync(AddClientBalanceEventData eventData)
        {
            var clients = await _clientManager.GetAllAsync();
            var currentClients = clients.Where(x => x.BranchId == eventData.BranchId);
            if (currentClients.Any())
            {
                foreach (var client in currentClients)
                {
                    var clientBalance = new ClientBalance()
                    {
                        ClientId = client.Id,
                        CurrencyId = eventData.CurrencyId,
                        Balance = 0
                    };

                    await _clientBalanceRepository.InsertAsync(clientBalance);
                }
                
            }
        }
    }
}
