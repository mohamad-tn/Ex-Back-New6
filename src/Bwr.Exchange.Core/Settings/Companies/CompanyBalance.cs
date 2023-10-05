using Abp.Domain.Entities.Auditing;
using Bwr.Exchange.Settings.Currencies;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bwr.Exchange.Settings.Companies
{
    public class CompanyBalance: FullAuditedEntity
    {
        public double Balance { get; set; }

        #region Currency
        public int CurrencyId { get; set; }
        [ForeignKey("CurrencyId")]
        public virtual Currency Currency { get; set; }
        #endregion

        #region Company
        public int CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; }
        #endregion 
    }
}
