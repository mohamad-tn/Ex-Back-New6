using Bwr.Exchange.Settings.Companies;
using Bwr.Exchange.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bwr.Exchange.CashFlows.CompanyCashFlows
{
    public class CompanyCashFlow : CashFlowBase
    {
        #region Company 
        public int CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; }
        #endregion
        public double CompanyCommission { get; set; }
        
    }
}
