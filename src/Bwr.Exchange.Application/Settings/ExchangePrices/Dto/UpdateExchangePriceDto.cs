using Abp.Application.Services.Dto;

namespace Bwr.Exchange.Settings.ExchangePrices.Dto
{
    public class UpdateExchangePriceDto : EntityDto
    {
        public decimal? MainPrice { get; set; }
        public decimal? PurchasingPrice { get; set; }
        public decimal? SellingPrice { get; set; }
        public int CurrencyId { get; set; }
    }
}
