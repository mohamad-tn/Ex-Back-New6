using Abp.Dependency;
using Abp.Events.Bus.Handlers;
using Bwr.Exchange.Customers.Services;
using System.Threading.Tasks;

namespace Bwr.Exchange.Transfers.Customers.Events
{
    public class AddImageToCustomerEventHandler: IAsyncEventHandler<AddImageToCustomerEventData>, ITransientDependency
    {
        private readonly ICustomerImageManager _customerImageManager;

        public AddImageToCustomerEventHandler(ICustomerImageManager customerImageManager)
        {
            _customerImageManager = customerImageManager;
        }

        public async Task HandleEventAsync(AddImageToCustomerEventData eventData)
        {
            foreach (var image in eventData.Images)
            {
                await _customerImageManager.CreateAsync(image);
            }
        }
    }
}
