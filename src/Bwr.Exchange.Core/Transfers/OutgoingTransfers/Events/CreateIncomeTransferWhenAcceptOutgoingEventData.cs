using Abp.Events.Bus;
using Bwr.Exchange.Customers;

namespace Bwr.Exchange.Transfers.OutgoingTransfers.Events
{
    public class CreateIncomeTransferWhenAcceptOutgoingEventData : EventData
    {

        public CreateIncomeTransferWhenAcceptOutgoingEventData(int? companyId, string date, string note,
            int? toCompanyId, int? toClientId, int incomeTransferId,
            int status, int paymentType, double amount, double commission,
            double companyCommission, int currencyId, int? beneficiaryId, int? senderId, double clientCommission, int? branchId, Customer sender, Customer beneficiary)
        {
            CompanyId = companyId;
            Date = date;
            Note = note;
            ToCompanyId = toCompanyId;
            ToClientId = toClientId;
            IncomeTransferId = incomeTransferId;
            Status = status;
            PaymentType = paymentType;
            Amount = amount;
            Commission = commission;
            CompanyCommission = companyCommission;
            CurrencyId = currencyId;
            BeneficiaryId = beneficiaryId;
            SenderId = senderId;
            ClientCommission = clientCommission;
            this.branchId = branchId;
            Sender = sender;
            Beneficiary = beneficiary;
        }

        //public int Number { get; set; }
        //public int Index { get; set; }
        public int? branchId { get; set; }
        public int? CompanyId { get; set; }
        public string Date { get; set; }
        public string Note { get; set; }
        public int? ToCompanyId { get; set; }
        public int? ToClientId { get; set; }
        public int IncomeTransferId { get; set; }
        public int Status { get; set; }
        public int PaymentType { get; set; }
        public double Amount { get; set; }
        public double Commission { get; set; }
        public double CompanyCommission { get; set; }
        public int CurrencyId { get; set; }
        public int? BeneficiaryId { get; set; }
        public Customer Beneficiary { get; set; }

        public int? SenderId { get; set; }
        public Customer Sender { get; set; }

        public double ClientCommission { get; set; }
    }
}
