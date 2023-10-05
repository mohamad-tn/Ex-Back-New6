using Abp.Domain.Entities.Auditing;
using Bwr.Exchange.Settings.Clients;
using Bwr.Exchange.Settings.Companies;
using Bwr.Exchange.Settings.Currencies;
using Bwr.Exchange.Settings.Expenses;
using Bwr.Exchange.Settings.Incomes;
using Bwr.Exchange.Settings.Treasuries;
using Bwr.Exchange.Transfers.IncomeTransfers;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bwr.Exchange.TreasuryActions
{
    public class TreasuryAction : FullAuditedEntity
    {
        public int Number { get; set; }
        public TreasuryActionType ActionType { get; set; }
        public DateTime Date { get; set; }
        public MainAccountType MainAccount { get; set; }

        #region Currency 
        public int? CurrencyId { get; set; }
        [ForeignKey("CurrencyId")]
        public virtual Currency Currency { get; set; } 
        #endregion

        #region Exchange Party Company 
        public int? ExchangePartyCompanyId { get; set; }
        [ForeignKey("ExchangePartyCompanyId")]
        public virtual Company ExchangePartyCompany { get; set; }
        #endregion

        #region Exchange Party Client 
        public int? ExchangePartyClientId { get; set; }
        [ForeignKey("ExchangePartyClientId")]
        public virtual Client ExchangePartyClient { get; set; }
        #endregion

        #region Main Account Company 
        public int? MainAccountCompanyId { get; set; }
        [ForeignKey("MainAccountCompanyId")]
        public virtual Company MainAccountCompany { get; set; }
        #endregion

        #region Main Account Client 
        public int? MainAccountClientId { get; set; }
        [ForeignKey("MainAccountClientId")]
        public virtual Client MainAccountClient { get; set; }
        #endregion

        #region Treasury 
        public int? TreasuryId { get; set; }
        [ForeignKey("TreasuryId")]
        public virtual Treasury Treasury { get; set; }
        #endregion

        #region Expense 
        public int? ExpenseId { get; set; }
        [ForeignKey("ExpenseId")]
        public virtual Expense Expense { get; set; }
        #endregion

        #region Income 
        public int? IncomeId { get; set; }
        [ForeignKey("IncomeId")]
        public virtual Income Income { get; set; }
        #endregion

        #region Income Transfer Detail
        public int? IncomeTransferDetailId { get; set; }
        [ForeignKey("IncomeTransferDetailId")]
        public virtual IncomeTransferDetail IncomeTransferDetail { get; set; }
        #endregion

        public double Amount { get; set; }
        public string Note { get; set; }
        public string InstrumentNo { get; set; }
        public string IdentificationNumber { get; set; }
        public string Issuer { get; set; }
        public int BranchId { get; set; }

    }
}
