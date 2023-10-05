using Abp.Application.Services.Dto;
using Bwr.Exchange.Customers.Dto;
using Bwr.Exchange.Shared.Dto;
using System.Collections.Generic;

namespace Bwr.Exchange.Transfers.OutgoingTransfers.Dto
{
    public class OutgoingTransferDto : EntityDto
    {
        public OutgoingTransferDto()
        {
            Images = new List<FileUploadDto>();
        }
        public int Number { get; set; }
        public int CurrencyId { get; set; }
        public int? BeneficiaryId { get; set; }
        public int? SenderId { get; set; }
        public int PaymentType { get; set; }
        public double Amount { get; set; }
        public double Commission { get; set; }
        public double CompanyCommission { get; set; }
        public double ClientCommission { get; set; }
        public string Date { get; set; }
        public int ToCompanyId { get; set; }
        public int CountryId { get; set; }
        public int? FromCompanyId { get; set; }
        public int? FromClientId { get; set; }
        public int? ToBranchId { get; set; }
        public int? FromBranchId { get; set; }
        public int? TreasuryId { get; set; }
        public double ReceivedAmount { get; set; }
        public string InstrumentNo { get; set; } //رقم الصك
        public string Reason { get; set; }
        public string Note { get; set; }
        public int Status { get; set; }

        public CustomerDto Beneficiary { get; set; }
        public CustomerDto Sender { get; set; }
        public IList<FileUploadDto> Images { get; set; }
    }
}
