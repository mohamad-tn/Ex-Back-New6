using Abp.Domain.Entities.Auditing;
using Bwr.Exchange.Settings.Branches;
using Bwr.Exchange.Settings.Countries;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bwr.Exchange.Settings.Clients
{
    public class Client : FullAuditedEntity
    {
        public Client()
        {
            ClientBalances = new List<ClientBalance>();
            ClientPhones = new List<ClientPhone>();
        }
        public string Name { get; set; }
        public string Address { get; set; }
        public bool Activated { get; set; }

        #region Branch
        public int BranchId { get; set; }

        [ForeignKey("BranchId")]
        public virtual Branch Branch { get; set; }
        #endregion

        #region Province
        public int ProvinceId { get; set; }
        [ForeignKey("ProvinceId")]
        public virtual Province Province { get; set; }
        #endregion

        public virtual IList<ClientBalance> ClientBalances { get; set; }
        public virtual IList<ClientPhone> ClientPhones { get; set; }
    }
}
