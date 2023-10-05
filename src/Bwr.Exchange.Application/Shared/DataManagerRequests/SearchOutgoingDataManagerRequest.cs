namespace Bwr.Exchange.Shared.DataManagerRequests
{
    public class SearchOutgoingDataManagerRequest : BWireDataManagerRequest
    {
        public int number { get; set; }
        public string fromDate { get; set; }
        public string toDate { get; set; }
        public int? paymentType { get; set; }
        public int? clientId { get; set; }
        public int? companyId { get; set; }
        public int? countryId { get; set; }
        public string beneficiary { get; set; }
        public string beneficiaryAddress { get; set; }
        public string sender { get; set; }
    }
}
