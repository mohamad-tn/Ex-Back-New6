using Abp.Application.Services.Dto;
using System;

namespace Bwr.Exchange.CashFlows.ManagementStatement.Dto
{
    public class CreateManagementDto : EntityDto
    {
        public int Type { get; set; }
        public double? Amount { get; set; }
        public DateTime Date { get; set; }
        public int? PaymentType { get; set; }
        public DateTime ChangeDate { get; set; }
        public int ChangeType { get; set; }
        public double? Number { get; set; }
        public int? TreasuryActionType { get; set; }
        public int? ActionType { get; set; }
        public string? MainAccount { get; set; }
        public string BeforChange { get; set; }
        public string AfterChange { get; set; }
        public double? AmountOfFirstCurrency { get; set; }
        public double? AmoutOfSecondCurrency { get; set; }
        public double? PaidAmountOfFirstCurrency { get; set; }
        public double? ReceivedAmountOfFirstCurrency { get; set; }
        public double? PaidAmountOfSecondCurrency { get; set; }
        public double? ReceivedAmountOfSecondCurrency { get; set; }
        public double? Commission { get; set; }
        public int? FirstCurrencyId { get; set; }
        public int? SecondCurrencyId { get; set; }
        public int? CurrencyId { get; set; }
        public int? ClientId { get; set; }
        public long UserId { get; set; }
        public int? CompanyId { get; set; }
        public int? ToCompanyId { get; set; }
        public int? SenderId { get; set; }
        public int? BeneficiaryId { get; set; }

    }
}
