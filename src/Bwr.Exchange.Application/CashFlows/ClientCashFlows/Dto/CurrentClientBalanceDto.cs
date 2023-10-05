using Bwr.Exchange.Settings.Clients.Dto;
using Bwr.Exchange.Settings.Currencies.Dto;

namespace Bwr.Exchange.CashFlows.ClientCashFlows.Dto
{
    public class CurrentClientBalanceDto
    {
        public CurrentClientBalanceDto(ClientDto client, CurrencyDto currency, double balance)
        {
            Client = client;
            Currency = currency;
            Balance = balance;
        }

        public ClientDto Client { get; set; }
        public CurrencyDto Currency { get; set; }

        public double Balance { get; set; }
    }
}
