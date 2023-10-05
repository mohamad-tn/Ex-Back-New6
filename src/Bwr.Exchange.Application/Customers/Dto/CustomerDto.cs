using Abp.Application.Services.Dto;

namespace Bwr.Exchange.Customers.Dto
{
    public class CustomerDto : EntityDto
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string IdentificationNumber { get; set; }
    }
}
