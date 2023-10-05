using Abp.Threading;
using Bwr.Exchange.CashFlows.CashFlowMatchings;
using Bwr.Exchange.CashFlows.ClientCashFlows.Dto;
using Bwr.Exchange.CashFlows.ClientCashFlows.Services;
using Bwr.Exchange.Settings.Clients.Dto;
using Bwr.Exchange.Settings.Clients.Services;
using Bwr.Exchange.Settings.Currencies.Dto;
using Bwr.Exchange.Settings.Currencies.Services;
using Bwr.Exchange.Shared.DataManagerRequests;
using Bwr.Exchange.Shared.Dto;
using Microsoft.AspNetCore.Mvc;
using Syncfusion.EJ2.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bwr.Exchange.Reflection.Extensions;
using Bwr.Exchange.CashFlows.Shared.Dto;
using Bwr.Exchange.Settings.Clients;
using Bwr.Exchange.Settings.Currencies;

namespace Bwr.Exchange.CashFlows.ClientCashFlows
{
    public class ClientCashFlowAppService : ExchangeAppServiceBase, IClientCashFlowAppService
    {
        private readonly IClientCashFlowManager _clientCashFlowManager;
        private readonly ICurrencyManager _currencyManager;
        private readonly IClientManager _clientManager;

        public ClientCashFlowAppService(IClientCashFlowManager clientCashFlowManager, ICurrencyManager currencyManager, IClientManager clientManager)
        {
            _clientCashFlowManager = clientCashFlowManager;
            _currencyManager = currencyManager;
            _clientManager = clientManager;
        }

        public IList<ClientCashFlowDto> Get(GetClientCashFlowInput input)
        {
            var clientCashFlows = _clientCashFlowManager.Get(input.ClientId);
            var clientCashFlowsDto = ObjectMapper.Map<List<ClientCashFlowDto>>(clientCashFlows);
            return clientCashFlowsDto;
        }

        public IList<ClientCashFlowTotalDto> GetClientsBalances(string date)
        {
            IList<Client> clients = new List<Client>();
            IList<Currency> currencies = new List<Currency>();

            var user = AsyncHelper.RunSync(GetCurrentUserAsync);
            if (user.BranchId != null)
            {
                clients = _clientManager.GetAll().Where(x => x.BranchId == user.BranchId).ToList();
                currencies = _currencyManager.GetAll().Where(x => x.BranchId == user.BranchId).ToList();

            }
            else
            {
                clients = _clientManager.GetAll();
                currencies = _currencyManager.GetAll();
            }

            var toDate = DateTime.Now;
            if (!string.IsNullOrEmpty(date))
            {
                toDate = DateTime.Parse(date);
            }
            toDate = new DateTime(toDate.Year, toDate.Month, toDate.Day, 23, 59, 59);

            var dtos = new List<ClientCashFlowTotalDto>();

            foreach (var client in clients)
            {
                var dto = new ClientCashFlowTotalDto()
                {
                    ClientId = client.Id,
                    ClientName = client.Name
                };

                foreach (var currency in currencies)
                {
                    var balance = _clientCashFlowManager.GetLastBalance(client.Id, currency.Id, toDate);
                    var currencyBalance = new ClientCashFlowTotalDetailDto()
                    {
                        CurrencyId = currency.Id,
                        CurrencyName = currency.Name,
                        CurrentBalance = balance
                    };

                    //التحقق في حال كانت الشركة نشطة اليوم
                    dto.IsActiveToday = _clientCashFlowManager.CheckIsActiveToday(client.Id, toDate);
                    dto.IsMatching = _clientCashFlowManager.CheckIsIfMatching(client.Id);
                    dto.CurrencyBalances.Add(currencyBalance);
                }

                dtos.Add(dto);   
            }

            return dtos;
        }

