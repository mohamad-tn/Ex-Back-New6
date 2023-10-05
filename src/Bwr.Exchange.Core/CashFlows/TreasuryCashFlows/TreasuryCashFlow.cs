using Abp.Domain.Entities.Auditing;
using Bwr.Exchange.Settings.Currencies;
using Bwr.Exchange.Settings.Treasuries;
using Bwr.Exchange.Shared;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bwr.Exchange.CashFlows.TreasuryCashFlows
{
    public class TreasuryCashFlow : CashFlowBase
    {
        public string Name { get; set; }

        #region Treasury 
        public int TreasuryId { get; set; }
        [ForeignKey("TreasuryId")]
        public virtual Treasury Treasury { get; set; }
        #endregion
    }
}
