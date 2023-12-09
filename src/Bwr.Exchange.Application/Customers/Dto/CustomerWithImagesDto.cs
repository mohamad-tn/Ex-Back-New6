using Abp.Application.Services.Dto;
using Bwr.Exchange.Shared.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bwr.Exchange.Customers.Dto
{
    public class CustomerWithImagesDto : EntityDto
    {
        public CustomerWithImagesDto()
        {
            Images = new List<FileUploadDto>();
        }

        public string Name { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string IdentificationNumber { get; set; }

        public IList<FileUploadDto> Images { get; set; }
    }
}
