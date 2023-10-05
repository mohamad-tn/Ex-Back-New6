using AutoMapper;
using Bwr.Exchange.Transfers.IncomeTransfers.Dto;

namespace Bwr.Exchange.Transfers.IncomeTransfers.Map
{
    public class IncomeTransferMapProfile : Profile
    {
        public IncomeTransferMapProfile()
        {
            CreateMap<IncomeTransfer, IncomeTransferDto>();
            CreateMap<IncomeTransfer, ReadIncomeTransferDto>();
            CreateMap<IncomeTransferDto, IncomeTransfer>();
            
        }
    }
}
