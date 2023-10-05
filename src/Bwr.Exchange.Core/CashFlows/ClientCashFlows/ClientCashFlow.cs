using Bwr.Exchange.Settings.Clients;
using Bwr.Exchange.Shared;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bwr.Exchange.CashFlows.ClientCashFlows
{
    public class ClientCashFlow : CashFlowBase
    {
        #region Client 
        public int ClientId { get; set; }
        [ForeignKey("ClientId")]
        public virtual Client Client { get; set; }
        #endregion
        
        public double ClientCommission { get; set; }
    }
}
