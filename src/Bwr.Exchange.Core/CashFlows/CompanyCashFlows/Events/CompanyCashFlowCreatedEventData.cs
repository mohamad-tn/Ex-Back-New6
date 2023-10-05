using Bwr.Exchange.CashFlows.Shared;
using System;

namespace Bwr.Exchange.CashFlows.CompanyCashFlows.Events
{
    public class CompanyCashFlowCreatedEventData : CashFlowEventData
    {
        public CompanyCashFlowCreatedEventData() { }
        public CompanyCashFlowCreatedEventData(
            DateTime date,
            int? currencyId,
            int? companyId,
            int transactionId,
            TransactionType transactionType,
            double amount,
            string type,
            double commission = 0.0,
            string instrumentNo = "",
            double companyCommission = 0.0,
            string note = ""
            )
        {
            CurrencyId = currencyId;
            CompanyId = companyId;
            Date = date;
            TransactionId = transactionId;
            TransactionType = transactionType;
            Type = type;
            Amount = amount;
            Commission = commission;
            CompanyCommission = companyCommission;
            InstrumentNo = instrumentNo;
            Note = note;
        }

        public int? CompanyId { get; set; }
        public double CompanyCommission { get; set; }
        
    }
}
