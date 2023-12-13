using Bwr.Exchange.Customers;
using Bwr.Exchange.Customers.Dto;
using Bwr.Exchange.Customers.Services;
using Bwr.Exchange.Settings.Treasuries.Services;
using Bwr.Exchange.Shared.Dto;
using Bwr.Exchange.Transfers.OutgoingTransfers.Dto;
using Bwr.Exchange.Transfers.OutgoingTransfers.Services;
using Microsoft.AspNetCore.Mvc;
using Syncfusion.EJ2.Base;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bwr.Exchange.Reflection.Extensions;
using System.Linq;
using System.Collections;
using Abp.UI;
using Microsoft.AspNetCore.Hosting;
using Abp.Threading;
using Bwr.Exchange.Shared.DataManagerRequests;
using Bwr.Exchange.Settings.Branches.Services;
using Bwr.Exchange.Settings.Clients.Services;
using Bwr.Exchange.Settings.Companies.Services;
using Bwr.Exchange.Settings.Clients;
using Bwr.Exchange.Settings.Companies;
using Bwr.Exchange.Settings.Currencies.Services;
using Bwr.Exchange.Settings.Currencies;
using Abp.Events.Bus;
using Abp.Runtime.Session;
using Bwr.Exchange.CashFlows.ManagementStatement.Events;

namespace Bwr.Exchange.Transfers.OutgoingTransfers
{
    public class OutgoingTransferAppService : ExchangeAppServiceBase, IOutgoingTransferAppService
    {
        private readonly IOutgoingTransferManager _outgoingTransferManager;
        private readonly ICustomerManager _customerManager;
        private readonly IClientManager _clientManager;
        private readonly ICompanyManager _companyManager;
        private readonly IBranchManager _branchManager;
        private readonly ICurrencyManager _currencyManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public OutgoingTransferAppService(
            OutgoingTransferManager outgoingTransferManager,
            ICustomerManager customerManager,
            ITreasuryManager treasuryManager,
            IWebHostEnvironment webHostEnvironment,
            IBranchManager branchManager,
            IClientManager clientManager,
            ICompanyManager companyManager,
            ICurrencyManager currencyManager)
        {
            _outgoingTransferManager = outgoingTransferManager;
            _customerManager = customerManager;
            _webHostEnvironment = webHostEnvironment;
            _branchManager = branchManager;
            _clientManager = clientManager;
            _companyManager = companyManager;
            _currencyManager = currencyManager;
        }

        public async Task<OutgoingTransferDto> CreateAsync(OutgoingTransferDto input)
        {
            IList<Currency> toBranchCurrencies = new List<Currency>();
            if (input.ToBranchId != null)
            {
                toBranchCurrencies = await _currencyManager.GetAllForCurrentBranchAsync((int)input.ToBranchId);

                var currentCurrency = await _currencyManager.GetByIdAsync(input.CurrencyId);

                if (!toBranchCurrencies.Any(x => x.Name.ToLower().Trim() == currentCurrency.Name.ToLower().Trim()))
                {
                    throw new UserFriendlyException(L(ValidationResultMessage.TheDestinationBranchDoseNotDealInThisCurrency));
                }
            }            

            var branch = await GetCurrentBranch();
            var treasury = await GetTreasuryByBranchIdAsync();
            if (input.ToBranchId != null)
            {
                var toBranch = await _branchManager.GetByIdAsync((int)input.ToBranchId);
                input.CountryId = toBranch.CountryId;
            }
            var outgoingTransfer = ObjectMapper.Map<OutgoingTransfer>(input);

            var sender = await CreateOrUpdateCustomer(input.Sender);
            var beneficiary = await CreateOrUpdateCustomer(input.Beneficiary);

            outgoingTransfer.SenderId = sender?.Id;
            outgoingTransfer.BeneficiaryId = beneficiary?.Id;

            outgoingTransfer.TreasuryId = treasury.Id;

            outgoingTransfer.FromBranchId = branch.Id; 

            var createdOutgoingTransfer = await _outgoingTransferManager.CreateAsync(outgoingTransfer);
            return ObjectMapper.Map<OutgoingTransferDto>(createdOutgoingTransfer);
        }

