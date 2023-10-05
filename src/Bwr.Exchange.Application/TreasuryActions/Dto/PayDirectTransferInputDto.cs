using Bwr.Exchange.Shared.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bwr.Exchange.TreasuryActions.Dto
{
    public class PayDirectTransferInputDto
    {
        public PayDirectTransferInputDto()
        {
            Images = new List<FileUploadDto>();
        }
        public TreasuryActionDto TreasuryAction { get; set; }
        public IList<FileUploadDto> Images { get; set; }
        public string PhoneNumber { get; set; }
    }
}
