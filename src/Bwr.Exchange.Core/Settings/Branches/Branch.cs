using Abp.Domain.Entities.Auditing;
using Bwr.Exchange.Authorization.Users;
using System.Collections.Generic;

namespace Bwr.Exchange.Settings.Branches
{
    public class Branch: FullAuditedEntity
    {
        public Branch()
        {
            User = new List<User>();
        }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public int CountryId { get; set; }

        #region Users
        public virtual IList<User> User { get; set; }
        #endregion
    }
}