        public async Task<OutgoingTransferDto> UpdateAsync(OutgoingTransferDto input)
        {
            var branch = await GetCurrentBranch();

            var outgoingTransfer = await _outgoingTransferManager.GetByIdAsync(input.Id);
            if (outgoingTransfer.ToBranchId != null &&
                outgoingTransfer.Status == OutgoingTransferStatus.Accepted)
            {
                throw new UserFriendlyException(L(ValidationResultMessage.TheOutgoingTransferHasBeenDisbursedItCannotBeModified));
            }
            else
            {
                string before = "";
                string after = "";

                #region Before & After
                if (outgoingTransfer.Note != input.Note)
                {
                    before = L("Note") + " : " + outgoingTransfer.Note;
                    after = L("Note") + " : " + input.Note;
                }

                if (outgoingTransfer.CurrencyId != input.CurrencyId)
                {
                    before = before + " - " + L("Currency") + " : " + (outgoingTransfer.CurrencyId != null ? _currencyManager.GetCurrencyNameById(outgoingTransfer.CurrencyId) : " ");
                    after = after + " - " + L("Currency") + " : " + (input.CurrencyId != null ? _currencyManager.GetCurrencyNameById(input.CurrencyId) : " ");
                }

                if (outgoingTransfer.BeneficiaryId != input.BeneficiaryId)
                {
                    before = before + " - " + L("Beneficiary") + " : " + (outgoingTransfer.BeneficiaryId != null ? _customerManager.GetCustomerNameById((int)outgoingTransfer.BeneficiaryId) : " ");
                    after = after + " - " + L("Beneficiary") + " : " + (input.BeneficiaryId != null ? _customerManager.GetCustomerNameById((int)input.BeneficiaryId) : " ");
                }

                if (outgoingTransfer.SenderId != input.SenderId)
                {
                    before = before + " - " + L("Sender") + " : " + (outgoingTransfer.SenderId != null ? _customerManager.GetCustomerNameById((int)outgoingTransfer.SenderId) : " ");
                    after = after + " - " + L("Sender") + " : " + (input.SenderId != null ? _customerManager.GetCustomerNameById((int)input.SenderId) : " ");
                }

                if (outgoingTransfer.Amount != input.Amount)
                {
                    before = before + " - " + L("Amount") + " : " + outgoingTransfer.Amount;
                    after = after + " - " + L("Amount") + " : " + input.Amount;
                }

                if (outgoingTransfer.ToCompanyId != input.ToCompanyId)
                {
                    before = before + " - " + L("ToCompany") + " : " + (outgoingTransfer.ToCompanyId != null ? _companyManager.GetCompanyNameById((int)outgoingTransfer.ToCompanyId) : " ");
                    after = after + " - " + L("ToCompany") + " : " + (input.ToCompanyId != null ? _companyManager.GetCompanyNameById((int)input.ToCompanyId) : " ");
                }

                if (outgoingTransfer.FromCompanyId != input.FromCompanyId)
                {
                    before = before + " - " + L("FromCompany") + " : " + (outgoingTransfer.FromCompanyId != null ? _companyManager.GetCompanyNameById((int)outgoingTransfer.FromCompanyId) : " ");
                    after = after + " - " + L("FromCompany") + " : " + (input.FromCompanyId != null ? _companyManager.GetCompanyNameById((int)input.FromCompanyId) : " ");
                }

                if ((int)outgoingTransfer.PaymentType != input.PaymentType)
                {
                    before = before + " - " + L("PaymentType") + " : " + L(((PaymentType)outgoingTransfer.PaymentType).ToString());
                    after = after + " - " + L("PaymentType") + " : " + L(((PaymentType)input.PaymentType).ToString());
                }

                if (outgoingTransfer.FromClientId != input.FromClientId)
                {
                    before = before + " - " + L("FromClient") + " : " + (outgoingTransfer.FromClientId != null ? _clientManager.GetClientNameById((int)outgoingTransfer.FromClientId) : " ");
                    after = after + " - " + L("FromClient") + " : " + (input.FromClientId != null ? _clientManager.GetClientNameById((int)input.FromClientId) : " ");
                }

                if (outgoingTransfer.ReceivedAmount != input.ReceivedAmount)
                {
                    before = before + " - " + L("ReceivedAmount") + " : " + outgoingTransfer.ReceivedAmount;
                    after = after + " - " + L("ReceivedAmount") + " : " + input.ReceivedAmount;
                }

                if (outgoingTransfer.InstrumentNo != input.InstrumentNo)
                {
                    before = before + " - " + L("InstrumentNo") + " : " + outgoingTransfer.InstrumentNo;
                    after = after + " - " + L("InstrumentNo") + " : " + input.InstrumentNo;
                }

                if (outgoingTransfer.Reason != input.Reason)
                {
                    before = before + " - " + L("Reason") + " : " + outgoingTransfer.Reason;
                    after = after + " - " + L("Reason") + " : " + input.Reason;
                }
                #endregion


                EventBus.Default.Trigger(
                    new CreateManagementEventData(
                        0, outgoingTransfer.Amount, outgoingTransfer.Date, (int?)outgoingTransfer.PaymentType,
                        DateTime.Now, 0, outgoingTransfer.Number, null, null, before, after, null, null, null, null,
                        null, null, outgoingTransfer.Commission, null, null, outgoingTransfer.CurrencyId,
                        outgoingTransfer.FromClientId, AbpSession.GetUserId(), outgoingTransfer.FromCompanyId
                        , outgoingTransfer.SenderId, outgoingTransfer.BeneficiaryId, outgoingTransfer.ToCompanyId, null,branch.Id
                        )
                    );


                var date = DateTime.Parse(input.Date);
                date = new DateTime
                        (
                            date.Year,
                            date.Month,
                            date.Day,
                            outgoingTransfer.Date.Hour,
                            outgoingTransfer.Date.Minute,
                            outgoingTransfer.Date.Second
                        );

                var isDeleted = await _outgoingTransferManager.DeleteCashFlowAsync(outgoingTransfer);
                if (isDeleted)
                {
                    ObjectMapper.Map<OutgoingTransferDto, OutgoingTransfer>(input, outgoingTransfer);

                    outgoingTransfer.Date = date;
                    var sender = await CreateOrUpdateCustomer(input.Sender);
                    var beneficiary = await CreateOrUpdateCustomer(input.Beneficiary);

                    outgoingTransfer.SenderId = sender?.Id;
                    outgoingTransfer.BeneficiaryId = beneficiary?.Id;

                    var updatedOutgoingTransfer = await _outgoingTransferManager.UpdateAsync(outgoingTransfer);
                    return ObjectMapper.Map<OutgoingTransferDto>(updatedOutgoingTransfer);
                }
                else
                {
                    throw new UserFriendlyException("حدث خطأ ما اثناء التعديل");
                }
            }
            
        }

