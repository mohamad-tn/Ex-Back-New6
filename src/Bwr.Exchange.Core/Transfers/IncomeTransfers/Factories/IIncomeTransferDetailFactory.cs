using Abp.Dependency;
using Abp.Domain.Uow;
using Bwr.Exchange.Transfers.IncomeTransfers.Services.Interfaces;

namespace Bwr.Exchange.Transfers.IncomeTransfers.Factories
{
    public interface IIncomeTransferDetailFactory : ITransientDependency
    {
        IIncomeTransferDetailDomainService CreateService(IncomeTransferDetail input);
    }
}
