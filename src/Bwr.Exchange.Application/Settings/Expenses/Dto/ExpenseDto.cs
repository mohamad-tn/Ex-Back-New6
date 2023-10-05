using Abp.Application.Services.Dto;

namespace Bwr.Exchange.Settings.Expenses.Dto
{
    public class ExpenseDto : EntityDto
    {
        public string Name { get; set; }
        public int? BranchId { get; set; }

    }
}
