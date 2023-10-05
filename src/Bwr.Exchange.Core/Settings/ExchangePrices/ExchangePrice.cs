using Abp.Domain.Entities.Auditing;
using Bwr.Exchange.Settings.Branches;
using Bwr.Exchange.Settings.Currencies;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bwr.Exchange.Settings.ExchangePrices
{
    public class ExchangePrice : FullAuditedEntity
    {
        public decimal? MainPrice { get; set; }
        public decimal? PurchasingPrice { get; set; }
        public decimal? SellingPrice { get; set; }

        #region Currency
        public int? CurrencyId { get; set; }
        [ForeignKey("CurrencyId")]
        public virtual Currency MainCurrency { get; set; }
        #endregion
    }
}
