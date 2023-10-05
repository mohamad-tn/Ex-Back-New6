using System;

namespace Bwr.Exchange.Transfers.OutgoingTransfers.Dto
{
    public class SearchOutgoingTransferInputDto
    {
        public int Number { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string PaymentType { get; set; }
        public string CountryId { get; set; }
        public string ClientId { get; set; }
        public string CompanyId { get; set; }
        public string Beneficiary { get; set; }
        public string BeneficiaryAddress { get; set; }
        public string Sender { get; set; }

    }
}
