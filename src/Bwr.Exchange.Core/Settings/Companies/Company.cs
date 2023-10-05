using Abp.Domain.Entities.Auditing;
using Bwr.Exchange.Settings.Branches;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bwr.Exchange.Settings.Companies
{
    public class Company : FullAuditedEntity
    {
        public Company()
        {
            CompanyBalances = new List<CompanyBalance>();
        }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }

        public virtual IList<CompanyBalance> CompanyBalances { get; set; }

        #region Branch
        public int BranchId { get; set; }

        [ForeignKey("BranchId")]
        public virtual Branch Branch { get; set; }
        #endregion
    }
}
