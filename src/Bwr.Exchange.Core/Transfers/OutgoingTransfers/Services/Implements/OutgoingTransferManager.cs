using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Events.Bus;
using Bwr.Exchange.Settings.Clients.Services;
using Bwr.Exchange.Settings.Companies.Services;
using Bwr.Exchange.Settings.Currencies;
using Bwr.Exchange.Transfers.IncomeTransfers;
using Bwr.Exchange.Transfers.IncomeTransfers.Services.Interfaces;
using Bwr.Exchange.Transfers.OutgoingTransfers.Events;
using Bwr.Exchange.Transfers.OutgoingTransfers.Factories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bwr.Exchange.Transfers.OutgoingTransfers.Services
{
    public class OutgoingTransferManager : IOutgoingTransferManager
    {
        private readonly IRepository<OutgoingTransfer> _outgoingTransferRepository;
        private readonly IRepository<Currency> _currencyRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IOutgoingTransferFactory _outgoingTransferFactory;

        public OutgoingTransferManager(
            IRepository<OutgoingTransfer> outgoingTransferRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IOutgoingTransferFactory outgoingTransferFactory,
            IRepository<Currency> currencyRepository)
        {
            _outgoingTransferRepository = outgoingTransferRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _outgoingTransferFactory = outgoingTransferFactory;
            _currencyRepository = currencyRepository;
        }

        //public async Task<OutgoingTransfer> CreateAsync(OutgoingTransfer input)
        //{
        //    //Date and time
        //    var currentDate = DateTime.Now;
        //    input.Date = new DateTime(
        //        input.Date.Year,
        //        input.Date.Month,
        //        input.Date.Day,
        //        currentDate.Hour,
        //        currentDate.Minute,
        //        currentDate.Second
        //        );

        //    var id = await _outgoingTransferRepository.InsertAndGetIdAsync(input);
        //    var createdOutgoingTransfer = GetByIdWithDetail(id);

        //    IOutgoingTransferDomainService service = _outgoingTransferFactory.CreateService(input);
        //    await service.CreateCashFlowAsync(createdOutgoingTransfer);

        //    return createdOutgoingTransfer;
        //}

        public async Task<OutgoingTransfer> GetByIdAsync(int id)
        {
            return await _outgoingTransferRepository.FirstOrDefaultAsync(id);
        }

        public OutgoingTransfer GetById(int id)
        {
            //return _outgoingTransferRepository.GetAllIncluding(
            //    b => b.Beneficiary,
            //    s => s.Sender,
            //    tb=>tb.ToBranch
            //    ).Where(x => x.Id == id).FirstOrDefault();
            return GetByIdWithDetail(id);
        }

        public async Task<IList<OutgoingTransfer>> GetAsync(Dictionary<string, object> dic)
        {
            IEnumerable<OutgoingTransfer> outgoingTransfers = await _outgoingTransferRepository.GetAllListAsync(x =>
              x.Date >= DateTime.Parse(dic["FromDate"].ToString()) &&
              x.Date >= DateTime.Parse(dic["ToDate"].ToString()));

            if (outgoingTransfers != null && outgoingTransfers.Any())
            {
                if (dic["PaymentType"] != null)
                {
                    outgoingTransfers = outgoingTransfers
                        .Where(x => x.PaymentType == (PaymentType)int.Parse(dic["PaymantType"].ToString()));
                }

                if (dic["ClientId"] != null)
                {
                    outgoingTransfers = outgoingTransfers
                        .Where(x => x.FromClientId == int.Parse(dic["ClientId"].ToString()));
                }
            }

            return outgoingTransfers.ToList();
        }

        public IList<OutgoingTransfer> Get(Dictionary<string, object> dic)
        {
            IEnumerable<OutgoingTransfer> outgoingTransfers = GetAllWithDetails();

            if (outgoingTransfers != null && outgoingTransfers.Any())
            {
                int number = 0;
                int.TryParse(dic["Number"].ToString(), out number);
                if (number != 0)
                {
                    outgoingTransfers = outgoingTransfers
                        .Where(x => x.Number == int.Parse(dic["Number"].ToString()));
                }
                else
                {
                    if (dic["FromDate"] != null && dic["ToDate"] != null)
                    {
                        outgoingTransfers = outgoingTransfers.Where(x =>
                          x.Date >= DateTime.Parse(dic["FromDate"].ToString()) &&
                          x.Date <= DateTime.Parse(dic["ToDate"].ToString())).ToList();
                    }
                    //
                    if (dic["PaymentType"] != null)
                    {
                        int paymentType = 0;
                        int.TryParse(dic["PaymentType"].ToString(), out paymentType);

                        if (paymentType != 0)
                        {
                            outgoingTransfers = outgoingTransfers
                                .Where(x => x.PaymentType == (PaymentType)int.Parse(dic["PaymentType"].ToString()));
                        }
                    }
                    //
                    if (dic["ClientId"] != null)
                    {
                        int clientId = 0;
                        int.TryParse(dic["ClientId"].ToString(), out clientId);

                        if (clientId != 0)
                        {
                            outgoingTransfers = outgoingTransfers
                                .Where(x => x.FromClientId == int.Parse(dic["ClientId"].ToString()));
                        }
                    }
                    //
                    if (dic["CompanyId"] != null)
                    {
                        int companyId = 0;
                        int.TryParse(dic["CompanyId"].ToString(), out companyId);

                        if (companyId != 0)
                        {
                            outgoingTransfers = outgoingTransfers
                                .Where(x => x.FromCompanyId == int.Parse(dic["CompanyId"].ToString()));
                        }
                    }
                    //
                    if (dic["CountryId"] != null)
                    {
                        int CountryId = 0;
                        int.TryParse(dic["CountryId"].ToString(), out CountryId);

                        if (CountryId != 0)
                        {
                            outgoingTransfers = outgoingTransfers
                                .Where(x => x.CountryId == int.Parse(dic["CountryId"].ToString()));
                        }
                    }

                    if (dic["Beneficiary"] != null)
                    {
                        outgoingTransfers = outgoingTransfers
                            .Where(x => x.Beneficiary != null && x.Beneficiary.Name.Contains(dic["Beneficiary"].ToString()));
                    }

                    if (dic["BeneficiaryAddress"] != null)
                    {
                        outgoingTransfers = outgoingTransfers
                            .Where(x => x.Beneficiary != null && x.Beneficiary.Address.Contains(dic["BeneficiaryAddress"].ToString()));
                    }

                    if (dic["Sender"] != null)
                    {
                        outgoingTransfers = outgoingTransfers
                            .Where(x => x.Sender != null && x.Sender.Name.Contains(dic["Sender"].ToString()));
                    }
                }

                return outgoingTransfers.ToList();
            }

            return new List<OutgoingTransfer>();
        }

        IQueryable<OutgoingTransfer> GetAllWithDetails()
        {
            var outgoingTransfers = _outgoingTransferRepository
                .GetAllIncluding(
                tc => tc.ToCompany,
                ttc => ttc.FromCompany,
                fc => fc.FromClient,
                fb=>fb.FromBranch,
                tb=>tb.ToBranch,
                co => co.Country,
                cu => cu.Currency,
                be => be.Beneficiary,
                se => se.Sender
                );
            return outgoingTransfers;
        }

        private OutgoingTransfer GetByIdWithDetail(int id)
        {
            return _outgoingTransferRepository.
                GetAllIncluding(
                    tm => tm.ToCompany,
                    fc => fc.FromClient,
                    fm => fm.FromCompany,
                    s => s.Sender,
                    b => b.Beneficiary,
                    ds => ds.Country,
                    cu => cu.Currency,
                    t => t.Treasury,
                    tb => tb.ToBranch,
                    fb => fb.FromBranch)
                .FirstOrDefault(x => x.Id == id);
        }

        public async Task<OutgoingTransfer> CreateAsync(OutgoingTransfer input)
        {
            var createdOutgoingTransfer = new OutgoingTransfer();

            using (var unitOfWork = _unitOfWorkManager.Begin())
            {
                //Date and time
                var currentDate = DateTime.Now;
                input.Date = new DateTime(
                    input.Date.Year,
                    input.Date.Month,
                    input.Date.Day,
                    currentDate.Hour,
                    currentDate.Minute,
                    currentDate.Second
                    );

                if (input.ToBranchId == null)
                {
                    input.Status = OutgoingTransferStatus.Accepted;
                    var id = await _outgoingTransferRepository.InsertAndGetIdAsync(input);

                    createdOutgoingTransfer = await _outgoingTransferRepository.GetAsync(id);

                    IOutgoingTransferDomainService service = _outgoingTransferFactory.CreateService(createdOutgoingTransfer);
                    await service.CreateCashFlowAsync(createdOutgoingTransfer);
                }
                else
                {
                    input.Status = OutgoingTransferStatus.Pending;
                    var id = await _outgoingTransferRepository.InsertAndGetIdAsync(input);

                    createdOutgoingTransfer = await _outgoingTransferRepository.GetAsync(id);
                }

                unitOfWork.Complete();
            }
            return createdOutgoingTransfer;

        }

        public async Task<OutgoingTransfer> UpdateAsync(OutgoingTransfer outgoingTransfer)
        {
            var updatedTreasuryAction = new OutgoingTransfer();

            using (var unitOfWork = _unitOfWorkManager.Begin())
            {
                if (outgoingTransfer.PaymentType == PaymentType.Cash ||
                outgoingTransfer.PaymentType == PaymentType.Client ||
                outgoingTransfer.PaymentType == PaymentType.Company)
                {
                    outgoingTransfer.Status = OutgoingTransferStatus.Accepted;
                    outgoingTransfer.ToBranchId = null;
                    updatedTreasuryAction = await _outgoingTransferRepository.UpdateAsync(outgoingTransfer);

                    IOutgoingTransferDomainService service = _outgoingTransferFactory.CreateService(updatedTreasuryAction);
                    await service.CreateCashFlowAsync(updatedTreasuryAction);
                }
                else
                {   
                    outgoingTransfer.Status = OutgoingTransferStatus.Pending;
                    outgoingTransfer.FromClientId = null;
                    outgoingTransfer.FromCompanyId = null;
                    updatedTreasuryAction = await _outgoingTransferRepository.UpdateAsync(outgoingTransfer);
                }

                unitOfWork.Complete();
            }

            return updatedTreasuryAction;
        }

        public async Task<OutgoingTransfer> DeleteAsync(OutgoingTransfer outgoingTransfer)
        {
            var updatedTreasuryAction = new OutgoingTransfer();

            using (var unitOfWork = _unitOfWorkManager.Begin())
            {
                var cashFlowDeleted = await DeleteCashFlowAsync(outgoingTransfer);
                if (cashFlowDeleted)
                {
                    await _outgoingTransferRepository.DeleteAsync(outgoingTransfer);
                }
                _unitOfWorkManager.Current.SaveChanges();

                unitOfWork.Complete();
            }

            return updatedTreasuryAction;
        }

        public async Task<bool> DeleteCashFlowAsync(OutgoingTransfer outgoingTransfer)
        {
                IOutgoingTransferDomainService service = _outgoingTransferFactory.CreateService(outgoingTransfer);
                return await service.DeleteCashFlowAsync(outgoingTransfer);
             
        }

        public int GetLastNumber()
        {
            var last = _outgoingTransferRepository.GetAll().OrderByDescending(x => x.Number).FirstOrDefault();
            return last == null ? 0 : last.Number;
        }

        public IList<OutgoingTransfer> GetAllNotCompleted(int? branchId)
        {
            if (branchId != null)
            {
                var OutTransfer = GetAllWithDetails()
                            .Where(x => x.Status == OutgoingTransferStatus.Pending && x.ToBranchId == branchId)
                            .ToList();
                return OutTransfer;
            }
            else
            {
                var OutTransfer = GetAllWithDetails().Where(x => x.Status == OutgoingTransferStatus.Pending).ToList();
                return OutTransfer;
            }
            
        }

        public async Task<OutgoingTransfer> AcceptOutgoingTransferFromBranchAsync(int outgoingTransferId, int branchId)
        {
            var outgoingTransfer = GetByIdWithDetail(outgoingTransferId);

            var destinationCurrency = await _currencyRepository
                .FirstOrDefaultAsync(x => x.Name.Trim().ToLower() == outgoingTransfer.Currency.Name.ToLower().Trim());

            OutgoingTransfer updatedOutgoingTransfer = new OutgoingTransfer();


            using (var unitOfWork = _unitOfWorkManager.Begin())
            {
                outgoingTransfer.Status = OutgoingTransferStatus.Accepted;
                updatedOutgoingTransfer = await _outgoingTransferRepository.UpdateAsync(outgoingTransfer);

                IOutgoingTransferDomainService service = _outgoingTransferFactory.CreateService(outgoingTransfer);
                await service.CreateCashFlowAsync(outgoingTransfer);

                var data = new CreateIncomeTransferWhenAcceptOutgoingEventData
                    (
                        outgoingTransfer.ToCompanyId,
                        outgoingTransfer.Date.ToString(),
                        outgoingTransfer.Note,
                        null, null, 0, 0, 0,
                        outgoingTransfer.Amount,
                        outgoingTransfer.Commission,
                        outgoingTransfer.CompanyCommission, destinationCurrency.Id,
                        outgoingTransfer.BeneficiaryId,
                        outgoingTransfer.SenderId, 0, branchId,
                        outgoingTransfer.Sender,
                        outgoingTransfer.Beneficiary

                    );

                await EventBus.Default.TriggerAsync(data);

                unitOfWork.Complete();
            }

            return updatedOutgoingTransfer;

        }

        public async Task<OutgoingTransfer> RejectOutgoingTransferFromBranchAsync(int outgoingTransferId)
        {
            var outgoingTransfer = await GetByIdAsync(outgoingTransferId);
            outgoingTransfer.Status = OutgoingTransferStatus.Rejected;

            return await _outgoingTransferRepository.UpdateAsync(outgoingTransfer);
        }

        public async Task<Dictionary<string,double>> GetAllForBranch(int? branchId)
        {
            Dictionary<string,double> result = new Dictionary<string,double>();
            IList<OutgoingTransfer> transfers = new List<OutgoingTransfer>();
            IList<OutgoingTransfer> branchTransfer = new List<OutgoingTransfer>();

            transfers = await _outgoingTransferRepository.GetAllListAsync();


            if (branchId != null)
            {
                branchTransfer = transfers.Where(x=>x.FromBranchId == branchId && x.ToBranchId != null).ToList();
            }
            else
            {
                branchTransfer = transfers.Where(x => x.ToBranchId != null).ToList();
            }
            double pending = branchTransfer.Where(x => x.Status == OutgoingTransferStatus.Pending).Count();
            double accepted = branchTransfer.Where(x => x.Status == OutgoingTransferStatus.Accepted).Count();
            double rejected = branchTransfer.Where(x => x.Status == OutgoingTransferStatus.Rejected).Count();

            result.Add("Pending", pending);
            result.Add("Accepted", accepted);
            result.Add("Rejected", rejected);

            return result;
        }
    }
}
