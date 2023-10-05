using Abp.Dependency;
using Abp.Events.Bus.Handlers;
using Bwr.Exchange.CashFlows.ClientCashFlows.Services;
using Bwr.Exchange.Settings.Clients.Services;
using System.Linq;
using System.Threading.Tasks;

namespace Bwr.Exchange.CashFlows.ClientCashFlows.Events
{
    public class ClientCashFlowCreatedEventHandler : IAsyncEventHandler<ClientCashFlowCreatedEventData>, ITransientDependency
    {
        private readonly IClientCashFlowManager _clientCashFlowManager;
        private readonly IClientManager _clientManager;

        public ClientCashFlowCreatedEventHandler(
            IClientCashFlowManager clientCashFlowManager,
            IClientManager clientManager
            )
        {
            _clientCashFlowManager = clientCashFlowManager;
            _clientManager = clientManager;
        }

        public async Task HandleEventAsync(ClientCashFlowCreatedEventData eventData)
        {
            //var previousBalance = _clientCashFlowManager.GetPreviousBalance(eventData.ClientId.Value, eventData.CurrencyId.Value, eventData.Date);

            var amount = eventData.ReceivedAmount == 0 ? eventData.Amount : eventData.ReceivedAmount; // في حال كانت حركة قبض للحوالة الصادرة سوف نعرض المبلغ المستلم
            var currentBalance = 0.0;
            if(eventData.ReceivedAmount != 0)
            {
                currentBalance = eventData.ReceivedAmount;
            }
            else
            {
                currentBalance = eventData.Amount + eventData.Commission + eventData.ClientCommission;
            }
            var clientCashFlow = new ClientCashFlow()
            {
                ClientId = eventData.ClientId.Value,
                CurrencyId = eventData.CurrencyId.Value,
                Date = eventData.Date,
                Amount = amount,
                Note = eventData.Note,
                Type = eventData.Type,
                CurrentBalance = currentBalance,
                TransactionId = eventData.TransactionId,
                TransactionType = eventData.TransactionType,
                Commission = eventData.ReceivedAmount == 0 ? eventData.Commission : 0.0, //في حال كانت حركة قبض للحوالة الصادرة سوف نعرض 0 
                ClientCommission = eventData.ClientCommission,
                Destination = eventData.Destination,
                Sender = eventData.Sender,
                Beneficiary = eventData.Beneficiary
            };

            await _clientCashFlowManager.CreateAsync(clientCashFlow);
            //await UpdateNextCashFlow(clientCashFlow);
        }

        private async Task UpdateNextCashFlow(ClientCashFlow current)
        {
            var clientCashFlows = await _clientCashFlowManager.GetNextAsync(current);
            if (clientCashFlows.Any())
            {
                var data = clientCashFlows.OrderBy(x => x.Date).ToList();
                for (int i = 0; i < clientCashFlows.Count(); i++)
                {
                    if (i == 0)
                    {
                        //if (data[i].PreviousBalance == current.CurrentBalance) // هذا يعني انه لايوجد تغيير وبالتالي نخرج من الحلقة
                        //    break;

                        data[i].PreviousBalance = current.CurrentBalance;
                        data[i].CurrentBalance = data[i].PreviousBalance + data[i].Amount + data[i].ClientCommission + data[i].Commission;
                    }
                    else
                    {
                        data[i].PreviousBalance = data[i - 1].CurrentBalance;
                        data[i].CurrentBalance = data[i].PreviousBalance + data[i].Amount + data[i].ClientCommission + data[i].Commission;
                    }

                    await _clientCashFlowManager.UpdateAsync(data[i]);
                }
            }

        }

    }
}
