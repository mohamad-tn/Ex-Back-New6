using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Events.Bus.Handlers;
using Bwr.Exchange.Settings.Companies;
using Bwr.Exchange.Settings.Companies.Services;
using System.Linq;
using System.Threading.Tasks;

namespace Bwr.Exchange.Settings.Companys.Events
{
    public class AddCompanyBalanceEventHandler : IAsyncEventHandler<AddCompanyBalanceEventData>, ITransientDependency
    {
        private readonly IRepository<CompanyBalance> _companyBalanceRepository;
        private readonly ICompanyManager _companyManager;

        public AddCompanyBalanceEventHandler(
            IRepository<CompanyBalance> companyBalanceRepository, 
            ICompanyManager companyManager)
        {
            _companyBalanceRepository = companyBalanceRepository;
            _companyManager = companyManager;
        }

        public async Task HandleEventAsync(AddCompanyBalanceEventData eventData)
        {
            var companys = await _companyManager.GetAllAsync();
            var currentCompanies = companys.Where(x => x.BranchId == eventData.BranchId);
            if (currentCompanies.Any())
            {
                foreach (var company in currentCompanies)
                {
                    var companyBalance = new CompanyBalance()
                    {
                        CompanyId = company.Id,
                        CurrencyId = eventData.CurrencyId,
                        Balance = 0
                    };

                    await _companyBalanceRepository.InsertAsync(companyBalance);
                }
                
            }
        }
    }
}
