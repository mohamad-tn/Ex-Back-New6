using Abp.Domain.Uow;
using Abp.Events.Bus;
using Abp.Runtime.Session;
using Bwr.Exchange.CashFlows.ManagementStatement.Events;
using Bwr.Exchange.Customers;
using Bwr.Exchange.Customers.Dto;
using Bwr.Exchange.Customers.Services;
using Bwr.Exchange.Settings.Clients.Services;
using Bwr.Exchange.Settings.Companies.Services;
using Bwr.Exchange.Settings.Currencies.Services;
using Bwr.Exchange.Settings.Treasuries.Services;
using Bwr.Exchange.Transfers.IncomeTransfers.Dto;
using Bwr.Exchange.Transfers.IncomeTransfers.Services.Interfaces;
using Bwr.Exchange.TreasuryActions.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bwr.Exchange.Transfers.IncomeTransfers
{
    public class IncomeTransferAppService : ExchangeAppServiceBase, IIncomeTransferAppService
    {
        private readonly IIncomeTransferManager _incomeTransferManager;
        private readonly ITreasuryActionManager _treasuryActionManager;
        private readonly ICustomerManager _customerManager;
        private readonly ITreasuryManager _treasuryManager;
        private readonly ICompanyManager _companyManager;
        private readonly IClientManager _clientManager;
        private readonly ICurrencyManager _currencyManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public IncomeTransferAppService(
            IIncomeTransferManager incomeTransferManager,
            ITreasuryActionManager treasuryActionManager,
            ICustomerManager customerManager,
            ITreasuryManager treasuryManager,
            IUnitOfWorkManager unitOfWorkManager,
            ICompanyManager companyManager,
            IClientManager clientManager,
            ICurrencyManager currencyManager)
        {
            _incomeTransferManager = incomeTransferManager;
            _treasuryActionManager = treasuryActionManager;
            _customerManager = customerManager;
            _treasuryManager = treasuryManager;
            _unitOfWorkManager = unitOfWorkManager;
            _companyManager = companyManager;
            _clientManager = clientManager;
            _currencyManager = currencyManager;
        }

        public async Task<IncomeTransferDto> CreateAsync(IncomeTransferDto input)
        {
            //var treasury = await _treasuryManager.GetTreasuryAsync();
            var treasury = await GetTreasuryByBranchIdAsync();
            var incomeTransfer = ObjectMapper.Map<IncomeTransfer>(input);

            for (int i = 0; i < input.IncomeTransferDetails.Count; i++)
            {
                if (input.IncomeTransferDetails[i].Sender != null)
                {
                    var sender = await CreateOrUpdateCustomer(input.IncomeTransferDetails[i].Sender);

                    incomeTransfer.IncomeTransferDetails[i].SenderId = sender.Id;
                    incomeTransfer.IncomeTransferDetails[i].Sender = sender;
                }

                if (input.IncomeTransferDetails[i].Beneficiary != null)
                {
                    var beneficiary = await CreateOrUpdateCustomer(input.IncomeTransferDetails[i].Beneficiary);
                    incomeTransfer.IncomeTransferDetails[i].BeneficiaryId = beneficiary.Id;
                    incomeTransfer.IncomeTransferDetails[i].Beneficiary = beneficiary;
                }
            }
            incomeTransfer.IncomeTransferDetails.OrderBy(x => x.Index);

            var createdIncomeTransfer = await _incomeTransferManager.CreateAsync(incomeTransfer);
            return ObjectMapper.Map<IncomeTransferDto>(createdIncomeTransfer);
        }

        public async Task<IncomeTransferDto> UpdateAsync(IncomeTransferDto input)
        {
            var dto = new IncomeTransferDto();

            //var treasury = await _treasuryManager.GetTreasuryAsync();
            var incomeTransfer = _incomeTransferManager.GetById(input.Id);
            var isDeleted = await _incomeTransferManager.DeleteDetailsAsync(incomeTransfer);

            for (int i = 0; i < incomeTransfer.IncomeTransferDetails.Count; i++)
            {
                string before = "";
                string after = "";

                #region Before & After
                if (incomeTransfer.Note != input.Note)
                {
                    before = L("Note") + " : " + incomeTransfer.Note;
                    after = L("Note") + " : " + input.Note;
                }

                if (incomeTransfer.Number != input.Number)
                {
                    before = before + " - " + L("Number") + " : " + incomeTransfer.Number;
                    after = after + " - " + L("Number") + " : " + input.Number;
                }

                if (incomeTransfer.IncomeTransferDetails[i].CurrencyId != input.IncomeTransferDetails[i].CurrencyId)
                {
                    before = before + " - " + L("Currency") + " : " + (incomeTransfer.IncomeTransferDetails[i].CurrencyId != null ? _currencyManager.GetCurrencyNameById(incomeTransfer.IncomeTransferDetails[i].CurrencyId) : " ");
                    after = after + " - " + L("Currency") + " : " + (input.IncomeTransferDetails[i].CurrencyId != null ? _currencyManager.GetCurrencyNameById(input.IncomeTransferDetails[i].CurrencyId) : " ");
                }

                if (incomeTransfer.IncomeTransferDetails[i].BeneficiaryId != input.IncomeTransferDetails[i].BeneficiaryId)
                {
                    before = before + " - " + L("Beneficiary") + " : " + (incomeTransfer.IncomeTransferDetails[i].BeneficiaryId != null ? _customerManager.GetCustomerNameById((int)incomeTransfer.IncomeTransferDetails[i].BeneficiaryId) : " ");
                    after = after + " - " + L("Beneficiary") + " : " + (input.IncomeTransferDetails[i].BeneficiaryId != null ? _customerManager.GetCustomerNameById((int)input.IncomeTransferDetails[i].BeneficiaryId) : " ");
                }

                if (incomeTransfer.IncomeTransferDetails[i].SenderId != input.IncomeTransferDetails[i].SenderId)
                {
                    before = before + " - " + L("Sender") + " : " + (incomeTransfer.IncomeTransferDetails[i].SenderId != null ? _customerManager.GetCustomerNameById((int)incomeTransfer.IncomeTransferDetails[i].SenderId) : " ");
                    after = after + " - " + L("Sender") + " : " + (input.IncomeTransferDetails[i].SenderId != null ? _customerManager.GetCustomerNameById((int)input.IncomeTransferDetails[i].SenderId) : " ");
                }

                if (incomeTransfer.IncomeTransferDetails[i].Amount != input.IncomeTransferDetails[i].Amount)
                {
                    before = before + " - " + L("Amount") + " : " + incomeTransfer.IncomeTransferDetails[i].Amount;
                    after = after + " - " + L("Amount") + " : " + input.IncomeTransferDetails[i].Amount;
                }

                if (incomeTransfer.CompanyId != input.CompanyId)
                {
                    before = before + " - " + L("Company") + " : " + (incomeTransfer.CompanyId != null ? _companyManager.GetCompanyNameById((int)incomeTransfer.CompanyId) : " ");
                    after = after + " - " + L("Company") + " : " + (input.CompanyId != null ? _companyManager.GetCompanyNameById((int)input.CompanyId) : " ");
                }

                if (incomeTransfer.IncomeTransferDetails[i].ToClientId != input.IncomeTransferDetails[i].ToClientId)
                {
                    before = before + " - " + L("Client") + " : " + (incomeTransfer.IncomeTransferDetails[i].ToClientId != null ? _clientManager.GetClientNameById((int)incomeTransfer.IncomeTransferDetails[i].ToClientId) : " ");
                    after = after + " - " + L("Client") + " : " + (input.IncomeTransferDetails[i].ToClientId != null ? _clientManager.GetClientNameById((int)input.IncomeTransferDetails[i].ToClientId) : " ");
                }

                if (incomeTransfer.IncomeTransferDetails[i].ToCompanyId != input.IncomeTransferDetails[i].ToCompanyId)
                {
                    before = before + " - " + L("ToCompany") + " : " + (incomeTransfer.IncomeTransferDetails[i].ToCompanyId != null ? _companyManager.GetCompanyNameById((int)incomeTransfer.IncomeTransferDetails[i].ToCompanyId) : " ");
                    after = after + " - " + L("ToCompany") + " : " + (input.IncomeTransferDetails[i].ToCompanyId != null ? _companyManager.GetCompanyNameById((int)input.IncomeTransferDetails[i].ToCompanyId) : " ");
                }

                if ((int)incomeTransfer.IncomeTransferDetails[i].PaymentType != input.IncomeTransferDetails[i].PaymentType)
                {
                    before = before + " - " + L("PaymentType") + " : " + ((PaymentType)incomeTransfer.IncomeTransferDetails[i].PaymentType);
                    after = after + " - " + L("PaymentType") + " : " + ((PaymentType)input.IncomeTransferDetails[i].PaymentType);
                }
                #endregion


                EventBus.Default.Trigger(
                new CreateManagementEventData(
                1, incomeTransfer.IncomeTransferDetails[i].Amount, incomeTransfer.Date,
                (int?)incomeTransfer.IncomeTransferDetails[i].PaymentType, DateTime.Now, 0,
                incomeTransfer.Number, null, null, before, after, null, null, null, null, null, null,
                incomeTransfer.IncomeTransferDetails[i].Commission, null, null,
                incomeTransfer.IncomeTransferDetails[i].CurrencyId, incomeTransfer.IncomeTransferDetails[i].ToClientId, AbpSession.GetUserId(),
                incomeTransfer.CompanyId, incomeTransfer.IncomeTransferDetails[i].SenderId,
                incomeTransfer.IncomeTransferDetails[i].BeneficiaryId, incomeTransfer.IncomeTransferDetails[i].ToCompanyId, null
                )
                );
            }

            ObjectMapper.Map<IncomeTransferDto, IncomeTransfer>(input, incomeTransfer);

            for (int i = 0; i < input.IncomeTransferDetails.Count; i++)
            {
                if (input.IncomeTransferDetails[i].Sender != null)
                {
                    var sender = await CreateOrUpdateCustomer(input.IncomeTransferDetails[i].Sender);

                    incomeTransfer.IncomeTransferDetails[i].SenderId = sender.Id;
                    incomeTransfer.IncomeTransferDetails[i].Sender = sender;
                }

                if (input.IncomeTransferDetails[i].Beneficiary != null)
                {
                    var beneficiary = await CreateOrUpdateCustomer(input.IncomeTransferDetails[i].Beneficiary);
                    incomeTransfer.IncomeTransferDetails[i].BeneficiaryId = beneficiary.Id;
                    incomeTransfer.IncomeTransferDetails[i].Beneficiary = beneficiary;
                }
            }

            var createdIncomeTransfer = await _incomeTransferManager.UpdateAsync(incomeTransfer);

            //الحوالات المسلمة
            if (incomeTransfer.IncomeTransferDetails.Any())
            {
                var ids = incomeTransfer.IncomeTransferDetails.Select(x => x.Id).ToList();
                var treasuryActions = _treasuryActionManager.GetByIncomeDetailsIds(ids);
                if (treasuryActions.Any())
                {
                    foreach(var treasuryAction in treasuryActions)
                    {
                        //هل الحوالة المسلمة تم حذفها او تم التعديل عليها
                        var incomDetail = input.IncomeTransferDetails.FirstOrDefault(x => x.Id == treasuryAction.IncomeTransferDetailId);
                        if (incomDetail != null && incomDetail.PaymentType == (int)PaymentType.Cash)
                        {
                            treasuryAction.Amount = incomDetail.Amount;
                            await UpdatePaidTransfer(treasuryAction);
                        }
                        else
                        {
                            await  _treasuryActionManager.DeleteAsync(treasuryAction);
                            
                        }
                    }
                }
            }
            //await _incomeTransferManager.CreateCashFlowAsync(createdIncomeTransfer);
            dto = ObjectMapper.Map<IncomeTransferDto>(createdIncomeTransfer);

            return dto;
        }

        private async Task UpdatePaidTransfer(TreasuryActions.TreasuryAction treasuryAction)
        {

            var cashFlowDeleted = await _treasuryActionManager.DeleteCashFlowAsync(treasuryAction);
            if (cashFlowDeleted)
            {
                var updatedTreasuryAction = await _treasuryActionManager.UpdateAsync(treasuryAction);

            }
        }
        public IList<IncomeTransferDto> GetForEdit(IncomeTransferGetForEditInputDto input)
        {
            var fromDate = string.IsNullOrEmpty(input.FromDate) ? DateTime.Now : DateTime.Parse(input.FromDate);
            fromDate = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day, 0, 0, 0);

            var toDate = string.IsNullOrEmpty(input.ToDate) ? DateTime.Now : DateTime.Parse(input.ToDate);
            toDate = new DateTime(toDate.Year, toDate.Month, toDate.Day, 23, 59, 59);

            var incomeTransfers = _incomeTransferManager.Get(fromDate, toDate, input.CompanyId, input.number).ToList();

            var incomeTransfersDtos = new List<IncomeTransferDto>();
            foreach(var incomeTransfer in incomeTransfers)
            {
                var sortedIncomeTransferDetails = incomeTransfer.IncomeTransferDetails.OrderBy(x => x.Index).ToList();
                incomeTransfer.IncomeTransferDetails = sortedIncomeTransferDetails;
                var dto = ObjectMapper.Map<IncomeTransferDto>(incomeTransfer);
                incomeTransfersDtos.Add(dto);
            }
            return incomeTransfersDtos;
        }

        private async Task<Customer> CreateOrUpdateCustomer(CustomerDto customerDto)
        {
            var customer = ObjectMapper.Map<Customer>(customerDto);
            return await _customerManager.CreateOrUpdateAsync(customer);
        }

        public int GetLastNumber()
        {
            return _incomeTransferManager.GetLastNumber();
        }

        public async Task DeleteAsync(int id)
        {
            var incomeTransfer = await _incomeTransferManager.GetByIdAsync(id);
            if (incomeTransfer != null)
            {
                var isDeleted = await _incomeTransferManager.DeleteAsync(incomeTransfer);

                if (isDeleted)
                {
                    foreach (var income in incomeTransfer.IncomeTransferDetails)
                    {
                        EventBus.Default.Trigger(
                           new CreateManagementEventData(
                               1, income.Amount, incomeTransfer.Date, (int?)income.PaymentType, DateTime.Now, 1,
                               incomeTransfer.Number, null, null, null, null, null, null, null, null, null, null,
                               income.Commission, null, null, income.CurrencyId, income.ToClientId, AbpSession.GetUserId(),
                               incomeTransfer.CompanyId, income.SenderId, income.BeneficiaryId, income.ToCompanyId, null
                         )
                     );
                    }
                }
            }
        }

        public async Task<Dictionary<string, double>> GetAllIncomeTransfersForBranch(long userId)
        {
            var user = await UserManager.GetUserByIdAsync(userId);
            if (user.BranchId != null)
            {
                return await _incomeTransferManager.GetAllForBranch((int)user.BranchId);
            }
            else
            {
                return await _incomeTransferManager.GetAllForBranch(null);
            }

        }
    }
}
