using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Events.Bus;
using Bwr.Exchange.CashFlows.TreasuryCashFlows.Events;
using Bwr.Exchange.Transfers.IncomeTransfers;
using Bwr.Exchange.Transfers.IncomeTransfers.Events;
using Bwr.Exchange.Transfers.IncomeTransfers.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Bwr.Exchange.TreasuryActions.Services.Implement
{
    public class SpendMainAccountDirectTransferExchangePartyTreasuryDomainService : ITreasuryActionDomainService
    {
        private readonly IRepository<TreasuryAction> _treasuryActionRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IIncomeTransferDetailManager _incomeTransferDetailManager;
        public TreasuryAction TreasuryAction { get; set; }

        public SpendMainAccountDirectTransferExchangePartyTreasuryDomainService(
            IRepository<TreasuryAction> treasuryActionRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IIncomeTransferDetailManager incomeTransferDetailManager,
            TreasuryAction treasuryAction)
        {
            _treasuryActionRepository = treasuryActionRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _incomeTransferDetailManager = incomeTransferDetailManager;
            TreasuryAction = treasuryAction;
        }

        public async Task<TreasuryAction> CreateTreasuryActionAsync()
        {
            int treasuryActionId;
            using (var unitOfWork = _unitOfWorkManager.Begin())
            {
                treasuryActionId = await _treasuryActionRepository.InsertAndGetIdAsync(TreasuryAction);

                var name = string.Empty;
                if (TreasuryAction.IncomeTransferDetailId != null)
                {
                    var incomeTransferDetail = await GetIncomeTransferDetail(TreasuryAction.IncomeTransferDetailId);
                    if (incomeTransferDetail != null && incomeTransferDetail.Beneficiary != null)
                    {
                        name = TreasuryAction.IncomeTransferDetail.Beneficiary.Name;
                    }
                }

                EventBus.Default.Trigger(
                    new TreasuryCashFlowCreatedEventData(
                        TreasuryAction.Date,
                        TreasuryAction.CurrencyId,
                        TreasuryAction.TreasuryId,
                        treasuryActionId,
                        CashFlows.TransactionType.TreasuryAction,
                        (TreasuryAction.Amount * -1),
                        TransactionConst.DirectTransfer,
                        name,
                        string.Empty
                        ));

                EventBus.Default.Trigger(
                    new ChangeIncomeTransferDetailStatusEventData(TreasuryAction.IncomeTransferDetail, IncomeTransferDetailStatus.Received));

                unitOfWork.Complete();
            }

            return TreasuryAction;
        }

        private TreasuryAction GetByIdWithDetail(int id)
        {
            return _treasuryActionRepository.GetAll().Include(x=>x.IncomeTransferDetail)
                .ThenInclude(x=>x.Beneficiary).FirstOrDefault(x => x.Id == id);
        }

        private async Task<IncomeTransferDetail> GetIncomeTransferDetail(int? incomeTransferDetailId)
        {
            return await _incomeTransferDetailManager.GetByIdAsync(incomeTransferDetailId.Value, x => x.Beneficiary);
        }

        public async Task<TreasuryAction> UpdateTreasuryActionAsync()
        {
            using (var unitOfWork = _unitOfWorkManager.Begin())
            {
                var updatedTreasuryAction = await _treasuryActionRepository.UpdateAsync(TreasuryAction);

                var name = string.Empty;
                if (TreasuryAction.IncomeTransferDetailId != null)
                {
                    var incomeTransferDetail = await GetIncomeTransferDetail(TreasuryAction.IncomeTransferDetailId);
                    if (incomeTransferDetail != null && incomeTransferDetail.Beneficiary != null)
                    {
                        name = TreasuryAction.IncomeTransferDetail.Beneficiary.Name;
                    }
                }

                EventBus.Default.Trigger(
                    new TreasuryCashFlowCreatedEventData(
                        updatedTreasuryAction.Date,
                        updatedTreasuryAction.CurrencyId,
                        updatedTreasuryAction.TreasuryId,
                        updatedTreasuryAction.Id,
                        CashFlows.TransactionType.TreasuryAction,
                        (updatedTreasuryAction.Amount * -1),
                        TransactionConst.DirectTransfer,
                        name,
                        string.Empty
                        ));

                EventBus.Default.Trigger(
                    new ChangeIncomeTransferDetailStatusEventData(updatedTreasuryAction.IncomeTransferDetail, IncomeTransferDetailStatus.Received));

                await _unitOfWorkManager.Current.SaveChangesAsync();
                await unitOfWork.CompleteAsync();
            }
            return TreasuryAction;
        }
    }
}
