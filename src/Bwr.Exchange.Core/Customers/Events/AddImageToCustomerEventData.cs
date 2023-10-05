using Abp.Events.Bus;
using Bwr.Exchange.Customers;
using System.Collections.Generic;

namespace Bwr.Exchange.Transfers.Customers.Events
{
    public class AddImageToCustomerEventData: EventData
    {
        public AddImageToCustomerEventData(
            List<CustomerImage> images)
        {
            if(images != null)
            {
                Images = images;
            }
            else
            {
                Images = new List<CustomerImage>();
            }
        }

        public IList<CustomerImage> Images { get; set; }
    }
}
