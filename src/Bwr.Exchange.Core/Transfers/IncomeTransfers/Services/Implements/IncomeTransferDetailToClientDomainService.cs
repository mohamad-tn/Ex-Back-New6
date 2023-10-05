using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Bwr.Exchange.Transfers.IncomeTransfers.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace Bwr.Exchange.Transfers.IncomeTransfers.Services.Implements
{
    public class IncomeTransferDetailToClientDomainService : IIncomeTransferDetailDomainService
    {
        private readonly IRepository<IncomeTransferDetail> _incomeTransferDetailRepository;
        public IncomeTransferDetail IncomeTransferDetail { get; set; }

        public IncomeTransferDetailToClientDomainService(
            IRepository<IncomeTransferDetail> incomeTransferDetailRepository,
            IncomeTransferDetail incomeTransferDetail)
        {
            _incomeTransferDetailRepository = incomeTransferDetailRepository;
            IncomeTransferDetail = incomeTransferDetail;
        }
        public Task<IncomeTransferDetail> CreateCashFlowAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IncomeTransferDetail> UpdateCashFlowAsync()
        {
            throw new NotImplementedException();
        }
        
    }
}