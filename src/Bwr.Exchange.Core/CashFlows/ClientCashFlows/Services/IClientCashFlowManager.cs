using Abp.Domain.Services;
using Bwr.Exchange.CashFlows.CashFlowMatchings;
using Bwr.Exchange.CashFlows.ClientCashFlows.Events;
using Bwr.Exchange.Settings.Clients;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bwr.Exchange.CashFlows.ClientCashFlows.Services
{
    public interface IClientCashFlowManager : IDomainService
    {
        Task CreateAsync(ClientCashFlow input);
        Task<ClientCashFlow> CreateAsync(ClientCashFlowCreatedEventData eventData);
        Task DeleteAsync(ClientCashFlow input);
        Task DeleteAsync(ClientCashFlowDeletedMultiEventData eventData);
        Task UpdateAsync(ClientCashFlow input);
        Task MatchAsync(CashFlowMatching cashFlowMacting, int clientId, int currencyId, DateTime fromDate, DateTime toDate);
        Task MatchAsync(CashFlowMatching cashFlowMacting, int id);
        Task MatchAsync(CashFlowMatching cashFlowMacting, IList<Dictionary<string, object>> items);
        Task<IList<ClientCashFlow>> GetNextAsync(ClientCashFlow currenctCashFlow);
        Task<ClientCashFlow> GetLastAsync(int clientId);
        Task<ClientCashFlow> GetLastAsync(int clientId, int currencyId);
        Task<ClientCashFlow> GetLastAsync(int clientId, int currencyId, DateTime toDate);
        Task<ClientCashFlow> GetByTransctionInfo(int transactionId, int transactionType);
        Task<ClientCashFlow> GetByTransctionInfo(int transactionId, TransactionType transactionType, string type);
        Task<IList<ClientCashFlow>> GetAllByTransctionInfo(int transactionId, TransactionType transactionType, string type);
        IList<ClientCashFlow> Get(int client);
        Task<IList<Client>> GetForDefaultersAsync(int days);
        IList<ClientCashFlow> Get(int clientId, int currencyId, DateTime fromDate, DateTime toDate);
        double GetPreviousBalance(int clientId, int currencyId, DateTime date);
        double GetPreviousBalance2(int clientId, int currencyId, DateTime date);
        double GetLastBalance(int clientId, int currencyId, DateTime toDate);
        bool CheckIsActiveToday(int id, DateTime toDate);
        bool CheckIsIfMatching(int clientId);
        Task<ClientCashFlow> GetLastByTypeAsync(int clientId, string type);
    }
}
