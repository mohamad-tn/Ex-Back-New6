using Abp.Domain.Entities.Auditing;
using Bwr.Exchange.Settings.Branches;
using Bwr.Exchange.Settings.Clients;
using Bwr.Exchange.Settings.Companies;
using Bwr.Exchange.Settings.Currencies;
using Bwr.Exchange.Transfers;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bwr.Exchange.ExchangeCurrencies
{
    public class ExchangeCurrency : FullAuditedEntity
    {
        public int Number { get; set; }
        public double AmountOfFirstCurrency { get; set; }
        public double AmoutOfSecondCurrency { get; set; }
        public double PaidAmountOfFirstCurrency { get; set; }
        public double ReceivedAmountOfFirstCurrency { get; set; }
        public double PaidAmountOfSecondCurrency { get; set; }
        public double ReceivedAmountOfSecondCurrency { get; set; }
        public double Commission { get; set; }
        public double ExchangePrice { get; set; }
        public PaymentType PaymentType { get; set; }
        public ActionType ActionType { get; set; }
        public DateTime Date { get; set; }
        public string Note { get; set; }

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

        #region Main Currency
        public int? MainCurrencyId { get; set; }
        [ForeignKey("MainCurrencyId")]
        public virtual Currency MainCurrency { get; set; }
        #endregion

        #region Client
        public int? ClientId { get; set; }
        [ForeignKey("ClientId")]
        public virtual Client Client { get; set; }
        #endregion

        #region Company
        public int? CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; }
        #endregion

        #region Branch
        public int BranchId { get; set; }

        [ForeignKey("BranchId")]
        public virtual Branch Branch { get; set; }
        #endregion


        //History
        public decimal? FirstMainPrice { get; set; }
        public decimal? FirstPurchasingPrice { get; set; }
        public decimal? FirstSellingPrice { get; set; }
        public bool FirstIsMainCurrency { get; set; }

        public decimal? SecondMainPrice { get; set; }
        public decimal? SecondPurchasingPrice { get; set; }
        public decimal? SecondSellingPrice { get; set; }
        public bool SecondIsMainCurrency { get; set; }
    }

}

