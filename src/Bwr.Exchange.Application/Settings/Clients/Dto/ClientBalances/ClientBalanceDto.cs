using Abp.Application.Services.Dto;

namespace Bwr.Exchange.Settings.Clients.Dto.ClientBalances
{
    public class ClientBalanceDto : EntityDto
    {
        public double Balance { get; set; }
        public int CurrencyId { get; set; }
        public int ClientId { get; set; }
        
    }
}
