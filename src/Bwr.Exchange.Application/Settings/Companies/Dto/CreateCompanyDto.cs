using Bwr.Exchange.Settings.Companies.Dto.CompanyBalances;
using System.Collections.Generic;

namespace Bwr.Exchange.Settings.Companies.Dto
{
    public class CreateCompanyDto
    {
        public CreateCompanyDto()
        {
            CompanyBalances = new List<CompanyBalanceDto>();
        }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public int BranchId { get; set; }
        public IList<CompanyBalanceDto> CompanyBalances { get; set; }
    }
}
