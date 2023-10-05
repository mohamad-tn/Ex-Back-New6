using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Events.Bus.Handlers;
using System.Linq;
using System.Threading.Tasks;

namespace Bwr.Exchange.Settings.Clients.Events
{
    public class DeleteClientBalanceEventHandler : IAsyncEventHandler<DeleteClientBalanceEventData>, ITransientDependency
    {
        private readonly IRepository<ClientBalance> _clientBalanceRepository;

        public DeleteClientBalanceEventHandler(IRepository<ClientBalance> clientBalanceRepository)
        {
            _clientBalanceRepository = clientBalanceRepository;
        }

        public async Task HandleEventAsync(DeleteClientBalanceEventData eventData)
        {
            var clientBalances = await _clientBalanceRepository.GetAllListAsync(x => x.CurrencyId == eventData.CurrencyId);
            if (clientBalances.Any())
            {
                foreach (var clientBalance in clientBalances)
                {
                    await _clientBalanceRepository.DeleteAsync(clientBalance);
                }
            }
        }
    }
}
