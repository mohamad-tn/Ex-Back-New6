namespace Bwr.Exchange.Shared.DataManagerRequests
{
    public class TreasuryCashFlowDataManagerRequest : BWireDataManagerRequest
    {
        public int currencyId { get; set; }
        public string fromDate { get; set; }
        public string toDate { get; set; }
    }
}
