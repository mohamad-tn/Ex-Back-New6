using Bwr.Exchange.Settings.Clients;
using Bwr.Exchange.Settings.Companies;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bwr.Exchange.Transfers.IncomeTransfers
{
    public class IncomeTransferDetail : Transfer
    {
        public int Index { get; set; }
        public IncomeTransferDetailStatus Status { get; set; }

        #region Receiver

        #region To Company
        public int? ToCompanyId { get; set; }
        [ForeignKey("ToCompanyId")]
        public virtual Company ToCompany { get; set; }
        #endregion

        #region To Client
        public int? ToClientId { get; set; }
        [ForeignKey("ToClientId")]
        public virtual Client ToClient { get; set; }
        #endregion

        #endregion

        #region Income Transfer
        public int IncomeTransferId { get; set; }
        [ForeignKey("IncomeTransferId")]
        public virtual IncomeTransfer IncomeTransfer { get; set; }
        #endregion
    }
}
