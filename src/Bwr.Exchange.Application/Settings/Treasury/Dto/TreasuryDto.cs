using Abp.Application.Services.Dto;
using Bwr.Exchange.Settings.Treasury.Dto.TreasuryBalance;
using System.Collections.Generic;

namespace Bwr.Exchange.Settings.Treasury.Dto
{
    public class TreasuryDto : EntityDto
    {
        //public TreasuryDto()
        //{
        //    TreasuryBalances = new List<TreasuryBalanceDto>();
        //}
        public string Name { get; set; }

        //public IList<TreasuryBalanceDto> TreasuryBalances { get; set; }
    }
}
