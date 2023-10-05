using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Events.Bus;
using Bwr.Exchange.CashFlows.ClientCashFlows.Events;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Bwr.Exchange.TreasuryActions.Services.Implement
{
    public class ReceiptMainAccountIncomesExchangePartyClientDomainService : ITreasuryActionDomainService
    {
        private readonly IRepository<TreasuryAction> _treasuryActionRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        public TreasuryAction TreasuryAction { get; set; }

        public ReceiptMainAccountIncomesExchangePartyClientDomainService(
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
                    new ClientCashFlowCreatedEventData(
                        TreasuryAction.Date,
                        TreasuryAction.CurrencyId,
                        TreasuryAction.ExchangePartyClientId,
                        treasuryActionId,
                        CashFlows.TransactionType.TreasuryAction,
                        (TreasuryAction.Amount),
                        TransactionConst.SpendToHim,
                        0.0,
                        string.Empty,
                        0.0,
                        TreasuryAction.Income.Name
                        ));

                unitOfWork.Complete();
            }
            return TreasuryAction;
        }

        private TreasuryAction GetByIdWithDetail(int id)
        {
            return _treasuryActionRepository.GetAllIncluding(ex => ex.Income).FirstOrDefault(x => x.Id == id);
        }

        public async Task<TreasuryAction> UpdateTreasuryActionAsync()
        {
            using (var unitOfWork = _unitOfWorkManager.Begin())
            {
                var updatedTreasuryAction = await _treasuryActionRepository.UpdateAsync(TreasuryAction);

                await _treasuryActionRepository.EnsurePropertyLoadedAsync(updatedTreasuryAction, x => x.Income);

                EventBus.Default.Trigger(
                    new ClientCashFlowCreatedEventData(
                        TreasuryAction.Date,
                        TreasuryAction.CurrencyId,
                        TreasuryAction.ExchangePartyClientId,
                        TreasuryAction.Id,
                        CashFlows.TransactionType.TreasuryAction,
                        (TreasuryAction.Amount),
                        TransactionConst.SpendToHim,
                        0.0,
                        string.Empty,
                        0.0,
                        TreasuryAction.Income.Name
                        ));

                unitOfWork.Complete();
            }
            return TreasuryAction;
        }
    }
}
