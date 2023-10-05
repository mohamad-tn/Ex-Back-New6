using Abp.Application.Services.Dto;
using Bwr.Exchange.Settings.Clients.Dto;
using Bwr.Exchange.Settings.Companies.Dto;
using Bwr.Exchange.Settings.Currencies.Dto;
using Bwr.Exchange.Settings.Expenses.Dto;
using Bwr.Exchange.Settings.Incomes.Dto;
using Bwr.Exchange.Settings.Treasury.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bwr.Exchange.TreasuryActions.Dto
{
    public class ListTreasuryActionDto: EntityDto
    {
        public int Number { get; set; }
        public int ActionType { get; set; }
        public string Date { get; set; }
        public int MainAccount { get; set; }
        public int? CurrencyId { get; set; }
        public CurrencyDto Currency { get; set; }
        public int? ExchangePartyCompanyId { get; set; }
        public CompanyDto ExchangePartyCompany { get; set; }
        public int? ExchangePartyClientId { get; set; }
        public ClientDto ExchangePartyClient { get; set; }
        public int? MainAccountCompanyId { get; set; }
        public CompanyDto MainAccountCompany { get; set; }
        public int? MainAccountClientId { get; set; }
        public ClientDto MainAccountClient { get; set; }
        public int? ExpenseId { get; set; }
        public ExpenseDto Expense { get; set; }
        public int? IncomeId { get; set; }
        public IncomeDto Income { get; set; }
        public int? TreasuryId { get; set; }
        public TreasuryDto Treasury { get; set; }
        public int? IncomeTransferDetailId { get; set; }
        public double Amount { get; set; }
        public string Note { get; set; }
        public string InstrumentNo { get; set; }
        public string IdentificationNumber { get; set; }
        public string Issuer { get; set; }
        public int BranchId { get; set; }

    }
}
