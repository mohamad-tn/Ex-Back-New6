using Bwr.Exchange.CashFlows.Shared.Dto;
using Bwr.Exchange.Settings.Companies.Dto;

namespace Bwr.Exchange.CashFlows.CompanyCashFlows.Dto
{
    public class CompanyCashFlowDto : CashFlowDto
    {

        #region Company 
        public int CompanyId { get; set; }
        public CompanyDto Company { get; set; }
        #endregion

        public double Commission { get; set; }
        public double CompanyCommission { get; set; }
        public double Balance { get; set; }
    }
}
