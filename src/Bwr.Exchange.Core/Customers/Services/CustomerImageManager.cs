using Abp.Domain.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bwr.Exchange.Customers.Services
{
    public class CustomerImageManager : ICustomerImageManager
    {
        private readonly IRepository<CustomerImage> _customerImageRepository;

        public CustomerImageManager(IRepository<CustomerImage> customerImageRepository)
        {
            _customerImageRepository = customerImageRepository;
        }

        public async Task<CustomerImage> CreateAsync(CustomerImage image)
        {
            return await _customerImageRepository.InsertAsync(image);
        }

        public Task DeleteAsync(int id)
        {
            return _customerImageRepository.DeleteAsync(id);
        }

        public async Task<IList<CustomerImage>> GetAllAsync(int customerId)
        {
            return await _customerImageRepository.GetAllListAsync(x=>x.CustomerId == customerId);
        }
    }
}
