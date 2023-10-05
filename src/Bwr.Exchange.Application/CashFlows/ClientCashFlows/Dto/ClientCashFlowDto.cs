using Bwr.Exchange.CashFlows.Shared.Dto;
using Bwr.Exchange.Settings.Clients.Dto;

namespace Bwr.Exchange.CashFlows.ClientCashFlows.Dto
{
    public class ClientCashFlowDto : CashFlowDto
    {

        #region Client 
        public int ClientId { get; set; }
        public ClientDto Client { get; set; }
        #endregion

        public double Commission { get; set; }
        public double ClientCommission { get; set; }
        public double Balance { get; set; }
    }
}
