using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Events.Bus;
using Bwr.Exchange.CashFlows.ClientCashFlows.Events;
using Bwr.Exchange.CashFlows.CompanyCashFlows.Events;
using Bwr.Exchange.CashFlows.TreasuryCashFlows.Events;
using Bwr.Exchange.TreasuryActions.Factories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bwr.Exchange.TreasuryActions.Services
{
    public class TreasuryActionManager : ITreasuryActionManager
    {
        private readonly ITreasuryActionFactory _treasuryActionFactory;
        private readonly IRepository<TreasuryAction> _treasuryActionRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        public TreasuryActionManager(
            ITreasuryActionFactory treasuryActionFactory,
            IRepository<TreasuryAction> treasuryActionRepository,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _treasuryActionFactory = treasuryActionFactory;
            _treasuryActionRepository = treasuryActionRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public async Task<TreasuryAction> CreateAsync(TreasuryAction input)
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

            ITreasuryActionDomainService service = _treasuryActionFactory.CreateService(input);
            return await service.CreateTreasuryActionAsync();
        }

        public async Task<IList<TreasuryAction>> GetAsync(Dictionary<string, object> dic)
        {
            var fromDate = DateTime.Parse(dic["FromDate"].ToString());
            var toDate = DateTime.Parse(dic["ToDate"].ToString());
            IEnumerable<TreasuryAction> treasuryActions = await _treasuryActionRepository.GetAllListAsync(x =>
              x.Date >= fromDate &&
              x.Date <= toDate);

            if (treasuryActions != null && treasuryActions.Any())
            {
                if (dic["CurrencyId"] != null)
                {
                    treasuryActions = treasuryActions
                        .Where(x => x.CurrencyId == int.Parse(dic["CurrencyId"].ToString()));
                }

                if (dic["ActionType"] != null)
                {
                    treasuryActions = treasuryActions
                        .Where(x => x.ActionType == (TreasuryActionType)int.Parse(dic["ActionType"].ToString()));
                }

            }

            return treasuryActions.ToList();
        }

        public async Task DeleteAsync(TreasuryAction input)
        {
            using (var unitOfWork = _unitOfWorkManager.Begin())
            {
                await _treasuryActionRepository.DeleteAsync(input);
                var cashFlowDeleted = await DeleteCashFlowAsync(input);
                if (cashFlowDeleted)
                {
                    await _treasuryActionRepository.DeleteAsync(input.Id);
                }
                await _unitOfWorkManager.Current.SaveChangesAsync();

                await unitOfWork.CompleteAsync();
            }
        }

        public async Task<bool> DeleteCashFlowAsync(TreasuryAction input)
        {
            try
            {
                //await _treasuryActionRepository.DeleteAsync(input.Id);
                await EventBus.Default.TriggerAsync(new ClientCashFlowDeletedEventData(input.Id, CashFlows.TransactionType.TreasuryAction));
                await EventBus.Default.TriggerAsync(new CompanyCashFlowDeletedEventData(input.Id, CashFlows.TransactionType.TreasuryAction));
                await EventBus.Default.TriggerAsync(new TreasuryCashFlowDeletedEventData(input.Id, CashFlows.TransactionType.TreasuryAction));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<TreasuryAction> GetByIdAsync(int id)
        {
            return await _treasuryActionRepository.GetAsync(id);
        }

        public async Task<TreasuryAction> UpdateAsync(TreasuryAction treasuryAction)
        {
            ITreasuryActionDomainService service = _treasuryActionFactory.CreateService(treasuryAction);
            return await service.UpdateTreasuryActionAsync();
        }

        public IList<TreasuryAction> GetForStatment(Dictionary<string, object> dic)
        {
            var actionType = (TreasuryActionType)(int.Parse(dic["ActionType"].ToString()));
            var mainAccount = int.Parse(dic["MainAccount"].ToString());
            var fromDate = DateTime.Parse(dic["FromDate"].ToString());
            var toDate = DateTime.Parse(dic["ToDate"].ToString());

            IEnumerable<TreasuryAction> treasuryActions = _treasuryActionRepository
                    .GetAllIncluding(
                    x => x.Currency,
                    co => co.MainAccountClient,
                    cm => cm.MainAccountCompany,
                    ec => ec.ExchangePartyClient,
                    eo => eo.ExchangePartyCompany,
                    e => e.Expense,
                    i => i.Income
                    ).Include(x=>x.IncomeTransferDetail)
                    .ThenInclude(x=>x.Beneficiary)
                    .Where(x =>
                    x.ActionType == actionType &&
                  x.Date >= fromDate &&
                  x.Date <= toDate);

            if (treasuryActions != null && treasuryActions.Any())
            {
                if (mainAccount == 0)
                {
                    var mainAccountClientId = dic["MainAccountClientId"] != null ? 
                        int.Parse(dic["MainAccountClientId"].ToString()) : 0;

                    if (mainAccountClientId == -1)
                    {
                        treasuryActions = treasuryActions
                        .Where(x => x.MainAccountClientId != null);
                    }
                    else
                    {
                        treasuryActions = treasuryActions
                        .Where(x => x.MainAccountClientId == mainAccountClientId);
                    }

                }
                else if (mainAccount == 1)
                {
                    var mainAccountCompanyId = dic["MainAccountCompanyId"] != null ? 
                        int.Parse(dic["MainAccountCompanyId"].ToString()) : 0;
                    if (mainAccountCompanyId == -1)
                    {
                        treasuryActions = treasuryActions
                        .Where(x => x.MainAccountCompanyId != null);
                    }
                    else
                    {
                        treasuryActions = treasuryActions
                        .Where(x => x.MainAccountCompanyId == mainAccountCompanyId);
                    }
                }
                else if (mainAccount == 2)
                {
                    var incomeId = dic["IncomeId"] != null ? int.Parse(dic["IncomeId"].ToString()) : 0;
                    if (incomeId == -1)
                    {
                        treasuryActions = treasuryActions
                        .Where(x => x.IncomeId != null);
                    }
                    else
                    {
                        treasuryActions = treasuryActions
                        .Where(x => x.IncomeId == incomeId);
                    }
                }
                else if (mainAccount == 3)
                {
                    var expenseId = dic["ExpenseId"] != null ? int.Parse(dic["ExpenseId"].ToString()) : 0;
                    if (expenseId == -1)
                    {
                        treasuryActions = treasuryActions
                        .Where(x => x.ExpenseId != null);
                    }
                    else
                    {
                        treasuryActions = treasuryActions
                        .Where(x => x.ExpenseId == expenseId);
                    }
                }
                else if (mainAccount == 4)
                {
                    var beneficiaryId = dic["BeneficiaryId"] != null ? int.Parse(dic["BeneficiaryId"].ToString()) : 0;
                    if (beneficiaryId == -1)
                    {
                        treasuryActions = treasuryActions
                        .Where(x => x.IncomeTransferDetailId != null);
                    }
                    else
                    {
                        treasuryActions = treasuryActions
                        .Where(x => x.IncomeTransferDetailId != null && x.IncomeTransferDetail.BeneficiaryId == beneficiaryId);
                    }
                }
            }

            return treasuryActions.ToList();
        }

        public IList<TreasuryAction> Get(Dictionary<string, object> dic)
        {
            if (dic["Number"] == null)
            {
                var fromDate = DateTime.Parse(dic["FromDate"].ToString());
                var toDate = DateTime.Parse(dic["ToDate"].ToString());
                IEnumerable<TreasuryAction> treasuryActions = _treasuryActionRepository
                    .GetAllIncluding(
                    x => x.Currency,
                    co => co.MainAccountClient,
                    cm => cm.MainAccountCompany,
                    ec => ec.ExchangePartyClient,
                    eo => eo.ExchangePartyCompany,
                    e => e.Expense,
                    i => i.Income
                    ).Where(x =>
                  x.Date >= fromDate &&
                  x.Date <= toDate);

                if (treasuryActions != null && treasuryActions.Any())
                {
                    if (dic["CurrencyId"] != null)
                    {
                        treasuryActions = treasuryActions
                            .Where(x => x.CurrencyId == int.Parse(dic["CurrencyId"].ToString()));
                    }

                    if (dic["ActionType"] != null)
                    {
                        treasuryActions = treasuryActions
                            .Where(x => x.ActionType == (TreasuryActionType)int.Parse(dic["ActionType"].ToString()));
                    }

                }

                return treasuryActions.ToList();
            }
            else
            {
                IEnumerable<TreasuryAction> treasuryActions = _treasuryActionRepository
                    .GetAllIncluding(
                    x => x.Currency,
                    co => co.MainAccountClient,
                    cm => cm.MainAccountCompany,
                    ec => ec.ExchangePartyClient,
                    eo => eo.ExchangePartyCompany,
                    e => e.Expense,
                    i => i.Income
                    ).Where(x =>
                  x.Number == int.Parse(dic["Number"].ToString()));

                return treasuryActions.ToList();
            }
        }

        public int GetLastNumber()
        {
            var last = _treasuryActionRepository.GetAll().OrderByDescending(x => x.Number).FirstOrDefault();
            return last == null ? 0 : last.Number;
        }

        public IList<TreasuryAction> GetByIncomeDetailsIds(List<int> ids)
        {

            return _treasuryActionRepository.GetAllList(x =>
            x.IncomeTransferDetailId != null &&
            ids.Contains(x.IncomeTransferDetailId.Value));
        }
    }
}
