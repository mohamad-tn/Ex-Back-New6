using Bwr.Exchange.Settings.Companies.Dto;

namespace Bwr.Exchange.Transfers.IncomeTransfers.Dto
{
    public class ReadIncomeTransferDto
    {
        public int id { get; set; }
        public string date { get; set; }
        public int number { get; set; }
        public string note { get; set; }

        #region Company
        public int? companyId { get; set; }
        public ReadCompanyDto company { get; set; }
        #endregion
    }
}
