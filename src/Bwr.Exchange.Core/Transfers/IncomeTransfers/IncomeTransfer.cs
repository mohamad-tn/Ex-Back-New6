using Bwr.Exchange.Settings.Companies;
using Bwr.Exchange.Shared;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bwr.Exchange.Transfers.IncomeTransfers
{
    public class IncomeTransfer : MoneyAction
    {
        public IncomeTransfer()
        {
            IncomeTransferDetails = new List<IncomeTransferDetail>();
        }

        public int Number { get; set; }

        #region Company
        public int? CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; }
        #endregion

        #region IncomeTransferDetails
        public virtual IList<IncomeTransferDetail> IncomeTransferDetails { get; set; }
        public virtual void AddIncomeTransferDetail(IncomeTransferDetail detail)
        {
            detail.IncomeTransfer = this;
            IncomeTransferDetails.Add(detail);
        }
        #endregion
    }
}
