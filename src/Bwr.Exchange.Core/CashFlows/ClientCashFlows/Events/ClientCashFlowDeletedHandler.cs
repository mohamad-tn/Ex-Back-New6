using Abp.Dependency;
using Abp.Events.Bus.Handlers;
using Bwr.Exchange.CashFlows.ClientCashFlows.Services;
using Bwr.Exchange.Settings.Companies.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bwr.Exchange.CashFlows.ClientCashFlows.Events
{
    public class ClientCashFlowDeletedHandler : IAsyncEventHandler<ClientCashFlowDeletedEventData>, ITransientDependency
    {
        private readonly IClientCashFlowManager _clientCashFlowManager;

        public ClientCashFlowDeletedHandler(IClientCashFlowManager clientCashFlowManager)
        {
            _clientCashFlowManager = clientCashFlowManager;
        }
        public async Task HandleEventAsync(ClientCashFlowDeletedEventData eventData)
        {
            ClientCashFlow clientCashFlow = null;
            if (!string.IsNullOrEmpty(eventData.Type))
            {
                clientCashFlow = await _clientCashFlowManager.
                GetByTransctionInfo(eventData.TransactionId, eventData.TransactionType, eventData.Type);
            }
            else
            {
                //Get first
                clientCashFlow = await _clientCashFlowManager.
                GetByTransctionInfo(eventData.TransactionId, (int)eventData.TransactionType);
            }
            
            if (clientCashFlow != null)
            {
                ////Get all next cashflows
                //var nextClientCashFlows = await _clientCashFlowManager.GetNextAsync(clientCashFlow);
                //if (nextClientCashFlows.Any())
                //{
                //    //Get previous balance
                //    var previousBalance = _clientCashFlowManager.GetPreviousBalance(clientCashFlow.ClientId, clientCashFlow.CurrencyId, clientCashFlow.Date);
                //    await UpdateNextCashFlow(nextClientCashFlows, previousBalance);
                //}

                await _clientCashFlowManager.DeleteAsync(clientCashFlow);
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
