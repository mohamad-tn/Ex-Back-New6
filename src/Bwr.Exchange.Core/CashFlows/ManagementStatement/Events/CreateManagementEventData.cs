using Abp.Events.Bus;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bwr.Exchange.CashFlows.ManagementStatement.Events
{
    public class CreateManagementEventData : EventData
    {
        public CreateManagementEventData(
            int type, double? amount, DateTime date, int? paymentType, DateTime changeDate,
            int changeType, double? number, int? treasuryActionType, string mainAccount,
            string beforChange, string afterChange, double? amountOfFirstCurrency,
            double? amoutOfSecondCurrency, double? paidAmountOfFirstCurrency,
            double? receivedAmountOfFirstCurrency, double? paidAmountOfSecondCurrency,
            double? receivedAmountOfSecondCurrency, double? commission, int? firstCurrencyId,
            int? secondCurrencyId, int? currencyId, int? clientId, long userId, int? companyId,
            int? senderId, int? beneficiaryId, int? toCompanyId, int? actionType)
        {
            Type = type;
            Amount = amount;
            Date = date;
            PaymentType = paymentType;
            ChangeDate = changeDate;
            ChangeType = changeType;
            Number = number;
            TreasuryActionType = treasuryActionType;
            MainAccount = mainAccount;
            BeforChange = beforChange;
            AfterChange = afterChange;
            AmountOfFirstCurrency = amountOfFirstCurrency;
            AmoutOfSecondCurrency = amoutOfSecondCurrency;
            PaidAmountOfFirstCurrency = paidAmountOfFirstCurrency;
            ReceivedAmountOfFirstCurrency = receivedAmountOfFirstCurrency;
            PaidAmountOfSecondCurrency = paidAmountOfSecondCurrency;
            ReceivedAmountOfSecondCurrency = receivedAmountOfSecondCurrency;
            Commission = commission;
            FirstCurrencyId = firstCurrencyId;
            SecondCurrencyId = secondCurrencyId;
            CurrencyId = currencyId;
            ClientId = clientId;
            UserId = userId;
            CompanyId = companyId;
            SenderId = senderId;
            BeneficiaryId = beneficiaryId;
            ToCompanyId = toCompanyId;
            ActionType = actionType;
        }

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
