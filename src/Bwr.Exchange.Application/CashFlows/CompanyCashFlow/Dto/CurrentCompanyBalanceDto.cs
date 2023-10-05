using Bwr.Exchange.Settings.Companies.Dto;
using Bwr.Exchange.Settings.Currencies.Dto;

namespace Bwr.Exchange.CashFlows.CompanyCashFlow.Dto
{
    public class CurrentCompanyBalanceDto
    {
        public CurrentCompanyBalanceDto(CompanyDto client, CurrencyDto currency, double balance)
        {
            Company = client;
            Currency = currency;
            Balance = balance;
        }

        public CompanyDto Company { get; set; }
        public CurrencyDto Currency { get; set; }

        public double Balance { get; set; }
    }
}
