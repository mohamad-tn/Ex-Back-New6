using Abp.Domain.Repositories;
using Bwr.Exchange.CashFlows.CashFlowMatchings;
using Bwr.Exchange.CashFlows.CashFlowMatchings.Services;
using Bwr.Exchange.CashFlows.ClientCashFlows.Events;
using Bwr.Exchange.Settings.Clients;
using Bwr.Exchange.Settings.Clients.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bwr.Exchange.CashFlows.ClientCashFlows.Services
{
    public class ClientCashFlowManager : IClientCashFlowManager
    {
        private readonly IRepository<ClientCashFlow> _clientCashFlowRepository;
        private readonly IClientManager _clientManager;
        private readonly ICashFlowMatchingManager _cashFlowMatchingManager;

        public ClientCashFlowManager(
            IRepository<ClientCashFlow> clientCashFlowRepository,
            IClientManager clientManager,
            ICashFlowMatchingManager cashFlowMatchingManager)
        {
            _clientCashFlowRepository = clientCashFlowRepository;
            _clientManager = clientManager;
            _cashFlowMatchingManager = cashFlowMatchingManager;
        }

        public async Task CreateAsync(ClientCashFlow input)
        {
            await _clientCashFlowRepository.InsertAsync(input);
        }

        public async Task<ClientCashFlow> CreateAsync(ClientCashFlowCreatedEventData eventData)
        {
            //if (eventData.PreviousBalance == null || eventData.PreviousBalance == 0)
            //{
            //    eventData.PreviousBalance = GetPreviousBalance(eventData.ClientId.Value, eventData.CurrencyId.Value, eventData.Date);
            //}

            var clientCashFlow = new ClientCashFlow()
            {
                ClientId = eventData.ClientId.Value,
                CurrencyId = eventData.CurrencyId.Value,
                Date = eventData.Date,
                Amount = eventData.Amount,
                Note = eventData.Note,
                Type = eventData.Type,
                //PreviousBalance = eventData.PreviousBalance.Value,
                CurrentBalance =  eventData.Amount + eventData.ClientCommission + eventData.Commission,
                TransactionId = eventData.TransactionId,
                TransactionType = eventData.TransactionType,
                Commission = eventData.Commission,
                ClientCommission = eventData.ClientCommission,
                Destination = eventData.Destination,
                Sender = eventData.Sender,
                Beneficiary = eventData.Beneficiary
            };

            var createdCashFlow = await _clientCashFlowRepository.InsertAsync(clientCashFlow);

            //await UpdateNextCashFlow(clientCashFlow);

            return createdCashFlow;
        }

        public async Task DeleteAsync(ClientCashFlow input)
        {
            await _clientCashFlowRepository.DeleteAsync(input);
        }

        public async Task DeleteAsync(ClientCashFlowDeletedMultiEventData eventData)
        {
            var clientCashFlows = await
               GetAllByTransctionInfo(eventData.TransactionId, eventData.TransactionType, eventData.Type);
            if (clientCashFlows.Any())
            {
                //int maxDeletedId = clientCashFlows.Max(x => x.Id);

                foreach (var clientCashFlow in clientCashFlows)
                {
                    _clientCashFlowRepository.Delete(clientCashFlow.Id);
                    Thread.Sleep(200);
                }

                //var firstCashFlow = clientCashFlows.First();
                //var lastCashFlow = clientCashFlows.Last();


                ////nextClientCashFlows = await GetNextAsync(lastCashFlow);
                //var nextClientCashFlows = _clientCashFlowRepository.GetAllList(x => x.Id > maxDeletedId);
                //if (nextClientCashFlows.Any())
                //{
                //    var previousCashFlow = _clientCashFlowRepository.GetAllList(x => x.Id < maxDeletedId)
                //    .OrderByDescending(x => x.Id).FirstOrDefault();
                //    var previousBalance = GetPreviousBalance(firstCashFlow.ClientId, firstCashFlow.CurrencyId, firstCashFlow.Date);

                //    await UpdateNextCashFlow(nextClientCashFlows, previousBalance);
                //}
            }
        }

        public IList<ClientCashFlow> Get(int clientId)
        {
            var clientCashFlows = _clientCashFlowRepository
                .GetAllIncluding(
                cu => cu.Currency,
                m => m.CashFlowMatching,
                tr => tr.Client)
                .Where(x => x.ClientId == clientId);
            return clientCashFlows.ToList();
        }

        public IList<ClientCashFlow> Get(int clientId, int currencyId, DateTime fromDate, DateTime toDate)
        {
            var clientCashFlows = _clientCashFlowRepository.GetAllIncluding(
                cu => cu.Currency,
                m => m.CashFlowMatching,
                tr => tr.Client)
                .Where(x =>
                x.ClientId == clientId &&
                x.CurrencyId == currencyId &&
                x.Date >= fromDate &&
                x.Date <= toDate);

            return clientCashFlows.OrderBy(x => x.Date).ThenBy(x => x.Id).ToList();
        }

        public async Task<ClientCashFlow> GetByTransctionInfo(int transactionId, int transactionType)
        {
            return await _clientCashFlowRepository.FirstOrDefaultAsync(x =>
                x.TransactionId == transactionId &&
                x.TransactionType == (TransactionType)transactionType
            );
        }

        public async Task<ClientCashFlow> GetLastAsync(int clientId)
        {
            ClientCashFlow clientCashFlow = null;
            var clientCashFlows = await _clientCashFlowRepository
                .GetAllListAsync(x => x.ClientId == clientId);
            if (clientCashFlows.Any())
            {
                clientCashFlow = clientCashFlows.OrderByDescending(x => x.Date).FirstOrDefault();
            }
            return clientCashFlow;
        }

        public async Task<ClientCashFlow> GetLastAsync(int clientId, int currencyId)
        {
            ClientCashFlow clientCashFlow = null;
            var clientCashFlows = await _clientCashFlowRepository
                .GetAllListAsync(x => x.ClientId == clientId && x.CurrencyId == currencyId);
            if (clientCashFlows.Any())
            {
                clientCashFlow = clientCashFlows.OrderByDescending(x => x.Date).FirstOrDefault();
            }
            return clientCashFlow;
        }

        public async Task<ClientCashFlow> GetLastAsync(int clientId, int currencyId, DateTime toDate)
        {
            ClientCashFlow clientCashFlow = null;

            var clientCashFlows = await _clientCashFlowRepository
                .GetAllListAsync(x => x.ClientId == clientId && x.CurrencyId == currencyId && x.Date <= toDate);
            if (clientCashFlows.Any())
            {
                clientCashFlows.OrderByDescending(x => x.Date).ThenByDescending(x => x.Id).FirstOrDefault();
            }

            return clientCashFlow;
        }

        public async Task<IList<ClientCashFlow>> GetNextAsync(ClientCashFlow currenctCashFlow)
        {
            var list = await _clientCashFlowRepository.GetAllListAsync(x =>
                x.CurrencyId == currenctCashFlow.CurrencyId &&
                x.ClientId == currenctCashFlow.ClientId &&
                x.Date > currenctCashFlow.Date);

            return list;
        }

        public double GetPreviousBalance(int clientId, int currencyId, DateTime date)
        {
            double balance = 0.0;
            var clientCashFlows = _clientCashFlowRepository
                .GetAllList(x => x.ClientId == clientId && x.CurrencyId == currencyId && x.Date < date);

            if (clientCashFlows.Any())
            {
                balance = clientCashFlows.OrderByDescending(x => x.Date).FirstOrDefault().CurrentBalance;
            }
            else
            {
                var clientBalance = _clientManager.GetClientBalance(clientId, currencyId);
                balance = clientBalance != null ? clientBalance.Balance : 0.0;
            }

            return balance;
        }

        public double GetPreviousBalance2(int clientId, int currencyId, DateTime date)
        {
            double balance = 0.0;
            var clientBalance = _clientManager.GetClientBalance(clientId, currencyId);
            if(clientBalance != null)
            {
                balance = clientBalance.Balance;

                var clientCashFlows = _clientCashFlowRepository
                .GetAllList(x => x.ClientId == clientId && x.CurrencyId == currencyId && x.Date < date);

                if (clientCashFlows.Any())
                {
                    balance += clientCashFlows.Sum(x => x.CurrentBalance);
                }
                
            }

            return balance;
        }

        public async Task UpdateAsync(ClientCashFlow input)
        {
            await _clientCashFlowRepository.UpdateAsync(input);
        }

        public double GetLastBalance(int clientId, int currencyId, DateTime toDate)
        {
            double balance = 0.0;
            var clientBalance = _clientManager.GetClientBalance(clientId, currencyId);
            if (clientBalance != null)
            {
                balance = clientBalance.Balance;

                var clientCashFlows = _clientCashFlowRepository
                .GetAllList(x => x.ClientId == clientId && x.CurrencyId == currencyId && x.Date <= toDate);

                if (clientCashFlows.Any())
                {
                    balance += clientCashFlows.Sum(x => x.CurrentBalance);
                }

            }

            return balance;
        }

        public async Task<IList<Client>> GetForDefaultersAsync(int days)
        {
            var toDate = DateTime.Now.AddDays(-1 * days);
            toDate = new DateTime(toDate.Year, toDate.Month, toDate.Day, 23, 59, 59);
            var clients = _clientManager.GetAll();

            var clientCashFlows = await _clientCashFlowRepository
                .GetAllListAsync(x => x.Date >= toDate);

            if (clientCashFlows.Any())
            {
                var clientIds = clientCashFlows.Select(x => x.ClientId).ToArray();
                clients = clients.Where(x => !clientIds.Contains(x.Id)).ToList();
            }

            return clients.ToList();
        }

        public async Task MatchAsync(CashFlowMatching cashFlowMacting, int clientId, int currencyId, DateTime fromDate, DateTime toDate)
        {
            var clientCashFlows = await _clientCashFlowRepository.GetAllListAsync(x =>
                x.ClientId == clientId &&
                x.CurrencyId == currencyId &&
                x.Date >= fromDate &&
                x.Date <= toDate);
            if (clientCashFlows.Any())
            {
                var createdCashFlowMacting = await _cashFlowMatchingManager.CreateAsync(cashFlowMacting);

                foreach (var clientCashFlow in clientCashFlows)
                {
                    clientCashFlow.CashFlowMatchingId = createdCashFlowMacting.Id;
                    clientCashFlow.Matched = true;
                    await _clientCashFlowRepository.UpdateAsync(clientCashFlow);
                }
            }
        }

        public async Task MatchAsync(CashFlowMatching cashFlowMacting, int id)
        {
            var clientCashFlow = await _clientCashFlowRepository.GetAsync(id);
            if (clientCashFlow != null)
            {
                var createdCashFlowMacting = await _cashFlowMatchingManager.CreateAsync(cashFlowMacting);

                clientCashFlow.CashFlowMatchingId = createdCashFlowMacting.Id;
                clientCashFlow.Matched = true;
                await _clientCashFlowRepository.UpdateAsync(clientCashFlow);
            }
        }

        public async Task<ClientCashFlow> GetByTransctionInfo(int transactionId, TransactionType transactionType, string type)
        {
            return await _clientCashFlowRepository.FirstOrDefaultAsync(x =>
                x.TransactionId == transactionId &&
                x.TransactionType == transactionType &&
                x.Type == type
            );
        }

        public bool CheckIsActiveToday(int clientId, DateTime toDate)
        {
            var clientCashFlow = _clientCashFlowRepository
                .FirstOrDefault(x => x.ClientId == clientId && x.Date.Date == toDate.Date);
            return clientCashFlow != null;
        }

        public bool CheckIsIfMatching(int clientId)
        {
            var clientCashFlow = _clientCashFlowRepository
                .FirstOrDefault(x =>
                x.ClientId == clientId &&
                x.Matched == true &&
                x.Date.Date == DateTime.Now.Date);

            return clientCashFlow != null;
        }

        private async Task UpdateNextCashFlow(ClientCashFlow current)
        {
            var clientCashFlows = await GetNextAsync(current);
            if (clientCashFlows.Any())
            {
                var data = clientCashFlows.OrderBy(x => x.Date).ToList();
                for (int i = 0; i < clientCashFlows.Count(); i++)
                {
                    if (i == 0)
                    {
                        //if (data[i].PreviousBalance == current.CurrentBalance) // هذا يعني انه لايوجد تغيير وبالتالي نخرج من الحلقة
                        //    break;

                        data[i].PreviousBalance = current.CurrentBalance;
                        data[i].CurrentBalance = data[i].PreviousBalance + data[i].Amount + data[i].ClientCommission + data[i].Commission;
                    }
                    else
                    {
                        data[i].PreviousBalance = data[i - 1].CurrentBalance;
                        data[i].CurrentBalance = data[i].PreviousBalance + data[i].Amount + data[i].ClientCommission + data[i].Commission;
                    }

                    await UpdateAsync(data[i]);
                }
            }

        }

        private async Task UpdateNextCashFlow(IList<ClientCashFlow> clientCashFlows, double previousBalance)
        {
            if (clientCashFlows.Any())
            {
                var data = clientCashFlows.OrderBy(x => x.Date).ToList();
                for (int i = 0; i < clientCashFlows.Count(); i++)
                {
                    if (i == 0)
                    {
                        data[i].PreviousBalance = previousBalance;
                        data[i].CurrentBalance = data[i].PreviousBalance + data[i].Amount + data[i].ClientCommission + data[i].Commission;
                    }
                    else
                    {
                        data[i].PreviousBalance = data[i - 1].CurrentBalance;
                        data[i].CurrentBalance = data[i].PreviousBalance + data[i].Amount + data[i].ClientCommission + data[i].Commission;
                    }

                    await UpdateAsync(data[i]);
                }
            }

        }
        public async Task<IList<ClientCashFlow>> GetAllByTransctionInfo(int transactionId, TransactionType transactionType, string type)
        {
            if (!string.IsNullOrEmpty(type))
            {
                return await _clientCashFlowRepository.GetAllListAsync(x =>
                x.TransactionId == transactionId &&
                x.TransactionType == transactionType &&
                x.Type == type
                );
            }

            return await _clientCashFlowRepository.GetAllListAsync(x =>
                x.TransactionId == transactionId &&
                x.TransactionType == transactionType
            );
        }

        public async Task MatchAsync(CashFlowMatching cashFlowMacting, IList<Dictionary<string, object>> items)
        {
            var createdCashFlowMacting = await _cashFlowMatchingManager.CreateAsync(cashFlowMacting);

            foreach (var item in items)
            {
                var id = int.Parse(item["Id"].ToString());
                var matched = bool.Parse(item["Matched"].ToString());

                var clientCashFlow = await _clientCashFlowRepository.GetAsync(id);
                if (clientCashFlow != null)
                {
                    clientCashFlow.CashFlowMatchingId = createdCashFlowMacting.Id;
                    clientCashFlow.Matched = matched;
                    await _clientCashFlowRepository.UpdateAsync(clientCashFlow);
                }
            }
        }

        public async Task<ClientCashFlow> GetLastByTypeAsync(int clientId, string type)
        {
            return await _clientCashFlowRepository.FirstOrDefaultAsync(x => x.ClientId == clientId && x.Type == type);
        }

    }
}
