using AutoMapper;
using Bwr.Exchange.Transfers.IncomeTransfers.Dto;
using Bwr.Exchange.Transfers.IncomeTransfers;
using System;
using System.Collections.Generic;
using System.Text;
using Bwr.Exchange.CashFlows.ManagementStatement.Dto;

namespace Bwr.Exchange.CashFlows.ManagementStatement.Map
{
    public class ManagementMapProfile : Profile
    {
        public ManagementMapProfile()
        {
            CreateMap<Management, ManagementDto>();
            CreateMap<Management, CreateManagementDto>();
            CreateMap<CreateManagementDto, Management>();
            CreateMap<ManagementDto, Management>();
        }
    }
}
