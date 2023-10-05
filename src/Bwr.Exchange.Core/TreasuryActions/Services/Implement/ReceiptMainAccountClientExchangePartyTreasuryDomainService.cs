using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Events.Bus;
using Bwr.Exchange.CashFlows.ClientCashFlows.Events;
using Bwr.Exchange.CashFlows.TreasuryCashFlows.Events;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Bwr.Exchange.TreasuryActions.Services.Implement
{
    public class ReceiptMainAccountClientExchangePartyTreasuryDomainService : ITreasuryActionDomainService
    {
        private readonly IRepository<TreasuryAction> _treasuryActionRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        public TreasuryAction TreasuryAction { get; set; }

        public ReceiptMainAccountClientExchangePartyTreasuryDomainService(
            IRepository<TreasuryAction> treasuryActionRepository,
            IUnitOfWorkManager unitOfWorkManager,
            TreasuryAction treasuryAction
            )
        {
            _treasuryActionRepository = treasuryActionRepository;
            _unitOfWorkManager = unitOfWorkManager;
            TreasuryAction = treasuryAction;
        }

        public async Task<TreasuryAction> CreateTreasuryActionAsync()
        {
            int treasuryActionId;
            using (var unitOfWork = _unitOfWorkManager.Begin())
            {
                treasuryActionId = await _treasuryActionRepository.InsertAndGetIdAsync(TreasuryAction);

                TreasuryAction = GetByIdWithDetail(treasuryActionId);

                EventBus.Default.Trigger(
                    new TreasuryCashFlowCreatedEventData(
                        TreasuryAction.Date,
                        TreasuryAction.CurrencyId,
                        TreasuryAction.TreasuryId,
                        treasuryActionId,
                        CashFlows.TransactionType.TreasuryAction,
                        (TreasuryAction.Amount),
                        TransactionConst.ClientReceivable,
                        TreasuryAction.MainAccountClient.Name
                        ));

                EventBus.Default.Trigger(
                    new ClientCashFlowCreatedEventData(
                        TreasuryAction.Date,
                        TreasuryAction.CurrencyId,
                        TreasuryAction.MainAccountClientId,
                        treasuryActionId,
                        CashFlows.TransactionType.TreasuryAction,
                        (TreasuryAction.Amount * -1),
                        TransactionConst.Receipt
                        ));

                unitOfWork.Complete();
            }
            return TreasuryAction;
        }

        private TreasuryAction GetByIdWithDetail(int id)
        {
            return _treasuryActionRepository.GetAllIncluding(ex => ex.MainAccountClient).FirstOrDefault(x => x.Id == id);
        }

        public async Task<TreasuryAction> UpdateTreasuryActionAsync()
        {
            using (var unitOfWork = _unitOfWorkManager.Begin())
            {
                var updatedTreasuryAction = await _treasuryActionRepository.UpdateAsync(TreasuryAction);

                await _treasuryActionRepository.EnsurePropertyLoadedAsync(updatedTreasuryAction, x => x.MainAccountClient);

                EventBus.Default.Trigger(
                    new TreasuryCashFlowCreatedEventData(
                        TreasuryAction.Date,
                        TreasuryAction.CurrencyId,
                        TreasuryAction.TreasuryId,
                        TreasuryAction.Id,
                        CashFlows.TransactionType.TreasuryAction,
                        (TreasuryAction.Amount),
                        TransactionConst.ClientReceivable,
                        TreasuryAction.MainAccountClient.Name
                        ));

                EventBus.Default.Trigger(
                    new ClientCashFlowCreatedEventData(
                        TreasuryAction.Date,
                        TreasuryAction.CurrencyId,
                        TreasuryAction.MainAccountClientId,
                        TreasuryAction.Id,
                        CashFlows.TransactionType.TreasuryAction,
                        (TreasuryAction.Amount * -1),
                        TransactionConst.Receipt
                        ));

                unitOfWork.Complete();
            }
            return TreasuryAction;
        }
    }
}
