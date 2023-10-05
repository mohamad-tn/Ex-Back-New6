using Abp.Domain.Services;
using Bwr.Exchange.CashFlows.CashFlowMatchings;
using Bwr.Exchange.CashFlows.CompanyCashFlows.Events;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bwr.Exchange.CashFlows.CompanyCashFlows.Services
{
    public interface ICompanyCashFlowManager : IDomainService
    {
        Task CreateAsync(CompanyCashFlow input);
        Task<CompanyCashFlow> CreateAsync(CompanyCashFlowCreatedEventData eventData);
        Task UpdateAsync(CompanyCashFlow input);
        Task DeleteAsync(CompanyCashFlow input);
        Task DeleteAsync(CompanyCashFlowDeletedMultiEventData eventData);
        void Delete(int transactionId, TransactionType transactionType);
        Task MatchAsync(CashFlowMatching cashFlowMacting, int companyId, int currencyId, DateTime fromDate, DateTime toDate);
        Task MatchAsync(CashFlowMatching cashFlowMacting, int id);
        Task MatchAsync(CashFlowMatching cashFlowMacting, IList<Dictionary<string, object>> items);
        Task<CompanyCashFlow> GetLastAsync(int companyId, int currencyId);
        Task<CompanyCashFlow> GetByTransctionInfo(int transactionId, int transactionType);
        Task<CompanyCashFlow> GetByTransctionInfo(int transactionId, TransactionType transactionType, string type);
        Task<IList<CompanyCashFlow>> GetAllByTransctionInfo(int transactionId, TransactionType transactionType, string type);
        Task<IList<CompanyCashFlow>> GetNextAsync(CompanyCashFlow currenctCashFlow);
        IList<CompanyCashFlow> Get(int companyId);
        IList<CompanyCashFlow> Get(int companyId, int currencyId, DateTime fromDate, DateTime toDate);
        double GetPreviousBalance(int companyId, int currencyId, DateTime date);
        double GetPreviousBalance2(int companyId, int currencyId, DateTime date);
        double GetLastBalance(int companyId, int currencyId, DateTime toDate);
        bool CheckIsActiveToday(int id, DateTime toDate);
        bool CheckIsIfMatching(int companyId);
    }
}
