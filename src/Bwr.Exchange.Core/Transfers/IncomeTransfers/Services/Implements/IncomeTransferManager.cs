using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Bwr.Exchange.Transfers.IncomeTransfers.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bwr.Exchange.Transfers.IncomeTransfers.Factories;
using Bwr.Exchange.CashFlows.CompanyCashFlows.Events;
using Abp.Events.Bus;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Bwr.Exchange.CashFlows.ClientCashFlows.Events;
using Bwr.Exchange.Settings.Clients.Services;
using Bwr.Exchange.Settings.Companies.Services;
using Bwr.Exchange.CashFlows.TreasuryCashFlows.Events;
using Bwr.Exchange.CashFlows.CompanyCashFlows.Services;
using Bwr.Exchange.CashFlows.CompanyCashFlows;
using Abp.Threading;
using Bwr.Exchange.CashFlows.ClientCashFlows;
using Bwr.Exchange.CashFlows.ClientCashFlows.Services;
using Bwr.Exchange.CashFlows.TreasuryCashFlows.Events.TreasuryCashFlowDeletedMulti;
using Bwr.Exchange.CashFlows.TreasuryCashFlows.Services;
using Bwr.Exchange.Transfers.OutgoingTransfers;
using System.Linq.Dynamic.Core;

namespace Bwr.Exchange.Transfers.IncomeTransfers.Services.Implements
{
    public class IncomeTransferManager : IIncomeTransferManager
    {
        private readonly IRepository<IncomeTransfer> _incomeTransferRepository;
        private readonly IIncomeTransferDetailManager _incomeTransferDetailManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IClientManager _clientManager;
        private readonly ICompanyManager _companyManager;
        private readonly ICompanyCashFlowManager _companyCashFlowManager;
        private readonly IClientCashFlowManager _clientCashFlowManager;
        private readonly ITreasuryCashFlowManager _treasuryCashFlowManager;
        private readonly IRepository<OutgoingTransfer> _outgoingTransferRepository;

        public IncomeTransferManager(IRepository<IncomeTransfer> incomeTransferRepository, IIncomeTransferDetailManager incomeTransferDetailManager, IUnitOfWorkManager unitOfWorkManager, IClientManager clientManager, ICompanyManager companyManager, ICompanyCashFlowManager companyCashFlowManager, IClientCashFlowManager clientCashFlowManager, ITreasuryCashFlowManager treasuryCashFlowManager, IRepository<OutgoingTransfer> outgoingTransferRepository)
        {
            _incomeTransferRepository = incomeTransferRepository;
            _incomeTransferDetailManager = incomeTransferDetailManager;
            _unitOfWorkManager = unitOfWorkManager;
            _clientManager = clientManager;
            _companyManager = companyManager;
            _companyCashFlowManager = companyCashFlowManager;
            _clientCashFlowManager = clientCashFlowManager;
            _treasuryCashFlowManager = treasuryCashFlowManager;
            _outgoingTransferRepository = outgoingTransferRepository;
        }

        public async Task<IncomeTransfer> CreateAsync(IncomeTransfer input)
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

            IncomeTransfer createdIncomeTransfer;
            using (var unitOfWork = _unitOfWorkManager.Begin())
            {
                var id = await _incomeTransferRepository.InsertAndGetIdAsync(input);
                createdIncomeTransfer = await _incomeTransferRepository.GetAsync(id);

                await CreateCashFlowAsync(createdIncomeTransfer);

                unitOfWork.Complete();
            }

            return createdIncomeTransfer;
        }

        public async Task<IncomeTransfer> UpdateAsync(IncomeTransfer input)
        {
            IncomeTransfer updatedIncomeTransfer;
            using (var unitOfWork = _unitOfWorkManager.Begin())
            {

                updatedIncomeTransfer = await _incomeTransferRepository.UpdateAsync(input);
                await _unitOfWorkManager.Current.SaveChangesAsync();
                await _incomeTransferRepository.EnsureCollectionLoadedAsync(updatedIncomeTransfer, x => x.IncomeTransferDetails);
                await CreateCashFlowAsync(updatedIncomeTransfer);
                unitOfWork.Complete();
            }
            return updatedIncomeTransfer;
        }

