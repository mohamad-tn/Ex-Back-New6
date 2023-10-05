using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Events.Bus;
using Bwr.Exchange.CashFlows.ClientCashFlows.Events;
using Bwr.Exchange.CashFlows.TreasuryCashFlows.Events;
using Bwr.Exchange.Transfers.IncomeTransfers;
using Bwr.Exchange.Transfers.IncomeTransfers.Events;
using Bwr.Exchange.Transfers.IncomeTransfers.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Bwr.Exchange.TreasuryActions.Services.Implement
{
    public class SpendMainAccountDirectTransferExchangePartyClientDomainService : ITreasuryActionDomainService
    {
        private readonly IRepository<TreasuryAction> _treasuryActionRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IIncomeTransferDetailManager _incomeTransferDetailManager;
        public TreasuryAction TreasuryAction { get; set; }

        public SpendMainAccountDirectTransferExchangePartyClientDomainService(
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

                TreasuryAction = await GetByIdWithDetail(treasuryActionId);
                
                var name = string.Empty;
                if (TreasuryAction.IncomeTransferDetailId != null)
                {
                    var incomeTransferDetail = await GetIncomeTransferDetail(TreasuryAction.IncomeTransferDetailId);
                    if(incomeTransferDetail != null && incomeTransferDetail.Beneficiary != null)
                    {
                        name = TreasuryAction.IncomeTransferDetail.Beneficiary.Name;
                    }
                }

                EventBus.Default.Trigger(
                    new ClientCashFlowCreatedEventData(
                        TreasuryAction.Date,
                        TreasuryAction.CurrencyId,
                        TreasuryAction.ExchangePartyClientId,
                        treasuryActionId,
                        CashFlows.TransactionType.TreasuryAction,
                        (TreasuryAction.Amount * -1),
                        TransactionConst.ReceiptFromHim,
                        0.0,
                        TreasuryAction.InstrumentNo,
                        0.0,
                        name
                        ));

                EventBus.Default.Trigger(
                    new ChangeIncomeTransferDetailStatusEventData(TreasuryAction.IncomeTransferDetail, IncomeTransferDetailStatus.Received));

                unitOfWork.Complete();
            }

            return TreasuryAction;
        }

        private async Task<IncomeTransferDetail> GetIncomeTransferDetail(int? incomeTransferDetailId)
        {
            return await _incomeTransferDetailManager.GetByIdAsync(incomeTransferDetailId.Value ,x=>x.Beneficiary);
        }

        private async Task<TreasuryAction> GetByIdWithDetail(int id)
        {
            var teasuryAction = await _treasuryActionRepository.GetAsync(id);
            await _treasuryActionRepository.EnsurePropertyLoadedAsync(teasuryAction, x => x.MainAccountClient);
            
            return teasuryAction;
        }

        public async Task<TreasuryAction> UpdateTreasuryActionAsync()
        {
            using (var unitOfWork = _unitOfWorkManager.Begin())
            {
                var updatedTreasuryAction = await _treasuryActionRepository.UpdateAsync(TreasuryAction);

                TreasuryAction = await GetByIdWithDetail(updatedTreasuryAction.Id);

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
                    new ClientCashFlowCreatedEventData(
                        TreasuryAction.Date,
                        TreasuryAction.CurrencyId,
                        TreasuryAction.ExchangePartyClientId,
                        updatedTreasuryAction.Id,
                        CashFlows.TransactionType.TreasuryAction,
                        (TreasuryAction.Amount * -1),
                        TransactionConst.ReceiptFromHim,
                        0.0,
                        TreasuryAction.InstrumentNo,
                        0.0,
                        name
                        ));

                EventBus.Default.Trigger(
                    new ChangeIncomeTransferDetailStatusEventData(TreasuryAction.IncomeTransferDetail, IncomeTransferDetailStatus.Received));

                unitOfWork.Complete();
            }
            return TreasuryAction;
        }
    }
}
