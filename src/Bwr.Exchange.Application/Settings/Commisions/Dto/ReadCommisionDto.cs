
using Bwr.Exchange.Settings.Currencies.Dto;

namespace Bwr.Exchange.Settings.Commisions.Dto
{
    public class ReadCommisionDto
    {
        public int id { get; set; }
        public double from { get; set; }
        public double to { get; set; }
        public double value { get; set; }
        public double percentage { get; set; }
        public int currencyId { get; set; }
        public ReadCurrencyDto currency { get; set; }

    }
}
