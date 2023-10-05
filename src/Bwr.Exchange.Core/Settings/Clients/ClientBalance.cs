using Abp.Domain.Entities.Auditing;
using Bwr.Exchange.Settings.Currencies;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bwr.Exchange.Settings.Clients
{
    public class ClientBalance : FullAuditedEntity
    {
        public double Balance { get; set; }

        #region Currency
        public int CurrencyId { get; set; }
        [ForeignKey("CurrencyId")]
        public virtual Currency Currency { get; set; }
        #endregion

        #region Client
        public int ClientId { get; set; }
        [ForeignKey("ClientId")]
        public virtual Client Client { get; set; }
        #endregion 
    }
}
