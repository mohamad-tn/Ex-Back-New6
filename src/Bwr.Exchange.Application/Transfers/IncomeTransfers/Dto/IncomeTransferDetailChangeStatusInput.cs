using Abp.Application.Services.Dto;

namespace Bwr.Exchange.Transfers.IncomeTransfers.Dto
{
    public class IncomeTransferDetailChangeStatusInput : EntityDto
    {
        public int Status { get; set; }
    }
}
