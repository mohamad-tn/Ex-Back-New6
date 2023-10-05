namespace Bwr.Exchange.Shared.DataManagerRequests
{
    public class ExchangeCurrencyDataManagerRequest : BWireDataManagerRequest
    {
        public string fromDate { get; set; }
        public string toDate { get; set; }
        public int? paymentType { get; set; }
        public int? actionType { get; set; }
        public int? companyId { get; set; }
        public int? clientId { get; set; }
        public int? currencyId { get; set; }
    }
}