        private async Task<string> GetBenificiaryName(IncomeTransferDetail detail)
        {
            if (detail.PaymentType == PaymentType.Client)
            {
                var client = await _clientManager.GetByIdAsync(detail.ToClientId.Value);
                return client.Name;
            }

            return detail.Beneficiary?.Name;
        }

        public Task<IList<IncomeTransfer>> GetAsync(Dictionary<string, object> dic)
        {
            throw new NotImplementedException();
        }

        public IList<IncomeTransfer> Get(Dictionary<string, object> dic)
        {
            throw new NotImplementedException();
        }

        public IncomeTransfer GetById(int id)
        {
            var incomeTransfer = _incomeTransferRepository.FirstOrDefault(x => x.Id == id);
            _incomeTransferRepository.EnsureCollectionLoaded(incomeTransfer, x => x.IncomeTransferDetails);
            return incomeTransfer;
        }

        public async Task<IncomeTransfer> GetByIdAsync(int id)
        {
            var incomeTransfer = await _incomeTransferRepository.FirstOrDefaultAsync(x => x.Id == id);
            await _incomeTransferRepository.EnsureCollectionLoadedAsync(incomeTransfer, x => x.IncomeTransferDetails);
            return incomeTransfer;
        }

        public IQueryable<IncomeTransfer> Get(DateTime fromDate, DateTime toDate, int? companyId, int? number)
        {
            if (number == null && companyId != null)
            {
                return _incomeTransferRepository.GetAllIncluding(c => c.Company, i => i.IncomeTransferDetails)
                .Where(x => x.Date >= fromDate && x.Date <= toDate && x.CompanyId == companyId);
            }
            else if (number == null && companyId == null)
            {
                return _incomeTransferRepository.GetAllIncluding(c => c.Company, i => i.IncomeTransferDetails)
               .Where(x => x.Date >= fromDate && x.Date <= toDate);
            }
            else if (number != null)
            {
                return _incomeTransferRepository.GetAllIncluding(c => c.Company, i => i.IncomeTransferDetails)
                .Where(x => x.Number == number);
            }
            else
            {
                return _incomeTransferRepository.GetAllIncluding(c => c.Company, i => i.IncomeTransferDetails);
            }
        }

