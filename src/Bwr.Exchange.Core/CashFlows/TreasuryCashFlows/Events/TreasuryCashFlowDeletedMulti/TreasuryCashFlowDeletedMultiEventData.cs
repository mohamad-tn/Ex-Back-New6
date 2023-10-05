using Bwr.Exchange.CashFlows.Shared;

namespace Bwr.Exchange.CashFlows.TreasuryCashFlows.Events.TreasuryCashFlowDeletedMulti
{
    public class TreasuryCashFlowDeletedMultiEventData : DeletedCashFlowEventData
    {
        public TreasuryCashFlowDeletedMultiEventData(
            int transactionId,
            TransactionType transactionType,
            int? currencyId,
            string type = null)
        {
            TransactionId = transactionId;
            TransactionType = transactionType;
            CurrencyId = currencyId;
            Type = type;
        }

        public int? CurrencyId { get; set; }
        public string Type { get; set; }
    }
}
