using Abp.Domain.Entities.Auditing;
using Bwr.Exchange.Customers;
using Bwr.Exchange.Settings.Currencies;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bwr.Exchange.Transfers
{
    public class Transfer : AuditedEntity
    {
        #region Currency
        public int CurrencyId { get; set; }
        [ForeignKey("CurrencyId")]
        public virtual Currency Currency { get; set; }
        #endregion

        #region Beneficiary
        public int? BeneficiaryId { get; set; }
        [ForeignKey("BeneficiaryId")]
        public virtual Customer Beneficiary { get; set; }
        #endregion

        #region Sender Name
        public int? SenderId { get; set; }
        [ForeignKey("SenderId")]
        public virtual Customer Sender { get; set; }
        #endregion

        public PaymentType PaymentType { get; set; }
        public double Amount { get; set; }
        public double Commission { get; set; }
        public double CompanyCommission { get; set; }
        public double ClientCommission { get; set; }
        public int Number { get; set; }
    }
}