        public async Task<bool> DeleteDetailsAsync(IncomeTransfer incomeTransfer)
        {
            try
            {
                using (var unitOfWork = _unitOfWorkManager.Begin())
                {
                    //_companyCashFlowManager.Delete(incomeTransfer.Id, CashFlows.TransactionType.IncomeTransfer);
                    await _companyCashFlowManager.DeleteAsync(new CompanyCashFlowDeletedMultiEventData(incomeTransfer.Id, CashFlows.TransactionType.IncomeTransfer, TransactionConst.TransferFromHim)) ;
                    
                    foreach (var detail in incomeTransfer.IncomeTransferDetails)
                    {
                        if (detail.PaymentType == PaymentType.Cash)
                        {
                            await EventBus.Default.TriggerAsync(new TreasuryCashFlowDeletedEventData(incomeTransfer.Id, CashFlows.TransactionType.IncomeTransfer));
                            
                        }
                        else if (detail.PaymentType == PaymentType.Company)
                        {
                            await _companyCashFlowManager.DeleteAsync(new CompanyCashFlowDeletedMultiEventData(incomeTransfer.Id, CashFlows.TransactionType.IncomeTransfer, TransactionConst.TransferToHim));
                        }
                        else if (detail.PaymentType == PaymentType.Client)
                        {
                            await _clientCashFlowManager.DeleteAsync(new ClientCashFlowDeletedMultiEventData(incomeTransfer.Id, CashFlows.TransactionType.IncomeTransfer, TransactionConst.TransferToHim));
                            
                        }

                        //await _incomeTransferDetailManager.DeteleAsync(detail);
                    }
                    await _unitOfWorkManager.Current.SaveChangesAsync();
                    await unitOfWork.CompleteAsync();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task CreateCashFlowAsync(IncomeTransfer createdIncomeTransfer)
        {
            var date = createdIncomeTransfer.Date;

            var createdCompanyCashFlows = new List<CompanyCashFlow>();
            var createdToCompanyCashFlows = new List<CompanyCashFlow>();
            var createdToClientCashFlows = new List<ClientCashFlow>();

            var incomeTransferDetails = createdIncomeTransfer.IncomeTransferDetails.OrderBy(x => x.Index).ToList();
            
            foreach (var detail in incomeTransferDetails)
            {
                date = date.AddMilliseconds(2);
                var beneficiaryName = await GetBenificiaryName(detail);
                //var previousCompanyCashFlow = createdCompanyCashFlows
                //    .Where(x => x.CompanyId == createdIncomeTransfer.CompanyId && x.CurrencyId == detail.CurrencyId)
                //    .LastOrDefault();

                var createdCompanyCashFlow = await _companyCashFlowManager.CreateAsync(
                new CompanyCashFlowCreatedEventData()
                {
                    Date = date,
                    CurrencyId = detail.CurrencyId,
                    CompanyId = createdIncomeTransfer.CompanyId,
                    TransactionId = createdIncomeTransfer.Id,
                    TransactionType = CashFlows.TransactionType.IncomeTransfer,
                    Amount = (detail.Amount),
                    Type = TransactionConst.TransferFromHim,
                    Commission = detail.Commission,
                    //PreviousBalance = previousCompanyCashFlow != null ? previousCompanyCashFlow.CurrentBalance : 0,
                    CompanyCommission = 0.0,
                    Note = createdIncomeTransfer.Note,
                    Beneficiary = beneficiaryName,
                    Sender = detail.Sender.Name,
                    Destination = null
                });
                createdCompanyCashFlows.Add(createdCompanyCashFlow);


                if (detail.PaymentType == PaymentType.Client)
                {
                    //var previousClientCashFlow = createdToClientCashFlows
                    //.Where(x => x.ClientId == detail.ToClientId && x.CurrencyId == detail.CurrencyId)
                    //.LastOrDefault();

                    var createdToClientCashFlow = await _clientCashFlowManager.CreateAsync(
                    new ClientCashFlowCreatedEventData()
                    {
                        Date = date,
                        CurrencyId = detail.CurrencyId,
                        ClientId = detail.ToClientId,
                        TransactionId = createdIncomeTransfer.Id,
                        TransactionType = CashFlows.TransactionType.IncomeTransfer,
                        Amount = (detail.Amount * -1),
                        Type = TransactionConst.TransferToHim,
                        Commission = (detail.ClientCommission * -1),
                        //PreviousBalance = previousClientCashFlow != null ? previousClientCashFlow.CurrentBalance : 0,
                        Note = detail.Sender.Name,
                        Sender = detail.Sender.Name,
                        //Beneficiary=detail.Beneficiary.Name
                    });

                    createdToClientCashFlows.Add(createdToClientCashFlow);
                }
                else if (detail.PaymentType == PaymentType.Company)
                {
                    //var previousToCompanyCashFlow = createdToCompanyCashFlows
                    //.Where(x => x.CompanyId == detail.ToCompanyId && x.CurrencyId == detail.CurrencyId)
                    //.LastOrDefault();

                    var createdToCompanyCashFlow = await _companyCashFlowManager.CreateAsync(
                    new CompanyCashFlowCreatedEventData()
                    {
                        Date = date,
                        CurrencyId = detail.CurrencyId,
                        CompanyId = detail.ToCompanyId,
                        TransactionId = createdIncomeTransfer.Id,
                        TransactionType = CashFlows.TransactionType.IncomeTransfer,
                        Amount = (detail.Amount * -1),
                        Type = TransactionConst.TransferToHim,
                        Note = createdIncomeTransfer.Note,
                        //PreviousBalance = previousToCompanyCashFlow != null ? previousToCompanyCashFlow.CurrentBalance : 0,
                        Beneficiary = beneficiaryName,
                        Commission = (detail.CompanyCommission * -1),
                        Sender = detail.Sender.Name
                    });

                    createdToCompanyCashFlows.Add(createdToCompanyCashFlow);
                }
            }

            //EventBus.Trigger(multiEventData);
        }

        public int GetLastNumber(int branchId)
        {
            IList<IncomeTransfer> incomeTransfers = new List<IncomeTransfer>();

            var branchCompanies = AsyncHelper.RunSync(() => _companyManager.GetAllForCurrentBranchAsync(branchId));

            var allIncomeTransfers = _incomeTransferRepository.GetAll();
            foreach (var incomeTransfer in allIncomeTransfers)
            {
                if (branchCompanies.Contains(incomeTransfer.Company))
                {
                    incomeTransfers.Add(incomeTransfer);
                }
            }
            var last = incomeTransfers.OrderByDescending(x => x.Number).FirstOrDefault();
            return last == null ? 0 : last.Number;
        }

        public async Task<bool> DeleteAsync(IncomeTransfer incomeTransfer)
        {
            try
            {
                using (var unitOfWork = _unitOfWorkManager.Begin())
                {
                    //_companyCashFlowManager.Delete(incomeTransfer.Id, CashFlows.TransactionType.IncomeTransfer);
                    await _companyCashFlowManager.DeleteAsync(new CompanyCashFlowDeletedMultiEventData(incomeTransfer.Id, CashFlows.TransactionType.IncomeTransfer, TransactionConst.TransferFromHim));


                    foreach (var detail in incomeTransfer.IncomeTransferDetails)
                    {
                        if (detail.PaymentType == PaymentType.Cash)
                        {
                            await _treasuryCashFlowManager.DeleteAsync(new TreasuryCashFlowDeletedMultiEventData(incomeTransfer.Id, CashFlows.TransactionType.IncomeTransfer, detail.CurrencyId, TransactionConst.DirectTransfer));
                           
                        }
                        else if (detail.PaymentType == PaymentType.Company)
                        {
                            await _companyCashFlowManager.DeleteAsync(new CompanyCashFlowDeletedMultiEventData(incomeTransfer.Id, CashFlows.TransactionType.IncomeTransfer, TransactionConst.TransferToHim));
                            
                        }
                        else if (detail.PaymentType == PaymentType.Client)
                        {
                            await _clientCashFlowManager.DeleteAsync(new ClientCashFlowDeletedMultiEventData(incomeTransfer.Id, CashFlows.TransactionType.IncomeTransfer, TransactionConst.TransferToHim));

                        }

                        await _incomeTransferDetailManager.DeteleAsync(detail);
                    }

                    await _incomeTransferRepository.DeleteAsync(incomeTransfer);

                    await _unitOfWorkManager.Current.SaveChangesAsync();
                    await unitOfWork.CompleteAsync();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<Dictionary<string, double>> GetAllForBranch(int? branchId)
        {
            Dictionary<string, double> result = new Dictionary<string, double>();
            IList<OutgoingTransfer> transfers = new List<OutgoingTransfer>();
            IList<OutgoingTransfer> branchTransfer = new List<OutgoingTransfer>();

            transfers = await _outgoingTransferRepository.GetAllListAsync();


            if (branchId != null)
            {
                branchTransfer = transfers.Where(x=>x.ToBranchId == branchId).ToList();
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
