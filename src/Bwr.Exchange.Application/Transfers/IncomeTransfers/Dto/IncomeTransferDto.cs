using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bwr.Exchange.Transfers.IncomeTransfers.Dto
{
    public class IncomeTransferDto:EntityDto
    {
        public IncomeTransferDto()
        {
            IncomeTransferDetails = new List<IncomeTransferDetailDto>();
        }
        public int Number { get; set; }
        public string Date { get; set; }
        public string Note { get; set; }
        public int? CompanyId { get; set; }

        public virtual IList<IncomeTransferDetailDto> IncomeTransferDetails { get; set; }
    }
}
