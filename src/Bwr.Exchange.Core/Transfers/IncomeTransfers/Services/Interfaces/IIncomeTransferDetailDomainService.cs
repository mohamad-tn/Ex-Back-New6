using Abp.Domain.Services;
using System.Threading.Tasks;

namespace Bwr.Exchange.Transfers.IncomeTransfers.Services.Interfaces
{
    public interface IIncomeTransferDetailDomainService : IDomainService
    {
        Task<IncomeTransferDetail> CreateCashFlowAsync();
        Task<IncomeTransferDetail> UpdateCashFlowAsync();
    }
}
