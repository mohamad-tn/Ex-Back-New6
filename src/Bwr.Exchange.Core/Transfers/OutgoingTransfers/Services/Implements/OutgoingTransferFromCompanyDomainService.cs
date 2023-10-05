using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Events.Bus;
using Bwr.Exchange.CashFlows.CompanyCashFlows.Events;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Bwr.Exchange.Transfers.OutgoingTransfers.Services.Implements
{
    public class OutgoingTransferFromCompanyDomainService : IOutgoingTransferDomainService
    {
        private readonly IRepository<OutgoingTransfer> _outgoingTransferRepository;

        public OutgoingTransferFromCompanyDomainService(IRepository<OutgoingTransfer> outgoingTransferRepository)
        {
            _outgoingTransferRepository = outgoingTransferRepository;
        }

        //private readonly IUnitOfWorkManager _unitOfWorkManager;

        //public OutgoingTransferFromCompanyDomainService(
        //    IRepository<OutgoingTransfer> outgoingTransferRepository,
        //    IUnitOfWorkManager unitOfWorkManager
        //    )
        //{
        //    _outgoingTransferRepository = outgoingTransferRepository;
        //    _unitOfWorkManager = unitOfWorkManager;
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
                new CompanyCashFlowCreatedEventData()
                {
                    Date = outgoingTransfer.Date,
                    CurrencyId = outgoingTransfer.CurrencyId,
                    CompanyId = outgoingTransfer.FromCompanyId,
                    TransactionId = outgoingTransfer.Id,
                    TransactionType = CashFlows.TransactionType.OutgoingTransfer,
                    Amount = (outgoingTransfer.Amount),
                    Type = TransactionConst.TransferFromHim,
                    Commission = outgoingTransfer.Commission,
                    InstrumentNo = outgoingTransfer.InstrumentNo,
                    CompanyCommission = 0.0,
                    Note = outgoingTransfer.Sender.Name,
                    Beneficiary = outgoingTransfer.Beneficiary.Name,
                    Sender = outgoingTransfer.Sender.Name,
                    Destination = outgoingTransfer.Country.Name
                });
        }
        

        public async Task<bool> DeleteCashFlowAsync(OutgoingTransfer outgoingTransfer)
        {
            try
            {
                
                await EventBus.Default.TriggerAsync(new CompanyCashFlowDeletedEventData(outgoingTransfer.Id, CashFlows.TransactionType.OutgoingTransfer, TransactionConst.TransferToHim));
                await EventBus.Default.TriggerAsync(new CompanyCashFlowDeletedEventData(outgoingTransfer.Id, CashFlows.TransactionType.OutgoingTransfer, TransactionConst.TransferFromHim));

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

    }
}
