using AutoMapper;
using Bwr.Exchange.Settings.Incomes.Dto;

namespace Bwr.Exchange.Settings.Incomes.Map
{
    public class IncomeMapProfile:Profile
    {
        public IncomeMapProfile()
        {
            CreateMap<Income, IncomeDto>();
            CreateMap<Income, ReadIncomeDto>();
            CreateMap<Income, CreateIncomeDto>();
            CreateMap<CreateIncomeDto, Income>();
            CreateMap<Income, UpdateIncomeDto>();
            CreateMap<UpdateIncomeDto, Income>();
        }
    }
}
