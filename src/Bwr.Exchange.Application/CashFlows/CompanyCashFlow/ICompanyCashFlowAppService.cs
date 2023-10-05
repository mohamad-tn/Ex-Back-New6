using Abp.Application.Services;
using Bwr.Exchange.CashFlows.CompanyCashFlow.Dto;
using Bwr.Exchange.CashFlows.CompanyCashFlows.Dto;
using Bwr.Exchange.CashFlows.Shared.Dto;
using Bwr.Exchange.Shared.DataManagerRequests;
using Bwr.Exchange.Shared.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bwr.Exchange.CashFlows.CompanyCashFlows
{
    public interface ICompanyCashFlowAppService : IApplicationService
    {
        IList<CompanyCashFlowDto> Get(GetCompanyCashFlowInput input);
        ReadGrudDto GetForGrid([FromBody] CashFlowDataManagerRequest dm);
        IList<CompanyCashFlowTotalDto> GetCompanysBalances(string date);
        Task<CurrentCompanyBalanceDto> GetCurrentBalanceAsync(GetCompanyCashFlowInput input);
        IList<SummaryCashFlowDto> Summary(string date);
        //Task<CompanyCashFlowMatchingDto> MatchAsync(CompanyCashFlowMatchingDto input);
    }
}
