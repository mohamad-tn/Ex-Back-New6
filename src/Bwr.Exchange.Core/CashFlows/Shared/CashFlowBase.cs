using Abp.Domain.Entities.Auditing;
using Bwr.Exchange.Authorization.Users;
using Bwr.Exchange.CashFlows;
using Bwr.Exchange.CashFlows.CashFlowMatchings;
using Bwr.Exchange.Settings.Currencies;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bwr.Exchange.Shared
{
    public class CashFlowBase : AuditedEntity
    {
        public DateTime Date { get; set; }
        public double Amount { get; set; }
        public double CurrentBalance { get; set; }
        public double PreviousBalance { get; set; }
        public int TransactionId { get; set; }
        public TransactionType TransactionType { get; set; }
        public bool Matched { get; set; }
        public bool? Shaded { get; set; }
        public string Type { get; set; }
        public string Note { get; set; }
        public string InstrumentNo { get; set; }
        public double Commission { get; set; }
        public string Destination { get; set; }
        public string Beneficiary { get; set; }
        public string Sender { get; set; }

        #region Currency 
        public int CurrencyId { get; set; }
        [ForeignKey("CurrencyId")]
        public virtual Currency Currency { get; set; }
        #endregion

        #region Matching
        public int? CashFlowMatchingId { get; set; }
        [ForeignKey("CashFlowMatchingId")]
        public virtual CashFlowMatching CashFlowMatching { get; set; }
        #endregion
    }
}
