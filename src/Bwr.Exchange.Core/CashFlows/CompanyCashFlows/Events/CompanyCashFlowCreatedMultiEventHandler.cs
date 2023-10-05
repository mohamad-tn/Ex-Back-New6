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
    public class CompanyCashFlowCreatedMultiEventHandler : IAsyncEventHandler<CompanyCashFlowCreatedMultiEventData>, ITransientDependency
    {
        private readonly ICompanyCashFlowManager _companyCashFlowManager;
        private readonly ICompanyManager _companyManager;

        public CompanyCashFlowCreatedMultiEventHandler(
            ICompanyCashFlowManager companyCashFlowManager,
            ICompanyManager companyManager
            )
        {
            _companyCashFlowManager = companyCashFlowManager;
            _companyManager = companyManager;
        }
        public async Task HandleEventAsync(CompanyCashFlowCreatedMultiEventData multiEventData)
        {
            if (multiEventData.Data.Any())
            {
                //var previousBalance = _companyCashFlowManager.GetPreviousBalance(multiEventData.Data[0].CompanyId.Value, multiEventData.Data[0].CurrencyId.Value, multiEventData.Data[0].Date);
                for(var i=0;i< multiEventData.Data.Count; i++)
                {
                    var companyCashFlow = new CompanyCashFlow()
                    {
                        CompanyId = multiEventData.Data[i].CompanyId.Value,
                        CurrencyId = multiEventData.Data[i].CurrencyId.Value,
                        Date = multiEventData.Data[i].Date,
                        Amount = multiEventData.Data[i].Amount,
                        Note = multiEventData.Data[i].Note,
                        Type = multiEventData.Data[i].Type,
                        //PreviousBalance = previousBalance,
                        CurrentBalance = multiEventData.Data[i].Amount + multiEventData.Data[i].CompanyCommission + multiEventData.Data[i].Commission,
                        TransactionId = multiEventData.Data[i].TransactionId,
                        TransactionType = multiEventData.Data[i].TransactionType,
                        Commission = multiEventData.Data[i].Commission,
                        CompanyCommission = multiEventData.Data[i].CompanyCommission,
                        Destination = multiEventData.Data[i].Destination,
                        Sender = multiEventData.Data[i].Sender,
                        Beneficiary = multiEventData.Data[i].Beneficiary
                    };


                    await _companyCashFlowManager.CreateAsync(companyCashFlow);
                    //previousBalance = companyCashFlow.CurrentBalance;

                    //if(i == (multiEventData.Data.Count - 1))
                    //{
                    //    await UpdateNextCashFlow(companyCashFlow);
                    //}
                }
                
            }
        }

        private async Task UpdateNextCashFlow(CompanyCashFlow current)
        {
            var companyCashFlows = await _companyCashFlowManager.GetNextAsync(current);
            if (companyCashFlows.Any())
            {
                var data = companyCashFlows.OrderBy(x => x.Date).ToList();
                for (int i = 0; i < companyCashFlows.Count(); i++)
                {
                    if (i == 0)
                    {
                        //if (data[i].PreviousBalance == current.CurrentBalance)
                        //    break;

                        data[i].PreviousBalance = current.CurrentBalance;
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
