using Bwr.Exchange.CashFlows.Shared;
using System;

namespace Bwr.Exchange.CashFlows.TreasuryCashFlows.Events
{
    public class TreasuryCashFlowCreatedEventData : CashFlowEventData
    {
        public TreasuryCashFlowCreatedEventData() { }
        public TreasuryCashFlowCreatedEventData(
            DateTime date, 
            int? currencyId, 
            int? treasuryId, 
            int transactionId, 
            TransactionType transactionType, 
            double amount, 
            string type, 
            string name,
            string note = ""
            )
        {
            Date = date;
            CurrencyId = currencyId;
            TreasuryId = treasuryId;
            TransactionId = transactionId;
            TransactionType = transactionType;
            Amount = amount;
            Type = type;
            Name = name;
            Note = note;
        }

        public int? TreasuryId { get; set; }
        public string Name { get; set; }
        
    }
}
