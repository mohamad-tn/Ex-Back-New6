using AutoMapper;
using Bwr.Exchange.Settings.Treasuries;
using Bwr.Exchange.Settings.Treasury.Dto.TreasuryBalance;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bwr.Exchange.Settings.Treasury.Map
{
    public class TreasuryBalanceMapProfile : Profile
    {
        public TreasuryBalanceMapProfile()
        {
            CreateMap<TreasuryBalance, TreasuryBalanceDto>();
            CreateMap<TreasuryBalanceDto, TreasuryBalance>();
        }
    }
}
