using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Events.Bus;
using Bwr.Exchange.CashFlows.ClientCashFlows.Events;
using Bwr.Exchange.CashFlows.CompanyCashFlows.Events;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Bwr.Exchange.TreasuryActions.Services.Implement
{
    public class ReceiptMainAccountCompanyExchangePartyClientDomainService : ITreasuryActionDomainService
    {
        private readonly IRepository<TreasuryAction> _treasuryActionRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        public TreasuryAction TreasuryAction { get; set; }

        public ReceiptMainAccountCompanyExchangePartyClientDomainService(
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
                        TreasuryAction.InstrumentNo,
                        0.0,
                        TreasuryAction.MainAccountCompany.Name
                        ));

                EventBus.Default.Trigger(
                    new CompanyCashFlowCreatedEventData(
                        TreasuryAction.Date,
                        TreasuryAction.CurrencyId,
                        TreasuryAction.MainAccountCompanyId,
                        treasuryActionId,
                        CashFlows.TransactionType.TreasuryAction,
                        (TreasuryAction.Amount * -1),
                        TransactionConst.Receipt,
                        0.0,
                        TreasuryAction.InstrumentNo,
                        0.0,
                        TreasuryAction.ExchangePartyClient.Name
                        ));

                unitOfWork.Complete();
            }
            return TreasuryAction;
        }

        private TreasuryAction GetByIdWithDetail(int id)
        {
            return _treasuryActionRepository.
                GetAllIncluding(
                    m => m.MainAccountCompany,
                    e => e.ExchangePartyClient)
                .FirstOrDefault(x => x.Id == id);
        }

        public async Task<TreasuryAction> UpdateTreasuryActionAsync()
        {
            using (var unitOfWork = _unitOfWorkManager.Begin())
            {
                var updatedTreasuryAction = await _treasuryActionRepository.UpdateAsync(TreasuryAction);

                await _treasuryActionRepository.EnsurePropertyLoadedAsync(updatedTreasuryAction, x => x.MainAccountCompany);
                await _treasuryActionRepository.EnsurePropertyLoadedAsync(updatedTreasuryAction, x => x.ExchangePartyClient);

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
                        TreasuryAction.InstrumentNo,
                        0.0,
                        TreasuryAction.MainAccountCompany.Name
                        ));

                EventBus.Default.Trigger(
                    new CompanyCashFlowCreatedEventData(
                        TreasuryAction.Date,
                        TreasuryAction.CurrencyId,
                        TreasuryAction.MainAccountCompanyId,
                        TreasuryAction.Id,
                        CashFlows.TransactionType.TreasuryAction,
                        (TreasuryAction.Amount * -1),
                        TransactionConst.Receipt,
                        0.0,
                        TreasuryAction.InstrumentNo,
                        0.0,
                        TreasuryAction.ExchangePartyClient.Name
                        ));

                unitOfWork.Complete();
            }
            return TreasuryAction;
        }
    }
}
