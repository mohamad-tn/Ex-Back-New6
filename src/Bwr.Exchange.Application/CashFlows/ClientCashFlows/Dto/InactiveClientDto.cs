
using System;

namespace Bwr.Exchange.CashFlows.ClientCashFlows.Dto
{
    public class InactiveClientDto
    {
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public DateTime? LastActionDate { get; set; }
    }
}
