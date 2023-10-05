using Abp.Events.Bus;
using Bwr.Exchange.CashFlows.Shared;
using System;

namespace Bwr.Exchange.CashFlows.ClientCashFlows.Events
{
    public class ClientCashFlowCreatedEventData : CashFlowEventData
    {
        public ClientCashFlowCreatedEventData() { }
        public ClientCashFlowCreatedEventData(
            DateTime date,
            int? currencyId,
            int? clientId,
            int transactionId,
            TransactionType transactionType,
            double amount,
            string type,
            double commission = 0.0,
            string instrumentNo = "",
            double clientCommission = 0.0,
            string note = ""
            )
        {
            CurrencyId = currencyId;
            ClientId = clientId;
            Date = date;
            TransactionId = transactionId;
            TransactionType = transactionType;
            Type = type;
            Amount = amount;
            Commission = commission;
            ClientCommission = clientCommission;
            InstrumentNo = instrumentNo;
            Note = note;
        }

        public int? ClientId { get; set; }
        public double ClientCommission { get; set; }
        public double ReceivedAmount { get; set; }
    }
}
