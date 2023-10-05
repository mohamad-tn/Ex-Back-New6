using Abp.Dependency;
using Abp.Events.Bus.Handlers;
using Bwr.Exchange.CashFlows.ClientCashFlows.Services;
using Bwr.Exchange.CashFlows.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bwr.Exchange.CashFlows.ClientCashFlows.Events
{
    public class ClientCashFlowDeletedMultiHandler : IAsyncEventHandler<ClientCashFlowDeletedMultiEventData>, ITransientDependency
    {
        private readonly IClientCashFlowManager _clientCashFlowManager;

        public ClientCashFlowDeletedMultiHandler(IClientCashFlowManager clientCashFlowManager)
        {
            _clientCashFlowManager = clientCashFlowManager;
        }
        public async Task HandleEventAsync(ClientCashFlowDeletedMultiEventData eventData)
        {
            var clientCashFlows = await _clientCashFlowManager.
                GetAllByTransctionInfo(eventData.TransactionId, eventData.TransactionType, eventData.Type);

            //double previousBalance = 0;
            //IList<ClientCashFlow> nextClientCashFlows = new List<ClientCashFlow>();

            if (clientCashFlows.Any())
            {
                //var firstCashFlow = clientCashFlows.First();
                //var lastCashFlow = clientCashFlows.Last();

                //previousBalance = _clientCashFlowManager.GetPreviousBalance(firstCashFlow.ClientId, firstCashFlow.CurrencyId, firstCashFlow.Date);
                //nextClientCashFlows = await _clientCashFlowManager.GetNextAsync(lastCashFlow);
                
                for (int i=0;i< clientCashFlows.Count; i++)
                {
                    await _clientCashFlowManager.DeleteAsync(clientCashFlows[i]);
                }

                //await UpdateNextCashFlow(nextClientCashFlows, previousBalance);
            }
        }

        private async Task UpdateNextCashFlow(IList<ClientCashFlow> clientCashFlows, double previousBalance)
        {
            if (clientCashFlows.Any())
            {
                var data = clientCashFlows.OrderBy(x => x.Date).ToList();
                for (int i = 0; i < clientCashFlows.Count(); i++)
                {
                    if (i == 0)
                    {
                        data[i].PreviousBalance = previousBalance;
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