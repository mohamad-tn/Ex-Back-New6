using Abp.Events.Bus;
using Bwr.Exchange.CashFlows.Shared;

namespace Bwr.Exchange.CashFlows.CompanyCashFlows.Events
{
    public class CompanyCashFlowDeletedMultiEventData : DeletedCashFlowEventData
    {
        public CompanyCashFlowDeletedMultiEventData(
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
