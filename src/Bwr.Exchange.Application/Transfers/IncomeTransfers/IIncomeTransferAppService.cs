using Abp.Application.Services;
using Bwr.Exchange.Transfers.IncomeTransfers.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bwr.Exchange.Transfers.IncomeTransfers
{
    public interface IIncomeTransferAppService : IApplicationService
    {
        Task<IncomeTransferDto> CreateAsync(IncomeTransferDto input);
        Task DeleteAsync(int id);
        IList<IncomeTransferDto> GetForEdit(IncomeTransferGetForEditInputDto input);
        Task<IncomeTransferDto> UpdateAsync(IncomeTransferDto input);
        int GetLastNumber();
        Task<Dictionary<string, double>> GetAllIncomeTransfersForBranch(long userId);

    }
}
