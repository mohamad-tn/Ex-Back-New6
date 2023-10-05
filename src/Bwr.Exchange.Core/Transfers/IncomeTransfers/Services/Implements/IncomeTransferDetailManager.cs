using Abp.Domain.Repositories;
using Bwr.Exchange.Transfers.IncomeTransfers.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bwr.Exchange.Transfers.IncomeTransfers.Services.Implements
{
    public class IncomeTransferDetailManager : IIncomeTransferDetailManager
    {
        private readonly IRepository<IncomeTransferDetail> _incomeTransferDetailRepository;

        public IncomeTransferDetailManager(IRepository<IncomeTransferDetail> incomeTransferDetailRepository)
        {
            _incomeTransferDetailRepository = incomeTransferDetailRepository;
        }

        public Task<IncomeTransferDetail> UpdateAsync(IncomeTransferDetail input)
        {
            return _incomeTransferDetailRepository.UpdateAsync(input);
        }

        public IQueryable<IncomeTransferDetail> GetNotReceived()
        {
            return _incomeTransferDetailRepository.GetAllIncluding(
                b => b.Beneficiary,
                s => s.Sender,
                c => c.Currency,
                tco => tco.ToCompany,
                tcl => tcl.ToCompany).Include(i => i.IncomeTransfer).ThenInclude(t=>t.Company)
                .Where(x => x.Status != IncomeTransferDetailStatus.Received && x.PaymentType == PaymentType.Cash);
        }

        public IQueryable<IncomeTransferDetail> GetAllDirectTransfers()
        {
            return _incomeTransferDetailRepository.GetAllIncluding(
                b => b.Beneficiary,
                s => s.Sender,
                c => c.Currency,
                tco => tco.ToCompany,
                tcl => tcl.ToCompany).Where(x => x.PaymentType == PaymentType.Cash).OrderBy(x => x.PaymentType);


        }

        public async Task<IncomeTransferDetail> GetByIdAsync(int id)
        {
            return await _incomeTransferDetailRepository.GetAsync(id);
        }

        public async Task<IncomeTransferDetail> GetByIdAsync(int id, params Expression<Func<IncomeTransferDetail, object>>[] propertySelectors)
        {
            var incomeTransferDetail = await _incomeTransferDetailRepository.GetAsync(id);
            foreach (var propertySelector in propertySelectors)
            {
                await _incomeTransferDetailRepository.EnsurePropertyLoadedAsync(incomeTransferDetail, propertySelector);
            }

            return incomeTransferDetail;
        }

        public async Task<IncomeTransferDetail> ChangeStatusAsync(int id, int status)
        {
            var incomeTransferDetail = await GetByIdAsync(id);
            incomeTransferDetail.Status = (IncomeTransferDetailStatus) status;
            return await UpdateAsync(incomeTransferDetail);
        }

        public async Task DeteleAsync(IncomeTransferDetail input)
        {
            await _incomeTransferDetailRepository.DeleteAsync(input);
        }

        public async Task HardDeteleAsync(IncomeTransferDetail input)
        {
            await _incomeTransferDetailRepository.DeleteAsync(input);
        }

        public async Task<IList<IncomeTransferDetail>> GetByIncomeTransferIdAsync(int id)
        {
            return await _incomeTransferDetailRepository.GetAllListAsync(x => x.IncomeTransferId == id);
        }

        public async Task<IncomeTransferDetail> CreateAsync(IncomeTransferDetail input)
        {
            var id = await _incomeTransferDetailRepository.InsertAndGetIdAsync(input);
            return await _incomeTransferDetailRepository.GetAsync(id);
        }

        public IQueryable<IncomeTransferDetail> GetNotReceivedToDate(DateTime toDae)
        {
            return _incomeTransferDetailRepository.GetAllIncluding(
                c => c.Currency,
                t=>t.IncomeTransfer
                )
                .Where(x => x.Status != IncomeTransferDetailStatus.Received && x.PaymentType == PaymentType.Cash && x.IncomeTransfer.Date <= toDae);
        }
    }
}
