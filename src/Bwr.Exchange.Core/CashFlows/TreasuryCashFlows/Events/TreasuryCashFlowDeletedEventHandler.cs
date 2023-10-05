using Abp.Dependency;
using Abp.Events.Bus.Handlers;
using Bwr.Exchange.CashFlows.TreasuryCashFlows.Services;
using Bwr.Exchange.Settings.Companies.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bwr.Exchange.CashFlows.TreasuryCashFlows.Events
{
    public class TreasuryCashFlowDeletedHandler : IAsyncEventHandler<TreasuryCashFlowDeletedEventData>, ITransientDependency
    {
        private readonly ITreasuryCashFlowManager _treasuryCashFlowManager;

        public TreasuryCashFlowDeletedHandler(ITreasuryCashFlowManager TreasuryCashFlowManager)
        {
            _treasuryCashFlowManager = TreasuryCashFlowManager;
        }
        public async Task HandleEventAsync(TreasuryCashFlowDeletedEventData eventData)
        {
            var treasuryCashFlow = await _treasuryCashFlowManager.GetByTransctionInfo(eventData.TransactionId, eventData.TransactionType);
            if (treasuryCashFlow != null)
            {
                ////Get all next cashflows
                //var nextTreasuryCashFlows = await _treasuryCashFlowManager.GetNextAsync(treasuryCashFlow);
                //if (nextTreasuryCashFlows.Any())
                //{
                //    //Get previous balance
                //    var previousBalance = _treasuryCashFlowManager.GetPreviousBalance(treasuryCashFlow.TreasuryId, treasuryCashFlow.CurrencyId, treasuryCashFlow.Date);
                //    await UpdateNextCashFlow(nextTreasuryCashFlows, previousBalance);
                //}

                await _treasuryCashFlowManager.DeleteAsync(treasuryCashFlow);
            }
        }

        private async Task UpdateNextCashFlow(IList<TreasuryCashFlow> treasuryCashFlows, double previousBalance)
        {
            if (treasuryCashFlows.Any())
            {
                var data = treasuryCashFlows.OrderBy(x => x.Date).ToList();
                for (int i = 0; i < treasuryCashFlows.Count(); i++)
                {
                    if (i == 0)
                    {
                        data[i].PreviousBalance = previousBalance;
                        data[i].CurrentBalance = data[i].PreviousBalance + data[i].Amount;
                    }
                    else
                    {
                        data[i].PreviousBalance = data[i - 1].CurrentBalance;
                        data[i].CurrentBalance = data[i].PreviousBalance + data[i].Amount;
                    }

                    await _treasuryCashFlowManager.UpdateAsync(data[i]);
                }
            }

        }
    }
}
