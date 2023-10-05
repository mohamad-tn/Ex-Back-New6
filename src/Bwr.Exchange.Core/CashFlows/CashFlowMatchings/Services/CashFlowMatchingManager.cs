using Abp.Domain.Repositories;
using System.Threading.Tasks;

namespace Bwr.Exchange.CashFlows.CashFlowMatchings.Services
{
    public class CashFlowMatchingManager: ICashFlowMatchingManager
    {
        private readonly IRepository<CashFlowMatching> _cashFlowMatchingRepository;

        public CashFlowMatchingManager(IRepository<CashFlowMatching> cashFlowMatchingRepository)
        {
            _cashFlowMatchingRepository = cashFlowMatchingRepository;
        }

        public async Task<CashFlowMatching> CreateAsync(CashFlowMatching cashFlowMatching)
        {
            var id = await _cashFlowMatchingRepository.InsertAndGetIdAsync(cashFlowMatching);
            return await _cashFlowMatchingRepository.GetAsync(id);
        }
    }
}
