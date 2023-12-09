using AutoMapper;
using Bwr.Exchange.Customers.Dto;
using Bwr.Exchange.Shared.Dto;

namespace Bwr.Exchange.Customers.Map
{
    public class CustomerMapProfile : Profile
    {
        public CustomerMapProfile()
        {
            CreateMap<Customer, CustomerDto>();
            CreateMap<CustomerDto, Customer>();
            CreateMap<Customer, ReadCustomerDto>();
            CreateMap<Customer, CustomerWithImagesDto>();
            CreateMap<CustomerImage, FileUploadDto>();
        }
    }
}
