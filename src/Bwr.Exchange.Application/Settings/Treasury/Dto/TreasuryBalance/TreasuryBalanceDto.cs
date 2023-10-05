using Abp.Application.Services.Dto;
using Bwr.Exchange.Settings.Currencies.Dto;

namespace Bwr.Exchange.Settings.Treasury.Dto.TreasuryBalance
{
    public class TreasuryBalanceDto : EntityDto
    {
        public double InitilBalance { get; set; }
        public int CurrencyId { get; set; }
        public CurrencyDto Currency { get; set; }
        public TreasuryDto Treasury { get; set; }
        public int TreasuryId { get; set; }
        
    }
}
