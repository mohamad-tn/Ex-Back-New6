using Abp.Application.Services.Dto;

namespace Bwr.Exchange.ExchangeCurrencies.Dto
{
    public class ExchangeCurrencyHistoryDto : EntityDto
    {
        public decimal? FirstMainPrice { get; set; }
        public decimal? FirstPurchasingPrice { get; set; }
        public decimal? FirstSellingPrice { get; set; }

        public decimal? SecondMainPrice { get; set; }
        public decimal? SecondPurchasingPrice { get; set; }
        public decimal? SecondSellingPrice { get; set; }

        public int? ExchangeCurrencyId { get; set; }
    }
}
