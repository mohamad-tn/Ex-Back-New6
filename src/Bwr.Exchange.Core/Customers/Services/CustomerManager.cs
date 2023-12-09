using Abp.Domain.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bwr.Exchange.Customers.Services
{
    public class CustomerManager : ICustomerManager
    {
        private readonly IRepository<Customer> _customerRepository;

        public CustomerManager(IRepository<Customer> customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<Customer> CreateAsync(Customer customer)
        {
            var customerId = await _customerRepository.InsertAndGetIdAsync(customer);
            return await GetByIdAsync(customerId);
        }

        public async Task<IList<Customer>> GetAllAsync()
        {
            return await _customerRepository.GetAllListAsync();
        }

        public async Task<Customer> GetByIdAsync(int id)
        {
            return await _customerRepository.FirstOrDefaultAsync(id);
        }

        public async Task<Customer> GetByNameAsync(string symbol)
        {
            return await _customerRepository.FirstOrDefaultAsync(x => x.Name.StartsWith(symbol));
        }

        public async Task<Customer> UpdateAsync(Customer customer)
        {
            return await _customerRepository.UpdateAsync(customer);
        }

        public async Task<Customer> CreateOrUpdateAsync(Customer input)
        {
            var customer = await _customerRepository
                .FirstOrDefaultAsync(x => x.Name.ToLower() == input.Name.ToLower());
            if(customer != null)
            {
                if (!string.IsNullOrEmpty(input.PhoneNumber))
                {
                    customer.PhoneNumber = input.PhoneNumber;
                    await _customerRepository.UpdateAsync(customer);
                }

                return customer;
            }
            else
            {
                var newCustomer = new Customer();
                newCustomer.Name = input.Name;
                newCustomer.PhoneNumber = input.PhoneNumber;
                newCustomer.Address = input.Address;
                newCustomer.IdentificationNumber = input.IdentificationNumber;

                var id = await _customerRepository.InsertAndGetIdAsync(newCustomer);
                return await _customerRepository.GetAsync(id);
            }
        }

        public async Task<Customer> AddIdentityNumber(string identificationNumber, int id)
        {
            var customer = await _customerRepository.GetAsync(id);
            customer.IdentificationNumber = identificationNumber;

            await _customerRepository.UpdateAsync(customer);

            return await _customerRepository.GetAsync(id);
        }

        public string GetCustomerNameById(int id)
        {
            var customer = _customerRepository.Get(id);
            return customer.Name;
        }

        public async Task<Customer> GetCustomerWithImages(int id)
        {
            var customer = await _customerRepository.GetAsync(id);

            if (customer != null)
            {
                await _customerRepository.EnsureCollectionLoadedAsync(customer, x => x.Images);
            }

            return customer;


        }
    }
}
