using Abp.Application.Services.Dto;
using Bwr.Exchange.Settings.Branches.Dto;
using Bwr.Exchange.Settings.Clients.Dto;
using Bwr.Exchange.Settings.Companies.Dto;
using Bwr.Exchange.Settings.Currencies.Dto;
using System;

namespace Bwr.Exchange.ExchangeCurrencies.Dto
{
    public class ExchangeCurrencyDto : EntityDto
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
        public int PaymentType { get; set; }
        public int ActionType { get; set; }
        public DateTime? Date { get; set; }
        public string Note { get; set; }

        #region First Currency
        public int? FirstCurrencyId { get; set; }
        public CurrencyDto FirstCurrency { get; set; }
        #endregion

        #region Second Currency
        public int? SecondCurrencyId { get; set; }
        public CurrencyDto SecondCurrency { get; set; }
        #endregion

        #region Main Currency
        public int? MainCurrencyId { get; set; }
        public CurrencyDto MainCurrency { get; set; }
        #endregion

        #region Client
        public int? ClientId { get; set; }
        public ClientDto Client { get; set; }
        #endregion

        #region Company
        public int? CompanyId { get; set; }
        public CompanyDto Company { get; set; }
        #endregion

        #region Branch
        public int BranchId { get; set; }
        public BranchDto Branch { get; set; }
        #endregion


        //History
        public double? FirstMainPrice { get; set; }
        public double? FirstPurchasingPrice { get; set; }
        public double? FirstSellingPrice { get; set; }
        public bool FirstIsMainCurrency { get; set; }

        public double? SecondMainPrice { get; set; }
        public double? SecondPurchasingPrice { get; set; }
        public double? SecondSellingPrice { get; set; }
        public bool SecondIsMainCurrency { get; set; }
    }
}
