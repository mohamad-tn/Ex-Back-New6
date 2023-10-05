using Abp.Application.Services.Dto;

namespace Bwr.Exchange.Settings.Companies.Dto.CompanyBalance
{
    public class CompanyBalanceForEditInputDto : EntityDto
    {
        public int TransactionType { get; set; }
    }
}
