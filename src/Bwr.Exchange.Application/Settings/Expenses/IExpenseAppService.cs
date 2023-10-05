
using Abp.Application.Services;
using Bwr.Exchange.Settings.Expenses.Dto;
using Bwr.Exchange.Shared.Interfaces;

namespace Bwr.Exchange.Settings.Expenses
{
    public interface IExpenseAppService : IApplicationService, ICrudEjAppService<ExpenseDto, CreateExpenseDto, UpdateExpenseDto>
    {

    }
}
