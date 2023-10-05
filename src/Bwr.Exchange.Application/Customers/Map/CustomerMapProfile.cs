using AutoMapper;
using Bwr.Exchange.Customers.Dto;

namespace Bwr.Exchange.Customers.Map
{
    public class CustomerMapProfile : Profile
    {
        public CustomerMapProfile()
        {
            CreateMap<Customer, CustomerDto>();
            CreateMap<CustomerDto, Customer>();

            CreateMap<Customer, ReadCustomerDto>();
        }
    }
}
