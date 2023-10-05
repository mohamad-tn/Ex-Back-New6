using Abp.Domain.Entities.Auditing;
using Bwr.Exchange.Settings.Branches;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bwr.Exchange.Settings.Treasuries
{
    public class Treasury : FullAuditedEntity
    {
        public Treasury()
        {
            TreasuryBalances = new List<TreasuryBalance>();
        }
        public string Name { get; set; }

        #region Branch
        public int BranchId { get; set; }

        [ForeignKey("BranchId")]
        public virtual Branch Branch { get; set; }
        #endregion

        public IList<TreasuryBalance> TreasuryBalances { get; set; }
    }
}
