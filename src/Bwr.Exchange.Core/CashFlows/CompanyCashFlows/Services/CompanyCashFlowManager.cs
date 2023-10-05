using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Bwr.Exchange.CashFlows.CashFlowMatchings;
using Bwr.Exchange.CashFlows.CashFlowMatchings.Services;
using Bwr.Exchange.CashFlows.CompanyCashFlows.Events;
using Bwr.Exchange.Settings.Companies.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bwr.Exchange.CashFlows.CompanyCashFlows.Services
{
    public class CompanyCashFlowManager : ICompanyCashFlowManager
    {
        private readonly IRepository<CompanyCashFlow> _companyCashFlowRepository;
        private readonly ICompanyManager _companyManager;
        private readonly ICashFlowMatchingManager _cashFlowMatchingManager;

        public CompanyCashFlowManager(
            IRepository<CompanyCashFlow> companyCashFlowRepository,
            ICompanyManager companyManager,
            ICashFlowMatchingManager cashFlowMatchingManager
            )
        {
            _companyCashFlowRepository = companyCashFlowRepository;
            _companyManager = companyManager;
            _cashFlowMatchingManager = cashFlowMatchingManager;
        }

        public async Task CreateAsync(CompanyCashFlow input)
        {
            await _companyCashFlowRepository.InsertAsync(input);
        }

        public async Task<CompanyCashFlow> CreateAsync(CompanyCashFlowCreatedEventData eventData)
        {
            //if (eventData.PreviousBalance == null || eventData.PreviousBalance == 0)
            //{
            //    eventData.PreviousBalance = GetPreviousBalance(eventData.CompanyId.Value, eventData.CurrencyId.Value, eventData.Date);
            //}

            var companyCashFlow = new CompanyCashFlow()
            {
                CompanyId = eventData.CompanyId.Value,
                CurrencyId = eventData.CurrencyId.Value,
                Date = eventData.Date,
                Amount = eventData.Amount,
                Note = eventData.Note,
                Type = eventData.Type,
                //PreviousBalance = eventData.PreviousBalance.Value,
                CurrentBalance = eventData.Amount + eventData.CompanyCommission + eventData.Commission,
                TransactionId = eventData.TransactionId,
                TransactionType = eventData.TransactionType,
                Commission = eventData.Commission,
                CompanyCommission = eventData.CompanyCommission,
                Destination = eventData.Destination,
                Sender = eventData.Sender,
                Beneficiary = eventData.Beneficiary
            };

            var createdCashFlow = await _companyCashFlowRepository.InsertAsync(companyCashFlow);

            //await UpdateNextCashFlow(companyCashFlow);

            return createdCashFlow;
        }

        public async Task UpdateAsync(CompanyCashFlow input)
        {
            await _companyCashFlowRepository.UpdateAsync(input);
        }

        public async Task DeleteAsync(CompanyCashFlow input)
        {
            await _companyCashFlowRepository.DeleteAsync(input);
        }

        public async Task DeleteAsync(CompanyCashFlowDeletedMultiEventData eventData)
        {
            var companyCashFlows = await
               GetAllByTransctionInfo(eventData.TransactionId, eventData.TransactionType, eventData.Type);

            if (companyCashFlows.Any())
            {
                //int maxDeletedId = companyCashFlows.Max(x => x.Id);

                foreach (var companyCashFlow in companyCashFlows)
                {
                    await _companyCashFlowRepository.DeleteAsync(companyCashFlow.Id);
                    Thread.Sleep(200);
                }

                //var firstCashFlow = companyCashFlows.First();
                //var lastCashFlow = companyCashFlows.Last();


                ////nextCompanyCashFlows = await GetNextAsync(lastCashFlow);
                //var nextCompanyCashFlows = _companyCashFlowRepository.GetAllList(x => x.Id > maxDeletedId);
                //if (nextCompanyCashFlows.Any())
                //{
                //    var previousCashFlow = _companyCashFlowRepository.GetAllList(x => x.Id < maxDeletedId)
                //    .OrderByDescending(x => x.Id).FirstOrDefault();
                //    var previousBalance = GetPreviousBalance(firstCashFlow.CompanyId, firstCashFlow.CurrencyId, firstCashFlow.Date);

                //    await UpdateNextCashFlow(nextCompanyCashFlows, previousBalance);
                //}
            }
        }

        public IList<CompanyCashFlow> Get(int companyId)
        {
            var companyCashFlows = _companyCashFlowRepository
                .GetAllIncluding(
                cu => cu.Currency,
                m => m.CashFlowMatching,
                tr => tr.Company)
                .Where(x => x.CompanyId == companyId);
            return companyCashFlows.ToList();
        }

        public async Task<CompanyCashFlow> GetLastAsync(int companyId, int currencyId)
        {
            CompanyCashFlow companyCashFlow = null;
            var companyCashFlows = await _companyCashFlowRepository
                .GetAllListAsync(x => x.CompanyId == companyId && x.CurrencyId == currencyId);
            if (companyCashFlows.Any())
            {
                companyCashFlow = companyCashFlows.OrderByDescending(x => x.Date).FirstOrDefault();
            }
            return companyCashFlow;
        }

        public IList<CompanyCashFlow> Get(int companyId, int currencyId, DateTime fromDate, DateTime toDate)
        {
            var companyCashFlows = _companyCashFlowRepository.GetAllIncluding(
                cu => cu.Currency,
                m => m.CashFlowMatching,
                tr => tr.Company)
                .Where(x =>
                x.CompanyId == companyId &&
                x.CurrencyId == currencyId &&
                x.Date >= fromDate &&
                x.Date <= toDate).OrderBy(x => x.Date)
                .ThenBy(x => x.Id).ToList();

            return companyCashFlows.ToList();
        }

        public double GetPreviousBalance(int companyId, int currencyId, DateTime date)
        {
            double balance = 0.0;
            var companyCashFlows = _companyCashFlowRepository
                .GetAllList(x => x.CompanyId == companyId && x.CurrencyId == currencyId && x.Date < date);

            if (companyCashFlows.Any())
            {
                balance = companyCashFlows.OrderByDescending(x => x.Date).FirstOrDefault().CurrentBalance;
            }
            else
            {
                var companyBalance = _companyManager.GetCompanyBalance(companyId, currencyId);
                balance = companyBalance != null ? companyBalance.Balance : 0.0;
            }

            return balance;
        }

        public double GetPreviousBalance2(int companyId, int currencyId, DateTime date)
        {
            double balance = 0.0;
            var companyBalance = _companyManager.GetCompanyBalance(companyId, currencyId);
            if (companyBalance != null)
            {
                balance = companyBalance.Balance;

                var companyCashFlows = _companyCashFlowRepository
                .GetAllList(x => x.CompanyId == companyId && x.CurrencyId == currencyId && x.Date < date);

                if (companyCashFlows.Any())
                {
                    balance += companyCashFlows.Sum(x => x.CurrentBalance);
                }

            }

            return balance;
        }

        public async Task<CompanyCashFlow> GetByTransctionInfo(int transactionId, int transactionType)
        {
            return await _companyCashFlowRepository.FirstOrDefaultAsync(x =>
                x.TransactionId == transactionId &&
                x.TransactionType == (TransactionType)transactionType
            );
        }

        public async Task<CompanyCashFlow> GetByTransctionInfo(int transactionId, TransactionType transactionType, string type)
        {
            return await _companyCashFlowRepository.FirstOrDefaultAsync(x =>
                x.TransactionId == transactionId &&
                x.TransactionType == transactionType &&
                x.Type == type
            );
        }

        public async Task<IList<CompanyCashFlow>> GetAllByTransctionInfo(int transactionId, TransactionType transactionType, string type)
        {
            if (!string.IsNullOrEmpty(type))
            {
                return await _companyCashFlowRepository.GetAllListAsync(x =>
                    x.TransactionId == transactionId &&
                    x.TransactionType == transactionType &&
                    x.Type == type
                );
            }
            return await _companyCashFlowRepository.GetAllListAsync(x =>
            x.TransactionId == transactionId &&
            x.TransactionType == transactionType
            );
        }
        public async Task<IList<CompanyCashFlow>> GetNextAsync(CompanyCashFlow currenctCashFlow)
        {
            var list = await _companyCashFlowRepository.GetAllListAsync(x =>
            x.CurrencyId == currenctCashFlow.CurrencyId &&
            x.CompanyId == currenctCashFlow.CompanyId &&
            x.Date > currenctCashFlow.Date);

            return list;
        }

        public double GetLastBalance(int companyId, int currencyId, DateTime toDate)
        {
            double balance = 0.0;
            var companyBalance = _companyManager.GetCompanyBalance(companyId, currencyId);
            if (companyBalance != null)
            {
                balance = companyBalance.Balance;

                var companyCashFlows = _companyCashFlowRepository
                .GetAllList(x => x.CompanyId == companyId && x.CurrencyId == currencyId && x.Date <= toDate);

                if (companyCashFlows.Any())
                {
                    balance += companyCashFlows.Sum(x => x.CurrentBalance);
                }

            }

            return balance;
        }

        public async Task MatchAsync(CashFlowMatching cashFlowMacting, int companyId, int currencyId, DateTime fromDate, DateTime toDate)
        {
            var companyCashFlows = await _companyCashFlowRepository.GetAllListAsync(x =>
                x.CompanyId == companyId &&
                x.CurrencyId == currencyId &&
                x.Date >= fromDate &&
                x.Date <= toDate);
            if (companyCashFlows.Any())
            {
                var createdCashFlowMacting = await _cashFlowMatchingManager.CreateAsync(cashFlowMacting);

                foreach (var companyCashFlow in companyCashFlows)
                {
                    companyCashFlow.CashFlowMatchingId = createdCashFlowMacting.Id;
                    await _companyCashFlowRepository.UpdateAsync(companyCashFlow);
                }
            }
        }

        public async Task MatchAsync(CashFlowMatching cashFlowMacting, int id)
        {
            var companyCashFlow = await _companyCashFlowRepository.GetAsync(id);
            if (companyCashFlow != null)
            {
                var createdCashFlowMacting = await _cashFlowMatchingManager.CreateAsync(cashFlowMacting);

                companyCashFlow.CashFlowMatchingId = createdCashFlowMacting.Id;
                companyCashFlow.Matched = true;
                await _companyCashFlowRepository.UpdateAsync(companyCashFlow);
            }
        }

        public async Task MatchAsync(CashFlowMatching cashFlowMacting, IList<Dictionary<string, object>> items)
        {
            var createdCashFlowMacting = await _cashFlowMatchingManager.CreateAsync(cashFlowMacting);

            foreach (var item in items)
            {
                var id = int.Parse(item["Id"].ToString());
                var matched = bool.Parse(item["Matched"].ToString());

                var companyCashFlow = await _companyCashFlowRepository.GetAsync(id);
                if (companyCashFlow != null)
                {
                    companyCashFlow.CashFlowMatchingId = createdCashFlowMacting.Id;
                    companyCashFlow.Matched = matched;
                    await _companyCashFlowRepository.UpdateAsync(companyCashFlow);
                }
            }
        }

        public bool CheckIsActiveToday(int companyId, DateTime toDate)
        {
            var companyCashFlow = _companyCashFlowRepository
                .FirstOrDefault(x => x.CompanyId == companyId && x.Date.Date == toDate.Date);
            return companyCashFlow != null;
        }

        public bool CheckIsIfMatching(int companyId)
        {
            var companyCashFlow = _companyCashFlowRepository
                .FirstOrDefault(x =>
                x.CompanyId == companyId &&
                x.Matched == true &&
                x.Date.Date == DateTime.Now.Date);

            return companyCashFlow != null;
        }

        private async Task UpdateNextCashFlow(CompanyCashFlow current)
        {
            var companyCashFlows = await GetNextAsync(current);
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

                    await UpdateAsync(data[i]);
                }
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

                    await UpdateAsync(data[i]);
                }
            }

        }

        public void Delete(int transactionId, TransactionType transactionType)
        {
            var cashFlows = _companyCashFlowRepository.GetAllList(x => x.TransactionId == transactionId && x.TransactionType == transactionType);
            foreach (var cashFlow in cashFlows)
            {
                _companyCashFlowRepository.Delete(cashFlow.Id);
                Thread.Sleep(200);
            }
        }
    }
}
