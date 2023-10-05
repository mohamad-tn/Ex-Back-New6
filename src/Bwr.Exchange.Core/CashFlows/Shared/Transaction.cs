using Abp.Domain.Values;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bwr.Exchange.CashFlows
{
    public class Transaction : ValueObject
    {
        public Transaction() { }
        public Transaction(int transactionId, TransactionType transactionType)
        {
            TransactionId = transactionId;
            TransactionType = transactionType;
        }
        public int TransactionId { get; private set; }
        public TransactionType TransactionType { get; private set; }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return TransactionId;
            yield return TransactionType;
        }
    }
}
