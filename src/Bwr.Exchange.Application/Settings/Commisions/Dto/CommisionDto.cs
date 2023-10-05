using Abp.Application.Services.Dto;
using Bwr.Exchange.Settings.Currencies.Dto;

namespace Bwr.Exchange.Settings.Commisions.Dto
{
    public class CommisionDto : EntityDto
    {
        public double From { get; set; }
        public double To { get; set; }
        public double Value { get; set; }
        public double Percentage { get; set; }
        public int CurrencyId { get; set; }
        public CurrencyDto Currency { get; set; }

    }
}
