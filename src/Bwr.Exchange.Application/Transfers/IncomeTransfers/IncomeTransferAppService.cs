using Abp.Domain.Uow;
using Bwr.Exchange.Customers;
using Bwr.Exchange.Customers.Dto;
using Bwr.Exchange.Customers.Services;
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
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public IncomeTransferAppService(
            IIncomeTransferManager incomeTransferManager, 
            ITreasuryActionManager treasuryActionManager, 
            ICustomerManager customerManager, 
            ITreasuryManager treasuryManager, 
            IUnitOfWorkManager unitOfWorkManager)
        {
            _incomeTransferManager = incomeTransferManager;
            _treasuryActionManager = treasuryActionManager;
            _customerManager = customerManager;
            _treasuryManager = treasuryManager;
            _unitOfWorkManager = unitOfWorkManager;
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
