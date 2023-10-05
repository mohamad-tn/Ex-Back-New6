using System.Collections;

namespace Bwr.Exchange.Shared.Dto
{
    public class ReadGrudDto
    {
        public int count { get; set; }
        public IEnumerable result { get; set; }
        public IEnumerable groupDs { get; set; }
    }
}
