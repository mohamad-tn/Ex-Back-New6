using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Events.Bus;
using Bwr.Exchange.CashFlows.CompanyCashFlows.Events;
using Bwr.Exchange.CashFlows.TreasuryCashFlows.Events;
using System.Linq;
using System.Threading.Tasks;

namespace Bwr.Exchange.Transfers.OutgoingTransfers.Services.Implements
{
    public class OutgoingTransferFromCashDomainService : IOutgoingTransferDomainService
    {
        private readonly IRepository<OutgoingTransfer> _outgoingTransferRepository;

        public OutgoingTransferFromCashDomainService(IRepository<OutgoingTransfer> outgoingTransferRepository)
        {
            _outgoingTransferRepository = outgoingTransferRepository;
        }

        //private readonly IUnitOfWorkManager _unitOfWorkManager;
        //public OutgoingTransferFromCashDomainService(
        //    IRepository<OutgoingTransfer> outgoingTransferRepository,
        //    IUnitOfWorkManager unitOfWorkManager
        //    )
        //{
        //    _outgoingTransferRepository = outgoingTransferRepository;
        //    _unitOfWorkManager = unitOfWorkManager;
        //}

        //public async Task<OutgoingTransfer> CreateAsync(OutgoingTransfer outgoingTransfer)
        //{
        //    var createdOutgoingTransfer = new OutgoingTransfer();
        //    using (var unitOfWork = _unitOfWorkManager.Begin())
        //    {
        //        var id = await _outgoingTransferRepository.InsertAndGetIdAsync(outgoingTransfer);

        //        await GetByIdWithDetail(createdOutgoingTransfer);

        //        await CreateCashFlowAsync(createdOutgoingTransfer);


        //        unitOfWork.Complete();
        //    }
        //    return createdOutgoingTransfer;

        //}

        //public async Task<OutgoingTransfer> UpdateAsync(OutgoingTransfer outgoingTransfer)
        //{
        //    var updatedTreasuryAction = new OutgoingTransfer();

        //    using (var unitOfWork = _unitOfWorkManager.Begin())
        //    {
        //        updatedTreasuryAction = await _outgoingTransferRepository.UpdateAsync(outgoingTransfer);

        //        await GetByIdWithDetail(updatedTreasuryAction);

        //        await CreateCashFlowAsync(updatedTreasuryAction);

        //        unitOfWork.Complete();
        //    }

        //    return updatedTreasuryAction;
        //}


        public async Task CreateCashFlowAsync(OutgoingTransfer outgoingTransfer)
        {
            await GetByIdWithDetail(outgoingTransfer);

            await EventBus.Default.TriggerAsync(
                    new CompanyCashFlowCreatedEventData()
                    {
                        Date = outgoingTransfer.Date,
                        CurrencyId = outgoingTransfer.CurrencyId,
                        CompanyId = outgoingTransfer.ToCompanyId,
                        TransactionId = outgoingTransfer.Id,
                        TransactionType = CashFlows.TransactionType.OutgoingTransfer,
                        Amount = (outgoingTransfer.Amount * -1),
                        Type = TransactionConst.TransferToHim,
                        Commission = 0.0,
                        InstrumentNo = outgoingTransfer.InstrumentNo,
                        CompanyCommission = (outgoingTransfer.CompanyCommission * -1),
                        Note = outgoingTransfer.Sender.Name,
                        Beneficiary = outgoingTransfer.Beneficiary.Name,
                        Sender = outgoingTransfer.Sender.Name,
                        Destination = outgoingTransfer.Country.Name
                    });

            await EventBus.Default.TriggerAsync(
                new TreasuryCashFlowCreatedEventData()
                {
                    Date = outgoingTransfer.Date,
                    CurrencyId = outgoingTransfer.CurrencyId,
                    TreasuryId = outgoingTransfer.TreasuryId,
                    TransactionId = outgoingTransfer.Id,
                    TransactionType = CashFlows.TransactionType.OutgoingTransfer,
                    Amount = (outgoingTransfer.Amount) + (outgoingTransfer.Commission),
                    Type = TransactionConst.Receipt,
                    InstrumentNo = outgoingTransfer.InstrumentNo,
                    Name = TransactionConst.CashTransfer,
                    Beneficiary = outgoingTransfer.Beneficiary.Name,
                    Sender = outgoingTransfer.Sender.Name,
                    Destination = outgoingTransfer.Country.Name
                });
        }

        public async Task<bool> DeleteCashFlowAsync(OutgoingTransfer outgoingTransfer)
        {
            try
            {
                //await EventBus.Default.TriggerAsync(new ClientCashFlowDeletedEventData(outgoingTransferId, CashFlows.TransactionType.OutgoingTransfer));
                await EventBus.Default.TriggerAsync(new CompanyCashFlowDeletedEventData(outgoingTransfer.Id, CashFlows.TransactionType.OutgoingTransfer, TransactionConst.TransferToHim));
                await EventBus.Default.TriggerAsync(new TreasuryCashFlowDeletedEventData(outgoingTransfer.Id, CashFlows.TransactionType.OutgoingTransfer));
                return true;
            }
            catch
            {
                return false;
            }
        }

        private async Task GetByIdWithDetail(OutgoingTransfer outgoingTransfer)
        {
            await _outgoingTransferRepository.EnsurePropertyLoadedAsync(outgoingTransfer, x => x.Sender);
            await _outgoingTransferRepository.EnsurePropertyLoadedAsync(outgoingTransfer, x => x.Beneficiary);
            await _outgoingTransferRepository.EnsurePropertyLoadedAsync(outgoingTransfer, x => x.Country);
        }

        //private OutgoingTransfer GetByIdWithDetail(int id)
        //{
        //    return _outgoingTransferRepository.
        //        GetAllIncluding(
        //            m => m.ToCompany,
        //            e => e.FromCompany,
        //            s => s.Sender,
        //            b => b.Beneficiary,
        //            ds => ds.Country)
        //        .FirstOrDefault(x => x.Id == id);
        //}
    }
}
