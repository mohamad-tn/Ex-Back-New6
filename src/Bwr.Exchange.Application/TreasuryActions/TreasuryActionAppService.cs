using Abp.Events.Bus;
using Bwr.Exchange.Settings.Clients.Services;
using Bwr.Exchange.Settings.Companies.Services;
using Bwr.Exchange.Settings.Treasuries.Services;
using Bwr.Exchange.Transfers.IncomeTransfers.Events;
using Bwr.Exchange.TreasuryActions.Dto;
using Bwr.Exchange.TreasuryActions.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bwr.Exchange.Reflection.Extensions;
using Bwr.Exchange.Transfers.IncomeTransfers;
using Bwr.Exchange.Customers;
using Bwr.Exchange.Transfers.Customers.Events;
using Bwr.Exchange.Transfers.IncomeTransfers.Services.Interfaces;
using System;
using Abp.UI;
using Microsoft.AspNetCore.Mvc;
using Bwr.Exchange.Shared.DataManagerRequests;
using Syncfusion.EJ2.Base;
using Bwr.Exchange.Shared.Dto;
using Bwr.Exchange.Customers.Services;
using Bwr.Exchange.Settings.Clients;
using Bwr.Exchange.Settings.Companies;
using Bwr.Exchange.Settings.Treasuries;
using Abp.Threading;
using Abp.Runtime.Session;
using Bwr.Exchange.CashFlows.ManagementStatement.Events;
using Bwr.Exchange.Transfers;
using Bwr.Exchange.Settings.Currencies.Services;
using Bwr.Exchange.Settings.Expenses.Services;
using Bwr.Exchange.Settings.Incomes.Services;

namespace Bwr.Exchange.TreasuryActions
{
    public class TreasuryActionAppService : ExchangeAppServiceBase, ITreasuryActionAppService
    {
        private readonly ITreasuryActionManager _treasuryActionManager;
        private readonly ITreasuryManager _treasuryManager;
        private readonly IClientManager _clientManager;
        private readonly IIncomeTransferDetailManager _incomeTransferDetailManager;
        private readonly ICompanyManager _companyManager;
        private readonly ICustomerManager _customerManager;
        private readonly ICurrencyManager _currencyManager;
        private readonly IExpenseManager _expenseManager;
        private readonly IIncomeManager _incomeManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TreasuryActionAppService(
            ITreasuryActionManager treasuryActionManager,
            ITreasuryManager treasuryManager,
            IClientManager clientManager,
            ICustomerManager customerManager,
            IIncomeTransferDetailManager incomeTransferDetailManager,
            ICompanyManager companyManager,
            IWebHostEnvironment webHostEnvironment,
            IHttpContextAccessor httpContextAccessor,
            ICurrencyManager currencyManager,
            IExpenseManager expenseManager,
            IIncomeManager incomeManager)
        {
            _treasuryActionManager = treasuryActionManager;
            _treasuryManager = treasuryManager;
            _clientManager = clientManager;
            _incomeTransferDetailManager = incomeTransferDetailManager;
            _companyManager = companyManager;
            _customerManager = customerManager;
            _webHostEnvironment = webHostEnvironment;
            _httpContextAccessor = httpContextAccessor;
            _currencyManager = currencyManager;
            _expenseManager = expenseManager;
            _incomeManager = incomeManager;
        }

        public async Task<TreasuryActionDto> CreateAsync(TreasuryActionDto input)
        {
            var branch = await GetCurrentBranch();
            var treasuryAction = ObjectMapper.Map<TreasuryAction>(input);
            treasuryAction.BranchId = branch.Id;
            var createdTreasuryAction = await _treasuryActionManager.CreateAsync(treasuryAction);
            return ObjectMapper.Map<TreasuryActionDto>(createdTreasuryAction);
        }

        public async Task<IList<TreasuryActionDto>> GetAsync(SearchTreasuryActionDto input)
        {
            var dic = input.ToDictionary();
            var treasuryActions = await _treasuryActionManager.GetAsync(dic);
            return ObjectMapper.Map<List<TreasuryActionDto>>(treasuryActions);
        }

