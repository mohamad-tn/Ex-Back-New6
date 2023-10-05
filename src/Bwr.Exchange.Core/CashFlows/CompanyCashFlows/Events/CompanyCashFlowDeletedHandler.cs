using Abp.Dependency;
using Abp.Events.Bus.Handlers;
using Bwr.Exchange.CashFlows.CompanyCashFlows.Services;
using Bwr.Exchange.Settings.Companies.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bwr.Exchange.CashFlows.CompanyCashFlows.Events
{
    public class CompanyCashFlowDeletedHandler : IAsyncEventHandler<CompanyCashFlowDeletedEventData>, ITransientDependency
    {
        private readonly ICompanyCashFlowManager _companyCashFlowManager;

        public CompanyCashFlowDeletedHandler(ICompanyCashFlowManager companyCashFlowManager)
        {
            _companyCashFlowManager = companyCashFlowManager;
        }
        public async Task HandleEventAsync(CompanyCashFlowDeletedEventData eventData)
        {
            CompanyCashFlow companyCashFlow = null;
            if (!string.IsNullOrEmpty(eventData.Type))
            {
                companyCashFlow = await _companyCashFlowManager.
                GetByTransctionInfo(eventData.TransactionId, eventData.TransactionType, eventData.Type);
            }
            else
            {
                //Get first
                companyCashFlow = await _companyCashFlowManager.
                GetByTransctionInfo(eventData.TransactionId, (int)eventData.TransactionType);
            }

            if (companyCashFlow != null)
            {
                ////Get all next cashflows
                //var nextCompanyCashFlows = await _companyCashFlowManager.GetNextAsync(companyCashFlow);
                //if (nextCompanyCashFlows.Any())
                //{
                //    //Get previous balance
                //    var previousBalance = _companyCashFlowManager.GetPreviousBalance(companyCashFlow.CompanyId, companyCashFlow.CurrencyId, companyCashFlow.Date);
                //    await UpdateNextCashFlow(nextCompanyCashFlows, previousBalance);
                //}

                await _companyCashFlowManager.DeleteAsync(companyCashFlow);
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