        public async Task DeleteAsync(int id)
        {
            var branch = await GetCurrentBranch();

            var outgoingTransfer = await _outgoingTransferManager.GetByIdAsync(id);
            if(outgoingTransfer != null)
            {
                await _outgoingTransferManager.DeleteAsync(outgoingTransfer);

                EventBus.Default.Trigger(
                new CreateManagementEventData(
                    0, outgoingTransfer.Amount, outgoingTransfer.Date, (int?)outgoingTransfer.PaymentType,
                    DateTime.Now, 1, outgoingTransfer.Number, null, null, null, null, null, null, null, null,
                    null, null, outgoingTransfer.Commission, null, null, outgoingTransfer.CurrencyId,
                    outgoingTransfer.FromClientId, AbpSession.GetUserId(), outgoingTransfer.FromCompanyId
                    , outgoingTransfer.SenderId, outgoingTransfer.BeneficiaryId, outgoingTransfer.ToCompanyId, null,branch.Id
                    )
                );
            }
        }

        public async Task<IList<OutgoingTransferDto>> Get(SearchOutgoingTransferInputDto input)
        {
            var dic = input.ToDictionary();
            var outgoingTransfers = await _outgoingTransferManager.GetAsync(dic);
            return ObjectMapper.Map<List<OutgoingTransferDto>>(outgoingTransfers);
        }

        public IList<ReadOutgoingTransferDto> GetForStatment(SearchOutgoingTransferInputDto input)
        {
            var treasury = AsyncHelper.RunSync(GetTreasuryByBranchIdAsync);

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
            var outgoingTransfers = _outgoingTransferManager.Get(dic).Where(x=>x.TreasuryId == treasury.Id);
            return ObjectMapper.Map<List<ReadOutgoingTransferDto>>(outgoingTransfers);
        }

        public OutgoingTransferDto GetById(int id)
        {
            var outgoingTransfer = _outgoingTransferManager.GetById(id);
            return ObjectMapper.Map<OutgoingTransferDto>(outgoingTransfer);
        }

        public async Task<OutgoingTransferDto> GetForEditAsync(int id)
        {
            var transfer = await _outgoingTransferManager.GetByIdAsync(id);
            return ObjectMapper.Map<OutgoingTransferDto>(transfer);
        }

        [HttpPost]
        public ReadGrudDto GetForGrid([FromBody] SearchOutgoingDataManagerRequest dm)
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

            var dic = new Dictionary<string, object>()
            {
                { "Number" , dm.number},
                { "FromDate" , fromDate},
                { "ToDate" , toDate},
                { "PaymentType" , dm.paymentType},
                { "ClientId" , dm.clientId},
                { "CompanyId" , dm.companyId},
                { "CountryId" , dm.countryId},
                { "Beneficiary" , dm.beneficiary},
                { "BeneficiaryAddress" , dm.beneficiaryAddress},
                { "Sender" , dm.sender}
            };

            var user = UserManager.GetUserById(dm.userId);
            IList<OutgoingTransfer> outgoingTransfers = new List<OutgoingTransfer>();

