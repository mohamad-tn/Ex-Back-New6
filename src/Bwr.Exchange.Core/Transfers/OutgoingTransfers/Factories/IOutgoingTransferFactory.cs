using Abp.Dependency;
using Bwr.Exchange.Transfers.OutgoingTransfers.Services;

namespace Bwr.Exchange.Transfers.OutgoingTransfers.Factories
{
    public interface IOutgoingTransferFactory : ITransientDependency
    {
        IOutgoingTransferDomainService CreateService(OutgoingTransfer input);
    }
}
