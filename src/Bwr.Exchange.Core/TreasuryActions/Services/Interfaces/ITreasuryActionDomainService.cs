using Abp.Dependency;
using Abp.Domain.Services;
using System.Threading.Tasks;

namespace Bwr.Exchange.TreasuryActions.Services
{
    public interface ITreasuryActionDomainService :IDomainService
    {
        Task<TreasuryAction> CreateTreasuryActionAsync();
        Task<TreasuryAction> UpdateTreasuryActionAsync();
    }
}
