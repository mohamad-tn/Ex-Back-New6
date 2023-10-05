using Abp.Application.Services.Dto;

namespace Bwr.Exchange.Settings.Companies.Dto.CompanyBalances
{
    public class CompanyBalanceDto : EntityDto
    {
        public double Balance { get; set; }
        public int CompanyId { get; set; }
        public int CurrencyId { get; set; }
    }
}