        public async Task<IList<ExchangePartyDto>> GetExchangeParties()
        {
            var exchangePartiesDto = new List<ExchangePartyDto>();
            IList<Client> clients = new List<Client>();
            IList<Company> companies = new List<Company>();
            Treasury treasury = new Treasury();
            IList<Treasury> treasuries = new List<Treasury>();
            var user = await GetCurrentUserAsync();
            if (user.BranchId != null)
            {
                clients = await _clientManager.GetAllForCurrentBranchAsync((int)user.BranchId);
                companies = await _companyManager.GetAllForCurrentBranchAsync((int)user.BranchId);
                treasury = _treasuryManager.GetByBranchId((int)user.BranchId);
            }
            else
            {
                clients = await _clientManager.GetAllAsync();
                companies = await _companyManager.GetAllAsync();
                treasuries = await _treasuryManager.GetAllAsync();
            }

            if (treasuries.Count > 0)
            {
                exchangePartiesDto.AddRange((from e in treasuries
                                             select new ExchangePartyDto
                                             {
                                                 Id = e.Id,
                                                 Name = e.Name,
                                                 Group = "Treasury",
                                                 ExchangePartyId = $"Tr{e.Id}"
                                             }).ToList());
            }
            else if (treasury != null)
            {
                exchangePartiesDto.Add(new ExchangePartyDto
                {
                    Id = treasury.Id,
                    Name = treasury.Name,
                    Group = "Treasury",
                    ExchangePartyId = $"Tr{treasury.Id}"
                });
            }

            exchangePartiesDto.AddRange((from e in clients
                                         select new ExchangePartyDto
                                         {
                                             Id = e.Id,
                                             Name = e.Name,
                                             Group = "Client",
                                             ExchangePartyId = $"Cl{e.Id}"
                                         }).ToList());

            exchangePartiesDto.AddRange((from e in companies
                                         select new ExchangePartyDto
                                         {
                                             Id = e.Id,
                                             Name = e.Name,
                                             Group = "Company",
                                             ExchangePartyId = $"Co{e.Id}"
                                         }).ToList());
            return exchangePartiesDto;
        }

        public async Task<TreasuryActionDto> PayDirectTransferAsync(PayDirectTransferInputDto input)
        {
            await GetCurrentBranch();
            var images = new List<CustomerImage>();
            var incomeTransferDetail = await _incomeTransferDetailManager.GetByIdAsync(input.TreasuryAction.IncomeTransferDetailId.Value);

            var customer = await _customerManager.GetByIdAsync(incomeTransferDetail.BeneficiaryId.Value);
            if (customer.PhoneNumber != input.PhoneNumber || customer.IdentificationNumber == input.TreasuryAction.IdentificationNumber)
            {
                customer.PhoneNumber = input.PhoneNumber;
                customer.IdentificationNumber = input.TreasuryAction.IdentificationNumber;
                await _customerManager.UpdateAsync(customer);
            }
            if (!string.IsNullOrEmpty(input.TreasuryAction.IdentificationNumber))
            {
                await _customerManager.AddIdentityNumber(input.TreasuryAction.IdentificationNumber, incomeTransferDetail.BeneficiaryId.Value);
            }

            if (input.Images.Any())
            {
                var rootPath = _webHostEnvironment.WebRootPath;
                foreach (var fileUploadDto in input.Images)
                {
                    if (string.IsNullOrEmpty(fileUploadDto.FilePath))
                    {
                        var imagePath = fileUploadDto.SaveFileAndGetUrl(rootPath, "customers");

                        images.Add(new CustomerImage()
                        {
                            CustomerId = incomeTransferDetail.BeneficiaryId,
                            Path = imagePath,
                            Name = fileUploadDto.FileName,
                            Size = fileUploadDto.FileSize,
                            Type = fileUploadDto.FileType
                        });
                    }
                }
            }

            var treasuryActionDto = await CreateAsync(input.TreasuryAction);
            EventBus.Default.Trigger(
                new AddImageToCustomerEventData(
                    images: images
                    )
                );
            return treasuryActionDto;
        }

        public async Task<TreasuryActionDto> GetForEditAsync(int id)
        {
            var treasuryAction = await _treasuryActionManager.GetByIdAsync(id);
            return ObjectMapper.Map<TreasuryActionDto>(treasuryAction);
        }

