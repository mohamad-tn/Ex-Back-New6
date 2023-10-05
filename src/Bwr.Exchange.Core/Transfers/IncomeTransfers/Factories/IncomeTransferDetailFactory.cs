using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Bwr.Exchange.Transfers.IncomeTransfers.Services.Implements;
using Bwr.Exchange.Transfers.IncomeTransfers.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Bwr.Exchange.Transfers.IncomeTransfers.Factories
{
    public class IncomeTransferDetailFactory : IIncomeTransferDetailFactory
    {
        private readonly IRepository<IncomeTransferDetail> _incomeTransferRepository;
        public IncomeTransferDetailFactory(
            IRepository<IncomeTransferDetail> incomeTransferRepository)
        {
            _incomeTransferRepository = incomeTransferRepository;
        }

        public IIncomeTransferDetailDomainService CreateService(IncomeTransferDetail input)
        {
            var incomeTreansferDetail = GetByIdWithDetail(input.Id);
            switch (input.PaymentType)
            {
                case PaymentType.Cash:
                    return new IncomeTransferDetailToCashDomainService(_incomeTransferRepository, incomeTreansferDetail);
                case PaymentType.Client:
                    return new IncomeTransferDetailToClientDomainService(_incomeTransferRepository, incomeTreansferDetail);
                case PaymentType.Company:
                    return new IncomeTransferDetailToCompanyDomainService(_incomeTransferRepository, incomeTreansferDetail);
                default:
                    return null;
            }
        }

        private IncomeTransferDetail GetByIdWithDetail(int id)
        {
            return _incomeTransferRepository.
                GetAllIncluding(
                    m => m.ToCompany,
                    s => s.Sender,
                    b => b.Beneficiary,
                    tc => tc.ToClient).Include(i => i.IncomeTransfer)
                    .ThenInclude(c => c.Company)
                .FirstOrDefault(x => x.Id == id);
        }
    }
}
