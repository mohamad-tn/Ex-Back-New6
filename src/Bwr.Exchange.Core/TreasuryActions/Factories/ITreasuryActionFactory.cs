using Abp.Dependency;
using Bwr.Exchange.TreasuryActions.Services;

namespace Bwr.Exchange.TreasuryActions.Factories
{
    public interface ITreasuryActionFactory: ITransientDependency
    {
        ITreasuryActionDomainService CreateService(TreasuryAction input);
    }
}