            if (user.BranchId != null)
            {
                outgoingTransfers = _outgoingTransferManager.Get(dic).Where(x=>x.FromBranchId == user.BranchId).ToList();
            }
            else
            {
                outgoingTransfers = _outgoingTransferManager.Get(dic).ToList();
            }

            IEnumerable<ReadOutgoingTransferDto> data = ObjectMapper.Map<List<ReadOutgoingTransferDto>>(outgoingTransfers);
            var operations = new DataOperations();

            IEnumerable groupDs = new List<ReadOutgoingTransferDto>();
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

        private async Task<Customer> CreateOrUpdateCustomer(CustomerDto customerDto)
        {
            var customer = ObjectMapper.Map<Customer>(customerDto);
            return await _customerManager.CreateOrUpdateAsync(customer);
        }

        private WhereFilter GetWhereFilter(List<WhereFilter> filterOptions, string name)
        {
            var filter = filterOptions.FirstOrDefault(x => x.Field == name);
            if (filter != null)
                return filter;

            foreach (var option in filterOptions)
            {
                if(option.predicates != null)
                    return GetWhereFilter(option.predicates, name);
            }

            return null;
        }

        public int GetLastNumber()
        {
            var currentBranch = AsyncHelper.RunSync(GetCurrentBranch);
            return _outgoingTransferManager.GetLastNumber(currentBranch.Id);
        }

        public async Task<IList<ReadOutgoingTransferDto>> GetAllNotCompleted()
        {
            IList<OutgoingTransfer> transfers = new List<OutgoingTransfer>();
            IList<Client> clients = new List<Client>();
            IList<Company> companies = new List<Company>();

            var user = await GetCurrentUserAsync();
            if (user.BranchId != null)
            {
                transfers = _outgoingTransferManager.GetAllNotCompleted((int)user.BranchId);
                clients = await _clientManager.GetAllForCurrentBranchAsync((int)user.BranchId);
                companies = await _companyManager.GetAllForCurrentBranchAsync((int)user.BranchId);

            }
            else
            {
                transfers = _outgoingTransferManager.GetAllNotCompleted(null);
            }
                var dto = ObjectMapper
                    .Map<IList<ReadOutgoingTransferDto>>
                    (transfers);

            for (int i = 0; i < dto.Count; i++)
            {
                if (clients.Any(x => x.Name.Trim().ToLower().Replace(" ", "").Replace("أ", "ا") == dto[i].beneficiary.Name.Trim().ToLower().Replace(" ", "").Replace("أ", "ا")))
                {
                    dto[i].beneficiary.Name = clients.FirstOrDefault(x => x.Name.Trim().ToLower().Replace(" ", "").Replace("أ", "ا") == dto[i].beneficiary.Name.Trim().ToLower().Replace(" ", "").Replace("أ", "ا")).Name + " - عميل";
                }
                else if (companies.Any(x => x.Name.Trim().ToLower().Replace(" ", "").Replace("أ", "ا") == dto[i].beneficiary.Name.Trim().ToLower().Replace(" ", "").Replace("أ", "ا")))
                {
                    dto[i].beneficiary.Name = companies.FirstOrDefault(x => x.Name.Trim().ToLower().Replace(" ", "").Replace("أ", "ا") == dto[i].beneficiary.Name.Trim().ToLower().Replace(" ", "").Replace("أ", "ا")).Name + " - شركة";
                }

                var stringDate = transfers[i].Date.ToString("dd-MM-yyyy");
                dto[i].date = stringDate;
            }

            return dto;
        }

        public async Task<OutgoingTransferDto> AcceptOutgoingTransferFromBranchAsync(int outgoingTransferId)
        {
            var branch = await GetCurrentBranch();
            var outgoingTransfer = await _outgoingTransferManager.AcceptOutgoingTransferFromBranchAsync(outgoingTransferId, branch.Id);
            return ObjectMapper.Map<OutgoingTransferDto>(outgoingTransfer);
        }

        public async Task<OutgoingTransferDto> RejectOutgoingTransferFromBranchasync(int outgoingTransfer)
        {
            var outgoing = await _outgoingTransferManager.RejectOutgoingTransferFromBranchAsync((int)outgoingTransfer);
            return ObjectMapper.Map<OutgoingTransferDto>(outgoing);
        }

        public async Task<Dictionary<string, double>> GetAllOutgoingTransfersForBranch(long userId)
        {
            var user = await UserManager.GetUserByIdAsync(userId);
            if (user.BranchId != null)
            {
                return await _outgoingTransferManager.GetAllForBranch((int)user.BranchId);
            }
            else
            {
                return await _outgoingTransferManager.GetAllForBranch(null);
            }

        }
    }
}
