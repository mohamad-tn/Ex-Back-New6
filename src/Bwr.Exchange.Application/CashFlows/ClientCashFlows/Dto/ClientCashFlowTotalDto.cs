using System.Collections.Generic;

namespace Bwr.Exchange.CashFlows.ClientCashFlows.Dto
{
    public class ClientCashFlowTotalDto
    {
        public ClientCashFlowTotalDto()
        {
            CurrencyBalances = new List<ClientCashFlowTotalDetailDto>();
        }
        #region Client 
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public bool IsActiveToday { get; set; }
        public bool IsMatching { get; set; }
        #endregion

        public IList<ClientCashFlowTotalDetailDto> CurrencyBalances { get; set; }
    }
}