        public async Task<TreasuryActionDto> UpdateAsync(TreasuryActionDto input)
        {
            await GetCurrentBranch();
            var treasuryAction = await _treasuryActionManager.GetByIdAsync(input.Id);

            string before = "";
            string after = "";

            #region Before & After
            if (treasuryAction.Note != input.Note)
            {
                before = L("Note") + " : " + treasuryAction.Note;
                after = L("Note") + " : " + input.Note;
            }

            if (treasuryAction.Number != input.Number)
            {
                before = before + " - " + L("Number") + " : " + treasuryAction.Number;
                after = after + " - " + L("Number") + " : " + input.Number;
            }

            if (treasuryAction.CurrencyId != input.CurrencyId)
            {
                before = before + " - " + L("Currency") + " : " + (treasuryAction.CurrencyId != null ? _currencyManager.GetCurrencyNameById((int)treasuryAction.CurrencyId) : " ");
                after = after + " - " + L("Currency") + " : " + (input.CurrencyId != null ? _currencyManager.GetCurrencyNameById((int)input.CurrencyId) : " ");
            }

            if (treasuryAction.ExchangePartyCompanyId != input.ExchangePartyCompanyId)
            {
                before = before + " - " + L("ExchangePartyCompany") + " : " + (treasuryAction.ExchangePartyCompanyId != null ? _companyManager.GetCompanyNameById((int)treasuryAction.ExchangePartyCompanyId) : " ");
                after = after + " - " + L("ExchangePartyCompany") + " : " + (input.ExchangePartyCompanyId != null ? _companyManager.GetCompanyNameById((int)input.ExchangePartyCompanyId) : " ");
            }

            if (treasuryAction.ExchangePartyClientId != input.ExchangePartyClientId)
            {
                before = before + " - " + L("ExchangePartyClient") + " : " + (treasuryAction.ExchangePartyClientId != null ? _clientManager.GetClientNameById((int)treasuryAction.ExchangePartyClientId) : " ");
                after = after + " - " + L("ExchangePartyClient") + " : " + (input.ExchangePartyClientId != null ? _clientManager.GetClientNameById((int)input.ExchangePartyClientId) : " ");
            }

            if (treasuryAction.Amount != input.Amount)
            {
                before = before + " - " + L("Amount") + " : " + treasuryAction.Amount;
                after = after + " - " + L("Amount") + " : " + input.Amount;
            }

            if (treasuryAction.MainAccountCompanyId != input.MainAccountCompanyId)
            {
                before = before + " - " + L("MainAccountCompany") + " : " + (treasuryAction.MainAccountCompanyId != null ? _companyManager.GetCompanyNameById((int)treasuryAction.MainAccountCompanyId) : " ");
                after = after + " - " + L("MainAccountCompany") + " : " + (input.MainAccountCompanyId != null ? _companyManager.GetCompanyNameById((int)input.MainAccountCompanyId) : " ");
            }

            if (treasuryAction.MainAccountClientId != input.MainAccountClientId)
            {
                before = before + " - " + L("MainAccountClient") + " : " + (treasuryAction.MainAccountClientId != null ? _clientManager.GetClientNameById((int)treasuryAction.MainAccountClientId) : " ");
                after = after + " - " + L("MainAccountClient") + " : " + (input.MainAccountClientId != null ? _clientManager.GetClientNameById((int)input.MainAccountClientId) : " ");
            }

            if (treasuryAction.ExpenseId != input.ExpenseId)
            {
                before = before + " - " + L("Expense") + " : " + (treasuryAction.ExpenseId != null ? _expenseManager.GetExpenseNameById((int)treasuryAction.ExpenseId) : " ");
                after = after + " - " + L("Expense") + " : " + (input.ExpenseId != null ? _expenseManager.GetExpenseNameById((int)input.ExpenseId) : " ");
            }

            if (treasuryAction.IncomeId != input.IncomeId)
            {
                before = before + " - " + L("Income") + " : " + (treasuryAction.IncomeId != null ? _incomeManager.GetIncomeNameById((int)treasuryAction.IncomeId) : " ");
                after = after + " - " + L("Income") + " : " + (input.IncomeId != null ? _incomeManager.GetIncomeNameById((int)input.IncomeId) : " ");
            }

            if ((int)treasuryAction.MainAccount != input.MainAccount)
            {
                before = before + " - " + L("MainAccount") + " : " + treasuryAction.MainAccount;
                after = after + " - " + L("MainAccount") + " : " + input.MainAccount;
            }

            if ((int)treasuryAction.ActionType != input.ActionType)
            {
                before = before + " - " + L("PaymentType") + " : " + ((PaymentType)treasuryAction.ActionType);
                after = after + " - " + L("PaymentType") + " : " + ((PaymentType)input.ActionType);
            }
            #endregion

            EventBus.Default.Trigger(
            new CreateManagementEventData(
                2, treasuryAction.Amount, treasuryAction.Date, null, DateTime.Now, 0, treasuryAction.Number,
                (int?)treasuryAction.ActionType, treasuryAction.MainAccount.ToString(), before, after, null, null, null, null, null,
                null, null, null, null, treasuryAction.CurrencyId, treasuryAction.MainAccountClientId, AbpSession.GetUserId(),
                treasuryAction.MainAccountCompanyId, null, null, treasuryAction.ExchangePartyCompanyId, null
                )
            );

            var date = DateTime.Parse(input.Date);
            date = new DateTime
                    (
                        date.Year,
                        date.Month,
                        date.Day,
                        treasuryAction.Date.Hour,
                        treasuryAction.Date.Minute,
                        treasuryAction.Date.Second
                    );
            var cashFlowDeleted = await _treasuryActionManager.DeleteCashFlowAsync(treasuryAction);
            if (cashFlowDeleted)
            {
                ObjectMapper.Map<TreasuryActionDto, TreasuryAction>(input, treasuryAction);
                treasuryAction.Date = date;

                var updatedTreasuryAction = await _treasuryActionManager.UpdateAsync(treasuryAction);
                return ObjectMapper.Map<TreasuryActionDto>(updatedTreasuryAction);
            }
            else
            {
                throw new UserFriendlyException("Exception Message");
            }
        }

