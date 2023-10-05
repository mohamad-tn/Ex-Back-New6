using Abp.Domain.Repositories;
using System.Threading.Tasks;

namespace Bwr.Exchange.Transfers.OutgoingTransfers.Services.Implements
{
    public class OutgoingTransferFromBranchDomainService : IOutgoingTransferDomainService
    {
        private readonly IRepository<OutgoingTransfer> _outgoingTransferRepository;

        public OutgoingTransferFromBranchDomainService(IRepository<OutgoingTransfer> outgoingTransferRepository)
        {
            _outgoingTransferRepository = outgoingTransferRepository;
        }

        public Task CreateCashFlowAsync(OutgoingTransfer outgoingTransfer)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> DeleteCashFlowAsync(OutgoingTransfer outgoingTransfer)
        {
            throw new System.NotImplementedException();
        }
    }
}
