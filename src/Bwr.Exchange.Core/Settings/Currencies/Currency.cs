using Abp.Domain.Entities.Auditing;
using Bwr.Exchange.Settings.Branches;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bwr.Exchange.Settings.Currencies
{
    public class Currency : FullAuditedEntity
    {
        public string Name { get; set; }
        public bool IsMainCurrency { get; set; }

        #region Branch
        public int? BranchId { get; set; }

        [ForeignKey("BranchId")]
        public virtual Branch Branch { get; set; }
        #endregion
    }
}
