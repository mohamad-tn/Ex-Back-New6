using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Events.Bus.Handlers;
using Bwr.Exchange.Settings.Clients.Services;
using Bwr.Exchange.Settings.Companies.Services;
using Bwr.Exchange.Transfers.IncomeTransfers;
using Bwr.Exchange.Transfers.IncomeTransfers.Services.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Bwr.Exchange.Transfers.OutgoingTransfers.Events
{
    public class CreateIncomeTransferWhenAcceptOutgoingEventHandlerr : IAsyncEventHandler<CreateIncomeTransferWhenAcceptOutgoingEventData>, ITransientDependency
    {
        private readonly IIncomeTransferManager _incomeTransferManager;
        private readonly IRepository<IncomeTransfer> _incomeTransferRepository;
        private readonly IIncomeTransferDetailManager _incomeTransferDetailManager;
        private readonly ICompanyManager _combanyManager;
        private readonly IClientManager _clientManager;

        public CreateIncomeTransferWhenAcceptOutgoingEventHandlerr(IIncomeTransferManager incomeTransferManager,
            IIncomeTransferDetailManager incomeTransferDetailManager,
            ICompanyManager combanyManager,
            IClientManager clientManager,
            IRepository<IncomeTransfer> incomeTransferRepository)
        {
            _incomeTransferManager = incomeTransferManager;
            _incomeTransferDetailManager = incomeTransferDetailManager;
            _combanyManager = combanyManager;
            _clientManager = clientManager;
            _incomeTransferRepository = incomeTransferRepository;
        }

        public async Task HandleEventAsync(CreateIncomeTransferWhenAcceptOutgoingEventData eventData)
        {
            var companies = await _combanyManager.GetAllForCurrentBranchAsync((int)eventData.branchId);
            var clients = await _clientManager.GetAllForCurrentBranchAsync((int)eventData.branchId);

            DateTime date;
            var NewDate = DateTime.TryParse(eventData.Date, out date);

            IncomeTransfer incomeTransfer = new IncomeTransfer()
            {
                CompanyId = eventData.CompanyId,
                Date = date,
                Note = eventData.Note
            };

            IncomeTransferDetail incomeDetail = new IncomeTransferDetail()
            {
                Amount = eventData.Amount,
                CurrencyId = eventData.CurrencyId,
                Commission = eventData.Commission,
                SenderId = eventData.SenderId,
            };

            if (companies.Any(x => x.Name.Trim().ToLower().Replace(" ", "").Replace("أ", "ا") == eventData.Beneficiary.Name.Trim().ToLower().Replace(" ", "").Replace("أ", "ا")))
            {                
                var company = companies
                    .Where(x => x.Name.Trim().ToLower().Replace(" ","").Replace("أ","ا") ==
                    eventData.Beneficiary.Name.Trim().ToLower().Replace(" ", "").Replace("أ", "ا"))
                    .FirstOrDefault();

                incomeDetail.PaymentType = PaymentType.Company;
                incomeDetail.ToCompanyId = company.Id;
                incomeDetail.CompanyCommission = eventData.CompanyCommission;
            }
            else if (clients.Any(x => x.Name.Trim().ToLower().Replace(" ", "").Replace("أ", "ا") == eventData.Beneficiary.Name.Trim().ToLower().Replace(" ", "").Replace("أ", "ا")))
            {
                var client = clients
                    .Where(x => x.Name.Trim().ToLower().Replace(" ", "").Replace("أ", "ا") ==
                    eventData.Beneficiary.Name.Trim().ToLower().Replace(" ", "").Replace("أ", "ا"))
                    .FirstOrDefault();

                incomeDetail.PaymentType = PaymentType.Client;
                incomeDetail.ToClientId = client.Id;
                incomeDetail.ClientCommission = eventData.ClientCommission;
            }
            else
            {
                incomeDetail.PaymentType = PaymentType.Cash;
                incomeDetail.BeneficiaryId = eventData.BeneficiaryId;
            }

            incomeTransfer.AddIncomeTransferDetail(incomeDetail);

            var insertedIncomeTransfer = await _incomeTransferManager.CreateAsync(incomeTransfer);

        }
    }
}
