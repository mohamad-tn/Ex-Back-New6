using Abp.Events.Bus;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bwr.Exchange.Transfers.IncomeTransfers.Events
{
    public class ChangeIncomeTransferDetailStatusEventData : EventData
    {
        public ChangeIncomeTransferDetailStatusEventData(
            IncomeTransferDetail incomeTransferDetail,
            IncomeTransferDetailStatus newStatus)
        {
            IncomeTransferDetail = incomeTransferDetail;
            IncomeTransferDetail.Status = newStatus;

        }

        public IncomeTransferDetail IncomeTransferDetail { get; set; }
        public IncomeTransferDetailStatus NewStatus { get; set; }
    }
}
