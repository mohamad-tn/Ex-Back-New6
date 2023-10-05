using Abp.Application.Services.Dto;

namespace Bwr.Exchange.Settings.Incomes.Dto
{
    public class IncomeDto : EntityDto
    {
        public string Name { get; set; }
        public int? BranchId { get; set; }

    }
}
