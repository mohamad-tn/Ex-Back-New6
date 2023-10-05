using Abp.Domain.Services;
using System.Threading.Tasks;

namespace Bwr.Exchange.CashFlows.CashFlowMatchings.Services
{
    public interface ICashFlowMatchingManager : IDomainService
    {
        Task<CashFlowMatching> CreateAsync(CashFlowMatching cashFlowMatching);
    }
}
