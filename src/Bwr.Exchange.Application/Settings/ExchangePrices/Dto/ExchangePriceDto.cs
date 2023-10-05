using Abp.Application.Services.Dto;
using Bwr.Exchange.Settings.Currencies.Dto;

namespace Bwr.Exchange.Settings.ExchangePrices.Dto
{
    public class ExchangePriceDto : EntityDto
    {
        public decimal? MainPrice { get; set; }
        public decimal? PurchasingPrice { get; set; }
        public decimal? SellingPrice { get; set; }
        public int? CurrencyId { get; set; }
        public CurrencyDto Currency { get; set; }

    }
}
