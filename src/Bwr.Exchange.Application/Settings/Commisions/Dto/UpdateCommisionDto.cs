using Abp.Application.Services.Dto;

namespace Bwr.Exchange.Settings.Commisions.Dto
{
    public class UpdateCommisionDto : EntityDto
    {
        public double From { get; set; }
        public double To { get; set; }
        public double Value { get; set; }
        public double Percentage { get; set; }
        public int CurrencyId { get; set; }
    }
}
