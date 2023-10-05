using Abp.Application.Services;
using Bwr.Exchange.Shared.DataManagerRequests;
using Bwr.Exchange.Shared.Dto;
using Bwr.Exchange.TreasuryActions.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bwr.Exchange.TreasuryActions
{
    public interface ITreasuryActionAppService : IApplicationService
    {
        Task<TreasuryActionDto> CreateAsync(TreasuryActionDto input);
        Task<TreasuryActionDto> UpdateAsync(TreasuryActionDto input);
        Task DeleteAsync(int id);
        Task<TreasuryActionDto> PayDirectTransferAsync(PayDirectTransferInputDto input);
        Task<TreasuryActionDto> GetForEditAsync(int id);
        Task<IList<ExchangePartyDto>> GetExchangeParties();
        Task<IList<TreasuryActionDto>> GetAsync(SearchTreasuryActionDto input);
        IList<TreasuryActionStatementOutputDto> GetFroStatment(TreasuryActionStatementInputDto input);
        ReadGrudDto GetForGrid([FromBody] TreasuryActionDataManagerRequest dm);
        int GetLastNumber();
        
    }
}
