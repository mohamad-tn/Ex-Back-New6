using Abp.Dependency;
using Abp.Events.Bus.Handlers;
using Bwr.Exchange.CashFlows.TreasuryCashFlows.Services;
using Bwr.Exchange.Settings.Treasuries.Services;
using System.Linq;
using System.Threading.Tasks;

namespace Bwr.Exchange.CashFlows.TreasuryCashFlows.Events
{
    public class TreasuryCashFlowCreatedEventHandler : IAsyncEventHandler<TreasuryCashFlowCreatedEventData>, ITransientDependency
    {

        private readonly ITreasuryCashFlowManager _treasuryCashFlowManager;
        private readonly ITreasuryBalanceManager _treasuryBalanceManager;

        public TreasuryCashFlowCreatedEventHandler(
            ITreasuryCashFlowManager treasuryCashFlowManager,
            ITreasuryBalanceManager treasuryBalanceManager
            )
        {
            _treasuryCashFlowManager = treasuryCashFlowManager;
            _treasuryBalanceManager = treasuryBalanceManager;
        }

        public async Task HandleEventAsync(TreasuryCashFlowCreatedEventData eventData)
        {
            //var previousBalance = _treasuryCashFlowManager.GetPreviousBalance(eventData.TreasuryId.Value, eventData.CurrencyId.Value, eventData.Date);
            var treasuryCashFlow = new TreasuryCashFlow()
            {
                Date = eventData.Date,
                Amount = eventData.Amount,
                CurrentBalance = eventData.Amount,
                CurrencyId = eventData.CurrencyId.Value,
                TreasuryId = eventData.TreasuryId.Value,
                TransactionId = eventData.TransactionId,
                TransactionType = eventData.TransactionType,
                Type = eventData.Type,
                Note = eventData.Note,
                Name = eventData.Name,
                Destination = eventData.Destination,
                Sender = eventData.Sender,
                Beneficiary = eventData.Beneficiary,
                //PreviousBalance = previousBalance
            };

            await _treasuryCashFlowManager.CreateAsync(treasuryCashFlow);
        }

        private async Task UpdateNextCashFlow(TreasuryCashFlow current)
        {
            var treasuryCashFlows = await _treasuryCashFlowManager.GetNextAsync(current);
            if (treasuryCashFlows.Any())
            {
                var data = treasuryCashFlows.OrderBy(x => x.Date).ToList();
                for (int i = 0; i < treasuryCashFlows.Count(); i++)
                {
                    if (i == 0)
                    {
                        //if (data[i].PreviousBalance == current.CurrentBalance)
                        //    break;

                        data[i].PreviousBalance = current.CurrentBalance;
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
        private async Task<double> GetPreviousBalance(int treasuryId, int currencyId)
        {
            var previousTreasuryBalance = await _treasuryCashFlowManager.GetLastAsync(treasuryId, currencyId);
            if (previousTreasuryBalance != null)
            {
                return previousTreasuryBalance.CurrentBalance;
            }
            else
            {
                var treasuryBalance = _treasuryBalanceManager.GetByTreasuryIdAndCurrency(treasuryId, currencyId);
                if (treasuryBalance != null)
                    return treasuryBalance.InitilBalance;
            }

            return 0.0;
        }
    }
}
