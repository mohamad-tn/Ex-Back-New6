using System;
using System.Collections.Generic;

namespace Bwr.Exchange.CashFlows.ClientCashFlows.Dto
{
    public class DefaultersOfPaymentDto
    {
        public DefaultersOfPaymentDto()
        {
            CurrencyBalances = new List<ClientCashFlowTotalDetailDto>();
        }
        #region Client 
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        #endregion

        public IList<ClientCashFlowTotalDetailDto> CurrencyBalances { get; set; }
        public DateTime? LastActionDate { get; set; }
    }
}
