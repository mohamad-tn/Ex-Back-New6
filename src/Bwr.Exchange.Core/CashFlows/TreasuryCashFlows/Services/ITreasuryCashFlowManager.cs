using Abp.Domain.Services;
using Bwr.Exchange.CashFlows.CashFlowMatchings;
using Bwr.Exchange.CashFlows.TreasuryCashFlows.Events.TreasuryCashFlowDeletedMulti;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bwr.Exchange.CashFlows.TreasuryCashFlows.Services
{
    public interface ITreasuryCashFlowManager : IDomainService
    {
        Task CreateAsync(TreasuryCashFlow input);
        Task UpdateAsync(TreasuryCashFlow input);
        Task DeleteAsync(TreasuryCashFlow input);
        Task DeleteAsync(TreasuryCashFlowDeletedMultiEventData eventData);
        Task MatchAsync(CashFlowMatching cashFlowMacting, int treasuryId, int currencyId, DateTime fromDate, DateTime toDate);
        Task<TreasuryCashFlow> GetLastAsync(int treasuryId, int currencyId);
        double GetPreviousBalance(int treasuryId, int currencyId, DateTime date);
        double GetPreviousBalance2(int treasuryId, int currencyId, DateTime date);
        IList<TreasuryCashFlow> Get(int treasuryId);
        IList<TreasuryCashFlow> Get(int treasuryId, int currencyId, DateTime fromDate, DateTime toDate);
        Task<TreasuryCashFlow> GetByTransctionInfo(int transactionId, TransactionType transactionType);
        Task<TreasuryCashFlow> GetByTransctionInfo(int transactionId, TransactionType transactionType, string type);
        Task<IList<TreasuryCashFlow>> GetNextAsync(TreasuryCashFlow currenctCashFlow);
        double GetLastBalance(int treasuryId, int currencyId, DateTime date);
    }
}
