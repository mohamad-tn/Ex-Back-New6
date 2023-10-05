using Abp.Dependency;
using Abp.Events.Bus.Handlers;
using Bwr.Exchange.CashFlows.CompanyCashFlows.Services;
using Bwr.Exchange.Settings.Companies.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bwr.Exchange.CashFlows.CompanyCashFlows.Events
{
    public class CompanyCashFlowCreatedAndUpdateNextHandler : IAsyncEventHandler<CompanyCashFlowCreatedAndUpdateNextEventData>, ITransientDependency
    {
        private readonly ICompanyCashFlowManager _companyCashFlowManager;
        private readonly ICompanyManager _companyManager;

        public CompanyCashFlowCreatedAndUpdateNextHandler(
            ICompanyCashFlowManager companyCashFlowManager,
            ICompanyManager companyManager
            )
        {
            _companyCashFlowManager = companyCashFlowManager;
            _companyManager = companyManager;
        }

        public async Task HandleEventAsync(CompanyCashFlowCreatedAndUpdateNextEventData eventData)
        {
            //var previousBalance = _companyCashFlowManager.GetPreviousBalance(eventData.CompanyId.Value, eventData.CurrencyId.Value, eventData.Date);
            var companyCashFlow = new CompanyCashFlow()
            {
                CompanyId = eventData.CompanyId.Value,
                CurrencyId = eventData.CurrencyId.Value,
                Date = eventData.Date,
                Amount = eventData.Amount,
                Note = eventData.Note,
                Type = eventData.Type,
                //PreviousBalance = previousBalance,
                CurrentBalance = eventData.Amount + eventData.CompanyCommission + eventData.Commission,
                TransactionId = eventData.TransactionId,
                TransactionType = eventData.TransactionType,
                Commission = eventData.Commission,
                CompanyCommission = eventData.CompanyCommission,
                Destination = eventData.Destination,
                Sender = eventData.Sender,
                Beneficiary = eventData.Beneficiary
            };

            await _companyCashFlowManager.CreateAsync(companyCashFlow);

            //await UpdateNextCashFlow(companyCashFlow);
        }

        private async Task UpdateNextCashFlow(CompanyCashFlow current)
        {
            var companyCashFlows = await _companyCashFlowManager.GetNextAsync(current);
            if (companyCashFlows.Any())
            {
                var data = companyCashFlows.OrderBy(x=>x.Date).ToList();
                for(int i = 0; i < companyCashFlows.Count(); i++)
                {
                    if(i == 0)
                    {
                        if (data[i].PreviousBalance == current.CurrentBalance)
                            break;

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

        private async Task<double> GetPreviousBalance(int companyId, int currencyId)
        {
            var previousCompanyBalance = await _companyCashFlowManager.GetLastAsync(companyId, currencyId);
            if (previousCompanyBalance != null)
            {
                return previousCompanyBalance.CurrentBalance;
            }
            else
            {
                var companyBalance = _companyManager.GetCompanyBalance(companyId, currencyId);
                if (companyBalance != null)
                    return companyBalance.Balance;
            }

            return 0.0;
        }
    }
}
