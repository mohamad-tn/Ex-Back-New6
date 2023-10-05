using Abp.Domain.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bwr.Exchange.Customers.Services
{
    public interface ICustomerImageManager: IDomainService
    {
        Task<IList<CustomerImage>> GetAllAsync(int customerId);
        Task<CustomerImage> CreateAsync(CustomerImage image);
        Task DeleteAsync(int id);
    }
}
