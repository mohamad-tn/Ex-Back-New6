using System;

namespace Bwr.Exchange.CashFlows.CompanyCashFlows.Dto
{
    public class GetCompanyCashFlowInput
    {
        public int CompanyId { get; set; }
        public int CurrencyId { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
    }
}
