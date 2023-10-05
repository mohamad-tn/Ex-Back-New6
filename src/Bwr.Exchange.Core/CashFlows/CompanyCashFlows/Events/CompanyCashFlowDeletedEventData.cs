using Bwr.Exchange.CashFlows.Shared;

namespace Bwr.Exchange.CashFlows.CompanyCashFlows.Events
{
    public class CompanyCashFlowDeletedEventData : DeletedCashFlowEventData
    {
        public CompanyCashFlowDeletedEventData(
            int transactionId, 
            TransactionType transactionType, 
            string type = null)
        {
            TransactionId = transactionId;
            TransactionType = transactionType;
            Type = type;
        }

        public string Type { get; set; }
    }
}
