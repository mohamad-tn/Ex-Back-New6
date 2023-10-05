using Abp.Application.Services;
using Bwr.Exchange.CashFlows.ClientCashFlows.Dto;
using Bwr.Exchange.CashFlows.Shared.Dto;
using Bwr.Exchange.Shared.DataManagerRequests;
using Bwr.Exchange.Shared.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bwr.Exchange.CashFlows.ClientCashFlows
{
    public interface IClientCashFlowAppService : IApplicationService
    {
        IList<ClientCashFlowDto> Get(GetClientCashFlowInput input);
        ReadGrudDto GetForGrid([FromBody] CashFlowDataManagerRequest dm);
        IList<ClientCashFlowTotalDto> GetClientsBalances(string date);
        Task<CurrentClientBalanceDto> GetCurrentBalanceAsync(GetClientCashFlowInput input);
        Task<ClientCashFlowMatchingDto> MatchAsync(ClientCashFlowMatchingDto input);
        Task<IList<DefaultersOfPaymentDto>> GetDefaulters(int days);
        IList<SummaryCashFlowDto> Summary(string date);
    }
}
