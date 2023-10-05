using Abp.Events.Bus;
using Bwr.Exchange.CashFlows.Shared;

namespace Bwr.Exchange.CashFlows.ClientCashFlows.Events
{
    public class ClientCashFlowDeletedMultiEventData : DeletedCashFlowEventData
    {
        public ClientCashFlowDeletedMultiEventData(
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
