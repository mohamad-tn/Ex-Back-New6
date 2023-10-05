using Abp.Dependency;
using Abp.Events.Bus.Handlers;
using Bwr.Exchange.CashFlows.CompanyCashFlows.Services;
using Bwr.Exchange.CashFlows.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bwr.Exchange.CashFlows.CompanyCashFlows.Events
{
    public class CompanyCashFlowDeletedMultiHandler : IAsyncEventHandler<CompanyCashFlowDeletedMultiEventData>, ITransientDependency
    {
        private readonly ICompanyCashFlowManager _companyCashFlowManager;

        public CompanyCashFlowDeletedMultiHandler(ICompanyCashFlowManager companyCashFlowManager)
        {
            _companyCashFlowManager = companyCashFlowManager;
        }
        public async Task HandleEventAsync(CompanyCashFlowDeletedMultiEventData eventData)
        {
            var companyCashFlows = await _companyCashFlowManager.
                GetAllByTransctionInfo(eventData.TransactionId, eventData.TransactionType, eventData.Type);

            //double previousBalance = 0;
            //IList<CompanyCashFlow> nextCompanyCashFlows = new List<CompanyCashFlow>();

            if (companyCashFlows.Any())
            {
                //var firstCashFlow = companyCashFlows.First();
                //var lastCashFlow = companyCashFlows.Last();

                //previousBalance = _companyCashFlowManager.GetPreviousBalance(firstCashFlow.CompanyId, firstCashFlow.CurrencyId, firstCashFlow.Date);
                //nextCompanyCashFlows = await _companyCashFlowManager.GetNextAsync(lastCashFlow);
                
                for (int i=0;i< companyCashFlows.Count; i++)
                {
                    await _companyCashFlowManager.DeleteAsync(companyCashFlows[i]);
                }

                //await UpdateNextCashFlow(nextCompanyCashFlows, previousBalance);
            }
        }

        private async Task UpdateNextCashFlow(IList<CompanyCashFlow> companyCashFlows, double previousBalance)
        {
            if (companyCashFlows.Any())
            {
                var data = companyCashFlows.OrderBy(x => x.Date).ToList();
                for (int i = 0; i < companyCashFlows.Count(); i++)
                {
                    if (i == 0)
                    {
                        data[i].PreviousBalance = previousBalance;
                        data[i].CurrentBalance = data[i].PreviousBalance + data[i].Amount + data[i].CompanyCommission + data[i].Commission;
                    }
                    else
                    {
                        data[i].PreviousBalance = data[i - 1].CurrentBalance;
                        data[i].CurrentBalance = data[i].PreviousBalance + data[i].Amount + data[i].CompanyCommission + data[i].Commission;
                    }

                    await _companyCashFlowManager.UpdateAsync(data[i]);
                }
            }

        }
    }
}