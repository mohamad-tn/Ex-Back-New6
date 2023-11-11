using Abp.Domain.Entities.Auditing;
using Bwr.Exchange.Authorization.Users;
using Bwr.Exchange.Customers;
using Bwr.Exchange.ExchangeCurrencies;
using Bwr.Exchange.Settings.Clients;
using Bwr.Exchange.Settings.Companies;
using Bwr.Exchange.Settings.Currencies;
using Bwr.Exchange.Transfers;
using Bwr.Exchange.TreasuryActions;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bwr.Exchange.CashFlows.ManagementStatement
{
    public class Management : FullAuditedEntity
    {
        public ManagementItemType Type { get; set; }
        public double? Amount { get; set; }
        public DateTime Date { get; set; } 
        public PaymentType? PaymentType { get; set; }
        public DateTime ChangeDate { get; set; }
        public ChangeType ChangeType { get; set; }
        public double? Number { get; set; }
        public TreasuryActionType? TreasuryActionType { get; set; }
        public ActionType? ActionType { get; set; }
        public string MainAccount { get; set; }
        public string BeforChange { get; set; }
        public string AfterChange { get; set; }

        #region ExChange
        public double? AmountOfFirstCurrency { get; set; }
        public double? AmoutOfSecondCurrency { get; set; }
        public double? PaidAmountOfFirstCurrency { get; set; }
        public double? ReceivedAmountOfFirstCurrency { get; set; }
        public double? PaidAmountOfSecondCurrency { get; set; }
        public double? ReceivedAmountOfSecondCurrency { get; set; }
        public double? Commission { get; set; }

        #region First Currency
        public int? FirstCurrencyId { get; set; }
        [ForeignKey("FirstCurrencyId")]
        public virtual Currency FirstCurrency { get; set; }
        #endregion

        #region Second Currency
        public int? SecondCurrencyId { get; set; }
        [ForeignKey("SecondCurrencyId")]
        public virtual Currency SecondCurrency { get; set; }
        #endregion

        #endregion

        #region Currency
        [ForeignKey("CurrencyId")]
        public virtual Currency Currency { get; set; }
        public int? CurrencyId { get; set; }
        #endregion

        #region Client
        public int? ClientId { get; set; }
        [ForeignKey("ClientId")]
        public virtual Client Client { get; set; }
        #endregion

        #region User
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        public long UserId { get; set; }
        #endregion

        #region Company
        public int? CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; }
        #endregion

        #region ToCompany
        public int? ToCompanyId { get; set; }
        [ForeignKey("ToCompanyId")]
        public virtual Company ToCompany { get; set; }
        #endregion

        #region Sender
        [ForeignKey("SenderId")]
        public virtual Customer Sender { get; set; }
        public int? SenderId { get; set; }
        #endregion

        #region Beneficiary
        [ForeignKey("BeneficiaryId")]
        public virtual Customer Beneficiary { get; set; }
        public int? BeneficiaryId { get; set; }
        #endregion

    }
}
