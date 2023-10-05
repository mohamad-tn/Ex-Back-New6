using System.Collections.Generic;

namespace Bwr.Exchange.CashFlows.CompanyCashFlow.Dto
{
    public class CompanyCashFlowTotalDto
    {
        public CompanyCashFlowTotalDto()
        {
            CurrencyBalances = new List<CompanyCashFlowTotalDetailDto>();
        }
        #region Company 
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public bool IsActiveToday { get; set; }
        public bool IsMatching { get; set; }
        #endregion

        public IList<CompanyCashFlowTotalDetailDto> CurrencyBalances { get; set; }
    }
}
