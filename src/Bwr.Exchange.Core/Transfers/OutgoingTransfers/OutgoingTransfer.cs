using Bwr.Exchange.Settings.Branches;
using Bwr.Exchange.Settings.Clients;
using Bwr.Exchange.Settings.Companies;
using Bwr.Exchange.Settings.Countries;
using Bwr.Exchange.Settings.Treasuries;
using Bwr.Exchange.Transfers.OutgoingTransfers;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bwr.Exchange.Transfers
{
    public class OutgoingTransfer : Transfer
    {
        public DateTime Date { get; set; }

        #region Receiver
        public int ToCompanyId { get; set; }
        [ForeignKey("ToCompanyId")]
        public virtual Company ToCompany { get; set; }
        #endregion

        #region Country
        public int CountryId { get; set; }
        [ForeignKey("CountryId")]
        public virtual Country Country { get; set; }
        #endregion

        #region Sender

        #region From Company
        public int? FromCompanyId { get; set; }
        [ForeignKey("FromCompanyId")]
        public virtual Company FromCompany { get; set; }
        #endregion

        #region From Client
        public int? FromClientId { get; set; }
        [ForeignKey("FromClientId")]
        public virtual Client FromClient { get; set; }
        #endregion

        #region From Branch
        public int? FromBranchId { get; set; }
        [ForeignKey("FromBranchId")]
        public virtual Branch FromBranch { get; set; }
        #endregion

        #region To Branch
        public int? ToBranchId { get; set; }
        [ForeignKey("ToBranchId")]
        public virtual Branch ToBranch { get; set; }
        #endregion

        #region Treasury
        public int? TreasuryId { get; set; }
        [ForeignKey("TreasuryId")]
        public virtual Treasury Treasury { get; set; }
        #endregion

        #endregion

        public double ReceivedAmount { get; set; }
        public string InstrumentNo { get; set; } //رقم الصك
        public string Reason { get; set; }
        public string Note { get; set; }
        public OutgoingTransferStatus Status { get; set; }
    }
}
