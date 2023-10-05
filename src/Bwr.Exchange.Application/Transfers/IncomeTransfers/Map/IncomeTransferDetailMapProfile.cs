using AutoMapper;
using Bwr.Exchange.Transfers.IncomeTransfers.Dto;

namespace Bwr.Exchange.Transfers.IncomeTransfers.Map
{
    public class IncomeTransferDetailMapProfile : Profile
    {
        public IncomeTransferDetailMapProfile()
        {
            CreateMap<IncomeTransferDetail, IncomeTransferDetailDto>();

            CreateMap<IncomeTransferDetailDto, IncomeTransferDetail>()
                .ForMember(x => x.Sender, m => m.Ignore())
                .ForMember(x => x.Beneficiary, m => m.Ignore());

            CreateMap<IncomeTransferDetail, ReadNoneReceivedTransferDto>();
                //.ForMember(x=>x.incomeTransfer, m => m.Ignore());
        }
    }
}
