using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Bwr.Exchange.ExchangeCurrencies
{
    public class ExchangeCurrencyHistory: FullAuditedEntity
    {
        public int? FirstCurrencyId { get; set; }
        public decimal? FirstMainPrice { get; set; }
        public decimal? FirstPurchasingPrice { get; set; }
        public decimal? FirstSellingPrice { get; set; }
        public bool FirstIsMainCurrency { get; set; }

        public int? SecondCurrencyId { get; set; }
        public decimal? SecondMainPrice { get; set; }
        public decimal? SecondPurchasingPrice { get; set; }
        public decimal? SecondSellingPrice { get; set; }
        public bool SecondIsMainCurrency { get; set; }

        #region ExchangeCurrency
        public int? ExchangeCurrencyId { get; set; }
        [ForeignKey("ExchangeCurrencyId")]
        public virtual ExchangeCurrency MainExchangeCurrency { get; set; }
        #endregion
    }
}
