using Abp.Application.Services.Dto;
using Bwr.Exchange.Customers.Dto;
using System.Collections.Generic;

namespace Bwr.Exchange.Transfers.IncomeTransfers.Dto
{
    public class IncomeTransferDetailDto : EntityDto
    {
        public int Index { get; set; }
        public int Status { get; set; }
        public int? ToCompanyId { get; set; }
        public int? ToClientId { get; set; }
        public int CurrencyId { get; set; }

        #region Beneficiary
        public int? BeneficiaryId { get; set; }
        public CustomerDto Beneficiary { get; set; }
        #endregion

        #region Sender Name
        public int? SenderId { get; set; }
        public CustomerDto Sender { get; set; }
        #endregion
        public int PaymentType { get; set; }
        public double Amount { get; set; }
        public double Commission { get; set; }
        public double CompanyCommission { get; set; }
        public double ClientCommission { get; set; }
    }
}