        public async Task<IList<DefaultersOfPaymentDto>> GetDefaulters(int days)
        {
            var dtos = new List<DefaultersOfPaymentDto>();
            IList<Client> clients = new List<Client>();
            IList<Currency> currencies = new List<Currency>();

            var user = await GetCurrentUserAsync();

            if (user.BranchId != null)
            {
                clients = await _clientCashFlowManager.GetForDefaultersAsync(days);
                clients = clients.Where(x => x.BranchId == user.BranchId).ToList();
                currencies = _currencyManager.GetAll().Where(x => x.BranchId == user.BranchId).ToList();
            }
            else
            {
                clients = await _clientCashFlowManager.GetForDefaultersAsync(days);
                currencies = _currencyManager.GetAll();
            }
 
            foreach (var client in clients)
            {
                var dto = new DefaultersOfPaymentDto()
                {
                    ClientId = client.Id,
                    ClientName = client.Name
                };

                foreach (var currency in currencies)
                {
                    var clientBalance = _clientCashFlowManager.GetLastBalance(client.Id, currency.Id, DateTime.Now);

                    var currencyBalance = new ClientCashFlowTotalDetailDto()
                    {
                        CurrencyId = currency.Id,
                        CurrencyName = currency.Name,
                        CurrentBalance = clientBalance
                    };

                    dto.CurrencyBalances.Add(currencyBalance);
                }

                if(dto.CurrencyBalances.Any(x=>x.CurrentBalance > 0))
                {
                    var lastReceiptCashFlow = await _clientCashFlowManager.GetLastByTypeAsync(client.Id, TransactionConst.Receipt);

                    if (lastReceiptCashFlow != null)
                    {
                        dto.LastActionDate = lastReceiptCashFlow.Date;
                    }

                    dtos.Add(dto);
                }
            }

            return dtos;
        }

        public async Task<IList<InactiveClientDto>> GetInactiveClient(int days)
        {
            var dtos = new List<InactiveClientDto>();

            IList<Client> clients = new List<Client>();

            var user = await GetCurrentUserAsync();

            if (user.BranchId != null)
            { 
                clients = await _clientCashFlowManager.GetForDefaultersAsync(days);
                clients = clients.Where(x => x.BranchId == user.BranchId).ToList();
            }
            else
            {
                clients = await _clientCashFlowManager.GetForDefaultersAsync(days);                
            }

            foreach (var client in clients)
            {
                var dto = new InactiveClientDto()
                {
                    ClientId = client.Id,
                    ClientName = client.Name
                };

                var cashFlow = await _clientCashFlowManager.GetLastAsync(client.Id);
                if (cashFlow != null)
                {
                    dto.LastActionDate = cashFlow.Date;
                    dtos.Add(dto);
                }
            }

            return dtos;
        }

        [HttpPost]
        public ReadGrudDto GetForGrid([FromBody] CashFlowDataManagerRequest dm)
        {
            DateTime fromDate = new DateTime(), toDate = new DateTime();

            if (!string.IsNullOrEmpty(dm.fromDate))
            {
                DateTime.TryParse(dm.fromDate, out fromDate);
                fromDate = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day, 0, 0, 0);
            }
            
            if (!string.IsNullOrEmpty(dm.toDate))
            {
                DateTime.TryParse(dm.toDate, out toDate);
                toDate = new DateTime(toDate.Year, toDate.Month, toDate.Day, 23, 59, 59);
            }

            var clientCashFlowsDto = new List<ClientCashFlowDto>();
            if(dm.id != 0 && dm.currencyId != 0)
            {
                //Add Previous Balance
                var previousBalance = _clientCashFlowManager.GetPreviousBalance2(dm.id, dm.currencyId, fromDate);
                clientCashFlowsDto.Add(new ClientCashFlowDto
                {
                    ClientId = dm.id,
                    CurrencyId = dm.currencyId,
                    CurrentBalance = previousBalance,
                    Type = TransactionConst.PreviousBalance
                });

                //Get Client Cash Flows
                var clientCashFlows = _clientCashFlowManager.Get(dm.id, dm.currencyId, fromDate, toDate);
                clientCashFlowsDto.AddRange(ObjectMapper.Map<List<ClientCashFlowDto>>(clientCashFlows));

                InitialBalance(clientCashFlowsDto);
            }
            

            IEnumerable<ClientCashFlowDto> data = clientCashFlowsDto;
            AddCreationUsers(data);
            var operations = new DataOperations();

            IEnumerable groupDs = new List<ClientCashFlowDto>();
            if (dm.Group != null)
            {
                groupDs = operations.PerformSelect(data, dm.Group);
            }

