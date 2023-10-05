using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Events.Bus;
using Bwr.Exchange.CashFlows.ClientCashFlows.Events;
using Bwr.Exchange.CashFlows.CompanyCashFlows.Events;
using Bwr.Exchange.CashFlows.TreasuryCashFlows.Events;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Bwr.Exchange.Transfers.OutgoingTransfers.Services.Implements
{
    public class OutgoingTransferFromClientDomainService : IOutgoingTransferDomainService
    {
        private readonly IRepository<OutgoingTransfer> _outgoingTransferRepository;

        public OutgoingTransferFromClientDomainService(IRepository<OutgoingTransfer> outgoingTransferRepository)
        {
            _outgoingTransferRepository = outgoingTransferRepository;
        }

        //private readonly IUnitOfWorkManager _unitOfWorkManager;
        //public OutgoingTransfer OutgoingTransfer { get; set; }

        //public OutgoingTransferFromClientDomainService(
        //    IRepository<OutgoingTransfer> outgoingTransferRepository,
        //    IUnitOfWorkManager unitOfWorkManager,
        //    OutgoingTransfer outgoingTransfer
        //    )
        //{
        //    _outgoingTransferRepository = outgoingTransferRepository;
        //    _unitOfWorkManager = unitOfWorkManager;
        //    OutgoingTransfer = outgoingTransfer;
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
                new ClientCashFlowCreatedEventData()
                {
                    Date = outgoingTransfer.Date,
                    CurrencyId = outgoingTransfer.CurrencyId,
                    ClientId = outgoingTransfer.FromClientId,
                    TransactionId = outgoingTransfer.Id,
                    TransactionType = CashFlows.TransactionType.OutgoingTransfer,
                    Amount = (outgoingTransfer.Amount),
                    Type = TransactionConst.TransferFromHim,
                    Commission = outgoingTransfer.Commission,
                    InstrumentNo = outgoingTransfer.InstrumentNo,
                    ClientCommission = 0.0,
                    Note = outgoingTransfer.Sender.Name,
                    Beneficiary = outgoingTransfer.Beneficiary.Name,
                    Sender = outgoingTransfer.Sender.Name,
                    Destination = outgoingTransfer.Country.Name
                });

            if (outgoingTransfer.ReceivedAmount != 0)
            {
                await CreateReceiptActionAsync(outgoingTransfer);
            }
        }

        public async Task<bool> DeleteCashFlowAsync(OutgoingTransfer outgoingTransfer)
        {
            try
            {
                await EventBus.Default.TriggerAsync(new ClientCashFlowDeletedEventData(outgoingTransfer.Id, CashFlows.TransactionType.OutgoingTransfer, TransactionConst.TransferFromHim));
                await EventBus.Default.TriggerAsync(new CompanyCashFlowDeletedEventData(outgoingTransfer.Id, CashFlows.TransactionType.OutgoingTransfer, TransactionConst.TransferToHim));
                await EventBus.Default.TriggerAsync(new ClientCashFlowDeletedEventData(outgoingTransfer.Id, CashFlows.TransactionType.OutgoingTransfer, TransactionConst.Receipt));
                await EventBus.Default.TriggerAsync(new TreasuryCashFlowDeletedEventData(outgoingTransfer.Id, CashFlows.TransactionType.OutgoingTransfer));
                return true;
            }
            catch
            {
                return false;
            }
        }

        private async Task CreateReceiptActionAsync(OutgoingTransfer outgoingTransfer)
        {
            await EventBus.Default.TriggerAsync(
                    new ClientCashFlowCreatedEventData()
                    {
                        Date = outgoingTransfer.Date,
                        CurrencyId = outgoingTransfer.CurrencyId,
                        ClientId = outgoingTransfer.FromClientId,
                        TransactionId = outgoingTransfer.Id,
                        TransactionType = CashFlows.TransactionType.OutgoingTransfer,
                        Amount = outgoingTransfer.Amount,
                        Commission = outgoingTransfer.Commission,
                        ReceivedAmount = outgoingTransfer.ReceivedAmount * -1,
                        Type = TransactionConst.Receipt,
                        InstrumentNo = outgoingTransfer.InstrumentNo,
                        Beneficiary = outgoingTransfer.Beneficiary.Name,
                        Sender = outgoingTransfer.Sender.Name,
                        Destination = outgoingTransfer.Country.Name
                    });

           await  EventBus.Default.TriggerAsync(
                    new TreasuryCashFlowCreatedEventData()
                    {
                        Date = outgoingTransfer.Date,
                        CurrencyId = outgoingTransfer.CurrencyId,
                        TreasuryId = outgoingTransfer.TreasuryId,
                        TransactionId = outgoingTransfer.Id,
                        TransactionType = CashFlows.TransactionType.OutgoingTransfer,
                        Name = outgoingTransfer.FromClient.Name,
                        Type = TransactionConst.Receipt,
                        Amount = outgoingTransfer.Amount,
                        Beneficiary = outgoingTransfer.Beneficiary.Name,
                        Sender = outgoingTransfer.Sender.Name
                    });
        }

        private async Task GetByIdWithDetail(OutgoingTransfer outgoingTransfer)
        {
            await _outgoingTransferRepository.EnsurePropertyLoadedAsync(outgoingTransfer, x => x.FromClient);
            await _outgoingTransferRepository.EnsurePropertyLoadedAsync(outgoingTransfer, x => x.Sender);
            await _outgoingTransferRepository.EnsurePropertyLoadedAsync(outgoingTransfer, x => x.Beneficiary);
            await _outgoingTransferRepository.EnsurePropertyLoadedAsync(outgoingTransfer, x => x.Country);
        }
    }
}
