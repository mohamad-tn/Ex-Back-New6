using Abp.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bwr.Exchange.Transfers.IncomeTransfers.Services.Interfaces
{
    public interface IIncomeTransferDetailManager: IDomainService
    {
        IQueryable<IncomeTransferDetail> GetNotReceived();
        IQueryable<IncomeTransferDetail> GetNotReceivedToDate(DateTime toDae);
        IQueryable<IncomeTransferDetail> GetAllDirectTransfers();
        Task<IncomeTransferDetail> GetByIdAsync(int id);
        Task<IncomeTransferDetail> GetByIdAsync(int id, params Expression<Func<IncomeTransferDetail, object>>[] propertySelectors);
        Task<IncomeTransferDetail> UpdateAsync(IncomeTransferDetail input);
        Task<IncomeTransferDetail> CreateAsync(IncomeTransferDetail input);
        Task DeteleAsync(IncomeTransferDetail input);
        Task HardDeteleAsync(IncomeTransferDetail input);
        Task<IncomeTransferDetail> ChangeStatusAsync(int id, int status);
        Task<IList<IncomeTransferDetail>> GetByIncomeTransferIdAsync(int id);
        
    }
}
