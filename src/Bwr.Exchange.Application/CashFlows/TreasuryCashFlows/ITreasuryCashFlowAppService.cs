using Abp.Application.Services;
using Bwr.Exchange.CashFlows.Shared.Dto;
using Bwr.Exchange.CashFlows.TreasuryCashFlows.Dto;
using Bwr.Exchange.Shared.DataManagerRequests;
using Bwr.Exchange.Shared.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bwr.Exchange.CashFlows.TreasuryCashFlows
{
    public interface ITreasuryCashFlowAppService : IApplicationService
    {
        IList<TreasuryCashFlowDto> Get(GetTreasuryCashFlowInput input);
        Task<IList<SummaryCashFlowDto>> Summary(string date);
        ReadGrudDto GetForGrid([FromBody] TreasuryCashFlowDataManagerRequest dm);
        //Task<TreasuryCashFlowMatchingDto> MatchAsync(TreasuryCashFlowMatchingDto input);
    }
}
