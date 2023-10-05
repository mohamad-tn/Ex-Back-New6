using Abp.Domain.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bwr.Exchange.Customers.Services
{
    public interface ICustomerManager: IDomainService
    {
        Task<Customer> GetByIdAsync(int id);
        Task<Customer> CreateAsync(Customer customer);
        Task<Customer> UpdateAsync(Customer customer);
        Task<Customer> CreateOrUpdateAsync(Customer customer);
        Task<IList<Customer>> GetAllAsync();
        Task<Customer> GetByNameAsync(string name);
        Task<Customer> AddIdentityNumber(string identificationNumber, int id);
    }
}
