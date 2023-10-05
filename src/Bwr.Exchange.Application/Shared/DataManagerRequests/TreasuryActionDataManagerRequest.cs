namespace Bwr.Exchange.Shared.DataManagerRequests
{
    public class TreasuryActionDataManagerRequest: BWireDataManagerRequest
    {
        public int? number { get; set; }
        public int? currencyId { get; set; }
        public string fromDate { get; set; }
        public string toDate { get; set; }
        public int? actionType { get; set; }
        public int? mainAccount { get; set; }
        public int? mainAccountCompanyId { get; set; }
        public int? mainAccountClientId { get; set; }
        public int? expenseId { get; set; }
        public int? incomeId { get; set; }
        public int? incomeTransferDetailId { get; set; }
    }
}
