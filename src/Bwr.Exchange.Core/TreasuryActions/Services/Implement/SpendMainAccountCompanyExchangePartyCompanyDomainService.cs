using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Events.Bus;
using Bwr.Exchange.CashFlows.CompanyCashFlows.Events;
using Bwr.Exchange.CashFlows.TreasuryCashFlows.Events;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Bwr.Exchange.TreasuryActions.Services.Implement
{
    public class SpendMainAccountCompanyExchangePartyCompanyDomainService : ITreasuryActionDomainService
    {
        private readonly IRepository<TreasuryAction> _treasuryActionRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        public TreasuryAction TreasuryAction { get; set; }

        public SpendMainAccountCompanyExchangePartyCompanyDomainService(
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
                    new CompanyCashFlowCreatedEventData(
                        TreasuryAction.Date,
                        TreasuryAction.CurrencyId,
                        TreasuryAction.ExchangePartyCompanyId,
                        treasuryActionId,
                        CashFlows.TransactionType.TreasuryAction,
                        (TreasuryAction.Amount * -1),
                        TransactionConst.ReceiptFromHim,
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
                        TreasuryAction.Amount,
                        TransactionConst.Spend,
                        0.0,
                        TreasuryAction.InstrumentNo,
                        0.0,
                        TreasuryAction.ExchangePartyCompany.Name
                        ));

                unitOfWork.Complete();
            }
            return TreasuryAction;
        }

        private TreasuryAction GetByIdWithDetail(int id)
        {
            var teasuryAction = _treasuryActionRepository.Get(id);
            _treasuryActionRepository.EnsurePropertyLoaded(teasuryAction, x => x.MainAccountCompany);
            _treasuryActionRepository.EnsurePropertyLoaded(teasuryAction, x => x.ExchangePartyCompany);

            return teasuryAction;
        }

        public async Task<TreasuryAction> UpdateTreasuryActionAsync()
        {
            using (var unitOfWork = _unitOfWorkManager.Begin())
            {
                var updatedTreasuryAction = await _treasuryActionRepository.UpdateAsync(TreasuryAction);

                TreasuryAction = GetByIdWithDetail(updatedTreasuryAction.Id);

                EventBus.Default.Trigger(
                    new CompanyCashFlowCreatedEventData(
                        TreasuryAction.Date,
                        TreasuryAction.CurrencyId,
                        TreasuryAction.ExchangePartyCompanyId,
                        TreasuryAction.Id,
                        CashFlows.TransactionType.TreasuryAction,
                        (TreasuryAction.Amount * -1),
                        TransactionConst.ReceiptFromHim,
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
                        TreasuryAction.Amount,
                        TransactionConst.Spend,
                        0.0,
                        TreasuryAction.InstrumentNo,
                        0.0,
                        TreasuryAction.ExchangePartyCompany.Name
                        ));

                unitOfWork.Complete();
            }
            return TreasuryAction;
        }
    }
}
