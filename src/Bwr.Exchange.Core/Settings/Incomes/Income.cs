using Abp.Domain.Entities.Auditing;
using Bwr.Exchange.Settings.Branches;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bwr.Exchange.Settings.Incomes
{
    public class Income : FullAuditedEntity
    {
        public Income(string name)
        {
            Name = name;
        }
        public string Name { get; private set; }

        #region Branch
        public int? BranchId { get; set; }

        [ForeignKey("BranchId")]
        public virtual Branch Branch { get; set; }
        #endregion
    }
}
