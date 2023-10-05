namespace Bwr.Exchange.Transfers.IncomeTransfers.Dto
{
    public class IncomeTransferGetForEditInputDto
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public int? CompanyId { get; set; }
        public int? number { get; set; }
    }
}
