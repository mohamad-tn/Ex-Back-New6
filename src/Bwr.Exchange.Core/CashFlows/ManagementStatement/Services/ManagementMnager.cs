using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bwr.Exchange.CashFlows.ManagementStatement.Services
{
    public class ManagementMnager : IManagementMnager
    {
        private readonly IRepository<Management> _managementRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public ManagementMnager(IRepository<Management> managementRepository, IUnitOfWorkManager unitOfWorkManager)
        {
            _managementRepository = managementRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public async Task<Management> CreateAsync(Management input)
        {
            var createdManagement = new Management();
            using (var unitOfWork = _unitOfWorkManager.Begin())
            {
                //Date and time
                var currentDate = DateTime.Now;
                input.ChangeDate = new DateTime(
                    input.ChangeDate.Year,
                    input.ChangeDate.Month,
                    input.ChangeDate.Day,
                    currentDate.Hour,
                    currentDate.Minute,
                    currentDate.Second
                    );

                var id = await _managementRepository.InsertAndGetIdAsync(input);

                createdManagement = await _managementRepository.GetAsync(id);

                unitOfWork.Complete();
            }
            return createdManagement;
        }

        public IList<Management> Get(Dictionary<string, object> dic, int type)
        {
            IEnumerable<Management> managements = _managementRepository.GetAllIncluding(
                c=>c.Currency,cl=>cl.Client,s=>s.Sender,b=>b.Beneficiary,fc=>fc.FirstCurrency,
                sc=>sc.SecondCurrency,co=>co.Company,tc=>tc.ToCompany);

            if (managements != null && managements.Any())
            {
                if (dic["FromDate"] != null && dic["FromDate"] != null)
                {
                    managements = managements.Where(x =>
                          x.ChangeDate >= DateTime.Parse(dic["FromDate"].ToString()) &&
                          x.ChangeDate <= DateTime.Parse(dic["ToDate"].ToString())).ToList();
                }

                if (type != null)
                {
                    managements = managements.Where(x => ((int)x.Type) == type);
                }

                return managements.ToList();
            }
            return new List<Management>();
        }

        public async Task<Dictionary<int, double>> getChangesCount()
        {
            Dictionary<int, double> changes = new Dictionary<int, double>();
            double OutgoingTransfersCount = 0;
            double IncomingTransfersCount = 0;
            double TreasuryCount = 0;
            double ExchangeCount = 0;

            changes.Add(0, OutgoingTransfersCount);
            changes.Add(1, IncomingTransfersCount);
            changes.Add(2, TreasuryCount);
            changes.Add(3, ExchangeCount);

            var allChanges = await _managementRepository.GetAllListAsync();

            foreach (var change in allChanges)
            {
                if (change.Type == ManagementItemType.OutgoingTransfer)
                {
                    if (changes.ContainsKey(0))
                    {
                        changes[0] = changes[0] + 1;
                        //OutgoingTransfersCount = changes[0] + 1;
                    }
                }

               else if (change.Type == ManagementItemType.IncomingTransfer)
                {
                    if (changes.ContainsKey(1))
                    {
                        changes[1] = changes[1] + 1;
                        //IncomingTransfersCount = changes[1] + 1;
                    }
                }

               else if (change.Type == ManagementItemType.Treasury)
                {
                    if (changes.ContainsKey(2))
                    {
                        changes[2] = changes[2] + 1;
                        //TreasuryCount = changes[2] + 1;
                    }
                }

               else if (change.Type == ManagementItemType.Exchange)
                {
                    if (changes.ContainsKey(3))
                    {
                        changes[3] = changes[3] + 1;
                        //ExchangeCount = changes[3] + 1;
                    }
                }
            }

            return changes;
        }
    }
}
