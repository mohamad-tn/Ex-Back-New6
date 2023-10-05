using Abp.Application.Services;
using Bwr.Exchange.Settings.Treasury.Dto.TreasuryBalance;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bwr.Exchange.Settings.Treasury
{
    public interface ITreasuryBalanceAppService: IApplicationService
    {
        Task<IList<TreasuryBalanceDto>> GetAllWithDetails();
        Task<TreasuryBalanceDto> InsertAndGetAsync(TreasuryBalanceDto input);
        Task<TreasuryBalanceDto> UpdateAndGetAsync(TreasuryBalanceDto input);
    }
}
