namespace Bwr.Exchange.Settings.Clients.Dto.ClientBalances
{
    public class ReadClientBalanceDto
    {
        public int id { get; set; }
        public double balance { get; set; }
        public int currencyId { get; set; }
        public int clientId { get; set; }
    }
}