            var count = data.Count();

            if (dm.Skip != 0)
            {
                data = operations.PerformSkip(data, dm.Skip);
            }

            if (dm.Take != 0)
            {
                data = operations.PerformTake(data, dm.Take);
            }
            
            return new ReadGrudDto() { result = data, count = count, groupDs = groupDs };
        }

        public async Task<CurrentClientBalanceDto> GetCurrentBalanceAsync(GetClientCashFlowInput input)
        {
            var toDate = DateTime.Now;
            if (!string.IsNullOrEmpty(input.ToDate))
            {
                toDate = DateTime.Parse(input.ToDate);
            }
            toDate = new DateTime(toDate.Year, toDate.Month, toDate.Day, 23, 59, 59);

            var balance = _clientCashFlowManager.GetLastBalance(input.ClientId, input.CurrencyId, toDate);

            var client = await _clientManager.GetByIdAsync(input.ClientId);
            var clientDto = ObjectMapper.Map<ClientDto>(client);

            var currency = await _currencyManager.GetByIdAsync(input.CurrencyId);
            var currencyDto = ObjectMapper.Map<CurrencyDto>(currency);

            return new CurrentClientBalanceDto(clientDto, currencyDto, balance);
        }

        public async Task<ClientCashFlowMatchingDto> MatchAsync(ClientCashFlowMatchingDto input)
        {
            var matching = new CashFlowMatching()
            {
                MatchingBy = input.MatchingBy,
                MatchingWith = input.MatchingWith,
                Description = input.Description
            };

            var listDic = new List<Dictionary<string, object>>();
            foreach (var item in input.Items)
            {
                listDic.Add(item.ToDictionary());
            }
            await _clientCashFlowManager.MatchAsync(matching, listDic);

            return input;
        }

        public IList<SummaryCashFlowDto> Summary(string date)
        {
            var toDate = DateTime.Now;
            if (!string.IsNullOrEmpty(date))
            {
                toDate = DateTime.Parse(date);
            }
            toDate = new DateTime(toDate.Year, toDate.Month, toDate.Day, 23, 59, 59);

            var dtos = new List<SummaryCashFlowDto>();

            IList<Client> clients = new List<Client>();
            IList<Currency> currencies = new List<Currency>();

            var user = AsyncHelper.RunSync(GetCurrentUserAsync);

            if (user.BranchId != null)
            {
                clients = _clientManager.GetAll().Where(x => x.BranchId == user.BranchId).ToList();
                currencies = _currencyManager.GetAll().Where(x => x.BranchId == user.BranchId).ToList();
            }
            else
            {
                clients = _clientManager.GetAll();
                currencies = _currencyManager.GetAll();
            }


            foreach (var currency in currencies)
            {
                var dto = new SummaryCashFlowDto()
                {
                    Currency = ObjectMapper.Map<CurrencyDto>(currency),
                };
                foreach (var client in clients)
                {
                    var balance = _clientCashFlowManager.GetLastBalance(client.Id, currency.Id, toDate);
                    dto.TotalBalance += balance;
                }

                dtos.Add(dto);
            }

            return dtos;
        }

        private WhereFilter GetWhereFilter(List<WhereFilter> filterOptions, string name)
        {
            var filter = filterOptions.FirstOrDefault(x => x.Field == name);
            if (filter != null)
                return filter;

            foreach(var option in filterOptions)
            {
                return GetWhereFilter(option.predicates, name);
            }

            return null;
        }

        private void AddCreationUsers(IEnumerable<ClientCashFlowDto> data)
        {
            foreach (var item in data)
            {
                if (item.CreatorUserId != null)
                {
                    var user = AsyncHelper.RunSync(() => UserManager.GetUserByIdAsync(item.CreatorUserId.Value));
                    item.CreatorUser = user?.FullName;
                }
            }
        }

        private void InitialBalance(List<ClientCashFlowDto> clientCashFlows)
        {
            double value = 0;
            for (int i = 0; i < clientCashFlows.Count; i++)
            {
                value += clientCashFlows[i].CurrentBalance;
                clientCashFlows[i].Balance = value;
            }
        }
    }
}
