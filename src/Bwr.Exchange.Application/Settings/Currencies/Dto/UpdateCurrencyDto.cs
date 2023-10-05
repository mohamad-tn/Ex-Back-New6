using Abp.Application.Services.Dto;

namespace Bwr.Exchange.Settings.Currencies.Dto
{
    public class UpdateCurrencyDto : EntityDto
    {
        public string Name { get; set; }
        public bool IsMainCurrency { get; set; }
    }
}
