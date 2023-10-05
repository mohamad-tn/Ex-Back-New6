using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Events.Bus.Handlers;
using Bwr.Exchange.Settings.Companies;
using System.Linq;
using System.Threading.Tasks;

namespace Bwr.Exchange.Settings.Companys.Events
{
    public class DeleteCompanyBalanceEventHandler : IAsyncEventHandler<DeleteCompanyBalanceEventData>, ITransientDependency
    {
        private readonly IRepository<CompanyBalance> _companyBalanceRepository;

        public DeleteCompanyBalanceEventHandler(IRepository<CompanyBalance> companyBalanceRepository)
        {
            _companyBalanceRepository = companyBalanceRepository;
        }

        public async Task HandleEventAsync(DeleteCompanyBalanceEventData eventData)
        {
            var companyBalances = await _companyBalanceRepository.GetAllListAsync(x => x.CurrencyId == eventData.CurrencyId);
            if (companyBalances.Any())
            {
                foreach (var companyBalance in companyBalances)
                {
                    await _companyBalanceRepository.DeleteAsync(companyBalance);
                }
            }
        }
    }
}
