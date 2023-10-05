using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Events.Bus;
using Bwr.Exchange.Transfers.IncomeTransfers.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace Bwr.Exchange.Transfers.IncomeTransfers.Services.Implements
{
    public class IncomeTransferDetailToCashDomainService : IIncomeTransferDetailDomainService
    {
        private readonly IRepository<IncomeTransferDetail> _incomeTransferRepository;
        public IncomeTransferDetail IncomeTransferDetail { get; set; }

        public IncomeTransferDetailToCashDomainService(
            IRepository<IncomeTransferDetail> incomeTransferRepository,
            IncomeTransferDetail incomeTransferDetail)
        {
            _incomeTransferRepository = incomeTransferRepository;
            IncomeTransferDetail = incomeTransferDetail;
        }
        public Task<IncomeTransferDetail> CreateCashFlowAsync()
        {
            // حوالة مبارشرة
            throw new Exception();
        }

        public Task<IncomeTransferDetail> UpdateCashFlowAsync()
        {
            throw new NotImplementedException();
        }
    }
}
