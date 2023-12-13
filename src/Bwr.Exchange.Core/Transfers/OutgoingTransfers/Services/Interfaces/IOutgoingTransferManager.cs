using Abp.Domain.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bwr.Exchange.Transfers.OutgoingTransfers.Services
{
    public interface IOutgoingTransferManager : IDomainService
    {
        Task<OutgoingTransfer> CreateAsync(OutgoingTransfer input);
        Task<OutgoingTransfer> UpdateAsync(OutgoingTransfer input);
        Task<OutgoingTransfer> DeleteAsync(OutgoingTransfer outgoingTransfer);
        Task<bool> DeleteCashFlowAsync(OutgoingTransfer input);
        Task<OutgoingTransfer> GetByIdAsync(int id);
        Task<IList<OutgoingTransfer>> GetAsync(Dictionary<string, object> dic);
        IList<OutgoingTransfer> Get(Dictionary<string, object> dic);
        OutgoingTransfer GetById(int id);
        int GetLastNumber(int branchId);
        IList<OutgoingTransfer> GetAllNotCompleted(int? branchId);
        Task<OutgoingTransfer> AcceptOutgoingTransferFromBranchAsync(int outgoingTransferId, int branchId);
        Task<OutgoingTransfer> RejectOutgoingTransferFromBranchAsync(int outgoingTransferId);
        Task<Dictionary<string,double>> GetAllForBranch(int? branchId);
    }
}
