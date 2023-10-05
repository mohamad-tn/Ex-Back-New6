using Bwr.Exchange.Shared;

namespace Bwr.Exchange.Customers
{
    public class CustomerImage : FileUploadBase
    {
        public int? CustomerId { get; set; }
        public virtual Customer Customer { get; set; }
    }
}
