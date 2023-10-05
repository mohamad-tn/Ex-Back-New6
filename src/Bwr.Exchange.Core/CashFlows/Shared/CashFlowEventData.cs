using Abp.Events.Bus;
using System;

namespace Bwr.Exchange.CashFlows.Shared
{
    public class CashFlowEventData : EventData
    {
        public int? CurrencyId { get; set; }
        public DateTime Date { get; set; }
        public int TransactionId { get; set; }
        public TransactionType TransactionType { get; set; }
        public string Type { get; set; }
        public string Note { get; set; }
        public double Amount { get; set; }
        public double Commission { get; set; }
        public double? PreviousBalance { get; set; }
        public string InstrumentNo { get; set; }
        public string Destination { get; set; }
        public string Beneficiary { get; set; }
        public string Sender { get; set; }
    }
}
