using Abp.Application.Services.Dto;

namespace Bwr.Exchange.Settings.Expenses.Dto
{
    public class UpdateExpenseDto : EntityDto
    {
        public string Name { get; set; }
    }
}
