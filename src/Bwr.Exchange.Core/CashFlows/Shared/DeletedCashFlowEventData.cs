using Abp.Events.Bus;

namespace Bwr.Exchange.CashFlows.Shared
{
    public class DeletedCashFlowEventData : EventData
    {
        public int TransactionId { get; set; }
        public TransactionType TransactionType { get; set; }
    }
}
