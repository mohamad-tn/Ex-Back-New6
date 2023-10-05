using Abp.Domain.Repositories;
using Bwr.Exchange.CashFlows.CashFlowMatchings;
using Bwr.Exchange.CashFlows.CashFlowMatchings.Services;
using Bwr.Exchange.CashFlows.TreasuryCashFlows.Events.TreasuryCashFlowDeletedMulti;
using Bwr.Exchange.Settings.Treasuries.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bwr.Exchange.CashFlows.TreasuryCashFlows.Services
{
    public class TreasuryCashFlowManager : ITreasuryCashFlowManager
    {
        private readonly IRepository<TreasuryCashFlow> _treasuryCashFlowRepository;
        private readonly ITreasuryBalanceManager _treasuryBalanceManager;
        private readonly ICashFlowMatchingManager _cashFlowMatchingManager;

        public TreasuryCashFlowManager(
            IRepository<TreasuryCashFlow> treasuryCashFlowRepository, 
            ITreasuryBalanceManager treasuryBalanceManager,
            ICashFlowMatchingManager cashFlowMatchingManager)
        {
            _treasuryCashFlowRepository = treasuryCashFlowRepository;
            _treasuryBalanceManager = treasuryBalanceManager;
            _cashFlowMatchingManager = cashFlowMatchingManager;
        }

        public async Task CreateAsync(TreasuryCashFlow input)
        {
            await _treasuryCashFlowRepository.InsertAsync(input);
        }
        public async Task UpdateAsync(TreasuryCashFlow input)
        {
            await _treasuryCashFlowRepository.UpdateAsync(input);
        }
        public async Task DeleteAsync(TreasuryCashFlow input)
        {
            await _treasuryCashFlowRepository.DeleteAsync(input);
        }
        public async Task DeleteAsync(TreasuryCashFlowDeletedMultiEventData eventData)
        {
            var result = await
               GetAllByTransctionInfo(eventData.TransactionId, eventData.TransactionType, eventData.Type);
            var treasuryCashFlows = result.Where(x => x.CurrencyId == eventData.CurrencyId);
            if (treasuryCashFlows.Any())
            {
                //int maxDeletedId = treasuryCashFlows.Max(x => x.Id);

                //var firstCashFlowDate = treasuryCashFlows.First().Date;
                //var treasuryId = treasuryCashFlows.First().TreasuryId;

                foreach (var treasuryCashFlow in treasuryCashFlows)
                {
                    await _treasuryCashFlowRepository.DeleteAsync(treasuryCashFlow.Id);
                    Thread.Sleep(200);
                }

                //var nextTreasuryCashFlows = _treasuryCashFlowRepository.GetAllList(x => x.Id > maxDeletedId && x.CurrencyId == eventData.CurrencyId);
                //if (nextTreasuryCashFlows.Any())
                //{
                //    var previousCashFlow = _treasuryCashFlowRepository.GetAllList(x => x.Id < maxDeletedId && x.CurrencyId == eventData.CurrencyId)
                //    .OrderByDescending(x => x.Id).FirstOrDefault();
                //    var previousBalance = GetPreviousBalance(treasuryId, eventData.CurrencyId.Value, firstCashFlowDate);

                //    await UpdateNextCashFlow(nextTreasuryCashFlows, previousBalance);
                //}
            }
        }
        public IList<TreasuryCashFlow> Get(int treasuryId)
        {
            var treasuryCashFlows = _treasuryCashFlowRepository
                .GetAllIncluding(
                cu => cu.Currency,
                m => m.CashFlowMatching,
                tr => tr.Treasury).Where(tcf => tcf.TreasuryId == treasuryId);
            return treasuryCashFlows.ToList();
        }
        public IList<TreasuryCashFlow> Get(int treasuryId, int currencyId, DateTime fromDate, DateTime toDate)
        {
            var treasuryCashFlows = _treasuryCashFlowRepository.GetAllIncluding(
                cu => cu.Currency,
                m => m.CashFlowMatching,
                tr => tr.Treasury)
                .Where(x =>
                x.TreasuryId == treasuryId &&
                x.CurrencyId == currencyId &&
                x.Date >= fromDate &&
                x.Date <= toDate).OrderBy(x => x.Date)
                .ThenBy(x => x.Id).ToList();

            return treasuryCashFlows.ToList();
        }
        public async Task<TreasuryCashFlow> GetLastAsync(int treasuryId, int currencyId)
        {
            TreasuryCashFlow treasuryCashFlow = null;
            var treasuryCashFlows = await _treasuryCashFlowRepository
                .GetAllListAsync(x => x.TreasuryId == treasuryId && x.CurrencyId == currencyId);
            if (treasuryCashFlows.Any())
            {
                treasuryCashFlow = treasuryCashFlows.OrderByDescending(x => x.Id).FirstOrDefault();
            }
            return treasuryCashFlow;
        }
        public async Task<TreasuryCashFlow> GetByTransctionInfo(int transactionId, TransactionType transactionType)
        {
            return await _treasuryCashFlowRepository.FirstOrDefaultAsync(x =>
                x.TransactionId == transactionId &&
                x.TransactionType == transactionType
            );
        }
        public async Task<TreasuryCashFlow> GetByTransctionInfo(int transactionId, TransactionType transactionType, string type)
        {
            return await _treasuryCashFlowRepository.FirstOrDefaultAsync(x =>
                x.TransactionId == transactionId &&
                x.TransactionType == transactionType &&
                x.Type == type
            );
        }
        public async Task<IList<TreasuryCashFlow>> GetNextAsync(TreasuryCashFlow currenctCashFlow)
        {
            var list = await _treasuryCashFlowRepository.GetAllListAsync(x =>
            x.CurrencyId == currenctCashFlow.CurrencyId &&
            x.TreasuryId == currenctCashFlow.TreasuryId &&
            x.Date > currenctCashFlow.Date);

            return list;
        }
        public double GetPreviousBalance(int treasuryId, int currencyId, DateTime date)
        {
            double balance = 0.0;
            var treasuryCashFlows = _treasuryCashFlowRepository
                .GetAllList(x => x.TreasuryId == treasuryId && x.CurrencyId == currencyId && x.Date < date);

            if (treasuryCashFlows.Any())
            {
                balance = treasuryCashFlows.OrderByDescending(x => x.Date).FirstOrDefault().CurrentBalance;
            }
            else
            {
                var treasuryBalance = _treasuryBalanceManager.GetByTreasuryIdAndCurrency(treasuryId, currencyId);
                balance = treasuryBalance != null ? treasuryBalance.InitilBalance : 0.0;
            }

            return balance;
        }
        public async Task<IList<TreasuryCashFlow>> GetAllByTransctionInfo(int transactionId, TransactionType transactionType, string type)
        {
            if (!string.IsNullOrEmpty(type))
            {
                return await _treasuryCashFlowRepository.GetAllListAsync(x =>
                x.TransactionId == transactionId &&
                x.TransactionType == transactionType &&
                x.Type == type
                );
            }

            return await _treasuryCashFlowRepository.GetAllListAsync(x =>
                x.TransactionId == transactionId &&
                x.TransactionType == transactionType
            );
        }
        public async Task MatchAsync(CashFlowMatching cashFlowMacting, int treasuryId, int currencyId, DateTime fromDate, DateTime toDate)
        {
            var treasuryCashFlows = await _treasuryCashFlowRepository.GetAllListAsync(x =>
                x.TreasuryId == treasuryId &&
                x.CurrencyId == currencyId &&
                x.Date >= fromDate &&
                x.Date <= toDate);
            if (treasuryCashFlows.Any())
            {
                var createdCashFlowMacting = await _cashFlowMatchingManager.CreateAsync(cashFlowMacting);

                foreach (var treasuryCashFlow in treasuryCashFlows)
                {
                    treasuryCashFlow.CashFlowMatchingId = createdCashFlowMacting.Id;
                    await _treasuryCashFlowRepository.UpdateAsync(treasuryCashFlow);
                }
            }
        }
        public double GetLastBalance(int treasuryId, int currencyId, DateTime date)
        {
            var treasuryBalance = _treasuryBalanceManager.GetByTreasuryIdAndCurrency(treasuryId, currencyId);
            double balance = treasuryBalance.InitilBalance;

            var treasuryCashFlows = _treasuryCashFlowRepository
                .GetAllList(x => x.TreasuryId == treasuryId && x.CurrencyId == currencyId && x.Date <= date);

            if (treasuryCashFlows.Any())
            {
                balance += treasuryCashFlows.Sum(x => x.CurrentBalance);
            }

            return balance;
        }
        public double GetPreviousBalance2(int treasuryId, int currencyId, DateTime date)
        {
            var treasuryBalance = _treasuryBalanceManager.GetByTreasuryIdAndCurrency(treasuryId, currencyId);
            double previousBalance = treasuryBalance.InitilBalance;

            var treasuryCashFlows = _treasuryCashFlowRepository
                .GetAllList(x => x.TreasuryId == treasuryId && x.CurrencyId == currencyId && x.Date < date);

            if (treasuryCashFlows.Any())
            {
                previousBalance += treasuryCashFlows.Sum(x => x.CurrentBalance);
            }
            
            return previousBalance;
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
                        //data[i].CurrentBalance = data[i].PreviousBalance + data[i].Amount + data[i].Commission;
                    }
                    else
                    {
                        data[i].PreviousBalance = data[i - 1].CurrentBalance;
                        data[i].CurrentBalance = data[i].PreviousBalance + data[i].Amount;
                        //data[i].CurrentBalance = data[i].PreviousBalance + data[i].Amount + data[i].Commission;
                    }

                    await UpdateAsync(data[i]);
                }
            }

        }
    }
}