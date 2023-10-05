using Abp.Application.Services.Dto;
using System;

namespace Bwr.Exchange.TreasuryActions.Dto
{
    public class TreasuryActionDto : EntityDto
    {
        public int Number { get; set; }
        public int ActionType { get; set; }
        public string Date { get; set; }
        public int MainAccount { get; set; }
        public int? CurrencyId { get; set; }
        public int? ExchangePartyCompanyId { get; set; }
        public int? ExchangePartyClientId { get; set; }
        public int? MainAccountCompanyId { get; set; }
        public int? MainAccountClientId { get; set; }
        public int? ExpenseId { get; set; }
        public int? IncomeId { get; set; }
        public int? TreasuryId { get; set; }
        public int? IncomeTransferDetailId { get; set; }
        public double Amount { get; set; }
        public string Note { get; set; }
        public string InstrumentNo { get; set; }
        public string IdentificationNumber { get; set; }
        public string Issuer { get; set; }

    }
}