        public async Task DeleteAsync(int id)
        {
            var treasuryAction = await _treasuryActionManager.GetByIdAsync(id);
            if (treasuryAction != null)
            {
                await _treasuryActionManager.DeleteAsync(treasuryAction);

                EventBus.Default.Trigger(
                new CreateManagementEventData(
                    2, treasuryAction.Amount, treasuryAction.Date, null, DateTime.Now, 1, treasuryAction.Number,
                    (int?)treasuryAction.ActionType, treasuryAction.MainAccount.ToString(), null, null, null, null, null, null, null,
                    null, null, null, null, treasuryAction.CurrencyId, treasuryAction.MainAccountClientId, AbpSession.GetUserId(),
                    treasuryAction.MainAccountCompanyId, null, null, treasuryAction.ExchangePartyCompanyId, null
                    )
                );
            }
        }

        [HttpPost]
        public ReadGrudDto GetForGrid([FromBody] TreasuryActionDataManagerRequest dm)
        {
            var fromDate = DateTime.Now;
            var toDate = DateTime.Now;

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

            var treasuryActionsDto = new List<ListTreasuryActionDto>();

            var dic = new Dictionary<string, object>()
            {
                { "Number" , dm.number},
                { "FromDate" , fromDate},
                { "ToDate" , toDate},
                { "ActionType" , dm.actionType},
                { "CurrencyId" , dm.currencyId}
            };

            IList<TreasuryAction> treasuryActions = new List<TreasuryAction>();
            var user = UserManager.GetUserById(dm.userId);

            if (user.BranchId != null)
            {
                treasuryActions = _treasuryActionManager.Get(dic).Where(x => x.BranchId == user.BranchId).ToList();
            }
            else
            {
                treasuryActions = _treasuryActionManager.Get(dic);

            }


            IEnumerable<ListTreasuryActionDto> data = ObjectMapper.Map<List<ListTreasuryActionDto>>(treasuryActions);
            switch (dm.mainAccount)
            {
                case 0:
                    {
                        if (dm.mainAccountClientId != null)
                            data = data.Where(x => x.MainAccountClientId == dm.mainAccountClientId);
                    }
                    break;
                case 1:
                    {
                        if (dm.mainAccountCompanyId != null)
                            data = data.Where(x => x.MainAccountCompanyId == dm.mainAccountCompanyId);
                    }
                    break;
                case 2:
                    {
                        if (dm.incomeId != null)
                            data = data.Where(x => x.IncomeId == dm.incomeId);
                    }
                    break;
                case 3:
                    {
                        if (dm.expenseId != null)
                            data = data.Where(x => x.ExpenseId == dm.expenseId);
                    }
                    break;
                case 4:
                    {
                        if (dm.incomeTransferDetailId != null)
                            data = data.Where(x => x.IncomeTransferDetailId == dm.incomeTransferDetailId);
                    }
                    break;
                default: break;
            }

            var operations = new DataOperations();


            var count = data.Count();

            if (dm.Skip != 0)
            {
                data = operations.PerformSkip(data, dm.Skip);
            }

            if (dm.Take != 0)
            {
                data = operations.PerformTake(data, dm.Take);
            }

            return new ReadGrudDto() { result = data, count = count, groupDs = null };
        }

        public IList<TreasuryActionStatementOutputDto> GetFroStatment(TreasuryActionStatementInputDto input)
        {
            if (!string.IsNullOrEmpty(input.FromDate))
            {
                DateTime fromDate;
                DateTime.TryParse(input.FromDate, out fromDate);
                fromDate = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day, 12, 0, 0);

                input.FromDate = fromDate.Date.ToString();
            }

            if (!string.IsNullOrEmpty(input.ToDate))
            {
                DateTime toDate;
                DateTime.TryParse(input.ToDate, out toDate);
                toDate = new DateTime(toDate.Year, toDate.Month, toDate.Day, 23, 59, 59);
                input.ToDate = toDate.ToString();
            }

            var dic = input.ToDictionary();

            IList<TreasuryAction> treasuryActions = new List<TreasuryAction>();
            var user = AsyncHelper.RunSync(GetCurrentUserAsync);
            if (user.BranchId != null)
            {
                treasuryActions = _treasuryActionManager.GetForStatment(dic).Where(x => x.BranchId == user.BranchId).ToList();
            }
            else
            {
                treasuryActions = _treasuryActionManager.GetForStatment(dic);
            }

            return ObjectMapper.Map<List<TreasuryActionStatementOutputDto>>(treasuryActions);
        }

        public int GetLastNumber()
        {
            return _treasuryActionManager.GetLastNumber();
        }
    }
}
