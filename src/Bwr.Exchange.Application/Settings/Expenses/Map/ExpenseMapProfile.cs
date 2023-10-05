using AutoMapper;
using Bwr.Exchange.Settings.Expenses.Dto;

namespace Bwr.Exchange.Settings.Expenses.Map
{
    public class ExpenseMapProfile:Profile
    {
        public ExpenseMapProfile()
        {
            CreateMap<Expense, ExpenseDto>();
            CreateMap<Expense, ReadExpenseDto>();
            CreateMap<Expense, CreateExpenseDto>();
            CreateMap<CreateExpenseDto, Expense>();
            CreateMap<Expense, UpdateExpenseDto>();
            CreateMap<UpdateExpenseDto, Expense>();
        }
    }
}
