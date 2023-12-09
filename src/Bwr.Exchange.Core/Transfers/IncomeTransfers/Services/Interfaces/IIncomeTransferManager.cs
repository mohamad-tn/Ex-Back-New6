using Abp.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bwr.Exchange.Transfers.IncomeTransfers.Services.Interfaces
{
    public interface IIncomeTransferManager : IDomainService
    {
        Task<IncomeTransfer> CreateAsync(IncomeTransfer input);
        Task<IncomeTransfer> UpdateAsync(IncomeTransfer input);
        Task<IncomeTransfer> GetByIdAsync(int id);
        Task<IList<IncomeTransfer>> GetAsync(Dictionary<string, object> dic);
        IQueryable<IncomeTransfer> Get(DateTime fromDate, DateTime toDate, int? companyId, int? number);
        IList<IncomeTransfer> Get(Dictionary<string, object> dic);
        IncomeTransfer GetById(int id);
        Task<bool> DeleteDetailsAsync(IncomeTransfer incomeTransfer);
        Task CreateCashFlowAsync(IncomeTransfer createdIncomeTransfer);
        int GetLastNumber(int branchId);
        Task<bool> DeleteAsync(IncomeTransfer incomeTransfer);
        Task<Dictionary<string, double>> GetAllForBranch(int? branchId);
    }
}
