using Bwr.Exchange.Settings.Companies.Dto.CompanyBalances;

namespace Bwr.Exchange.Settings.Companies.Dto.CompanyBalance
{
    public class CompanyBalanceWithCurrencyDto : CompanyBalanceDto
    {
        public CompanyBalanceWithCurrencyDto(double balance, int companyId, int currencyId, string currencyName)
        {
            Balance = balance;
            CompanyId = companyId;
            CurrencyId = currencyId;
            CurrencyName = currencyName;
        }

        public double Balance { get; set; }
        public int CompanyId { get; set; }
        public int CurrencyId { get; set; }
        public string CurrencyName { get; set; }
    }
}
