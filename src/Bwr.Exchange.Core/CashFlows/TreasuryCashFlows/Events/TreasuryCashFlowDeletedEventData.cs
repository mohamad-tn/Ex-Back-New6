using Bwr.Exchange.CashFlows.Shared;

namespace Bwr.Exchange.CashFlows.TreasuryCashFlows.Events
{
    public class TreasuryCashFlowDeletedEventData : DeletedCashFlowEventData
    {
        public TreasuryCashFlowDeletedEventData(int transactionId, TransactionType transactionType)
        {
            TransactionId = transactionId;
            TransactionType = transactionType;
        }
    }
}
