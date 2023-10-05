using Abp.Domain.Entities.Auditing;
using Bwr.Exchange.Settings.Commisions;
using Bwr.Exchange.Settings.Currencies;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Bwr.Exchange.Settings
{
    public class Commision : FullAuditedEntity
    {
        public double From { get; set; }
        public double To { get; set; }
        public double Value { get; set; }
        public double Percentage { get; set; }

        #region Currency
        public int CurrencyId { get; private set; }
        [ForeignKey("CurrencyId")]
        public virtual Currency Currency { get; private set; }
        #endregion
    }
}
