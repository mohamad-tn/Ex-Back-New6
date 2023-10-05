using Abp.Threading;
using Abp.UI;
using Bwr.Exchange.Settings.Clients;
using Bwr.Exchange.Settings.Expenses.Dto;
using Bwr.Exchange.Settings.Expenses.Services;
using Bwr.Exchange.Shared.DataManagerRequests;
using Bwr.Exchange.Shared.Dto;
using Bwr.Exchange.Shared.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Syncfusion.EJ2.Base;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bwr.Exchange.Settings.Expenses
{
    public class ExpenseAppService : ExchangeAppServiceBase, IExpenseAppService
    {
        private readonly IExpenseManager _expenseManager;
        public ExpenseAppService(ExpenseManager expenseManager)
        {
            _expenseManager = expenseManager;
        }

        public async Task<IList<ExpenseDto>> GetAllAsync()
        {
            IList<Expense> expenses = new List<Expense>();
            var user = await GetCurrentUserAsync();
            if (user.BranchId != null)
            {
                expenses = await _expenseManager.GetAllForCurrentBranchAsync((int)user.BranchId);
            }
            else
            {
                expenses = await _expenseManager.GetAllAsync();
            }                

            return ObjectMapper.Map<List<ExpenseDto>>(expenses);
        }
        [HttpPost]
        public ReadGrudDto GetForGrid([FromBody] BWireDataManagerRequest dm)
        {
            IList<Expense> data = new List<Expense>();
            var user = UserManager.GetUserById(dm.userId);
            if (user.BranchId == null)
            {
                data = _expenseManager.GetAll();
            }
            else
            {
                data = _expenseManager.GetAll().Where(x => x.BranchId == user.BranchId).ToList();
            }
               
            IEnumerable<ReadExpenseDto> expenses = ObjectMapper.Map<List<ReadExpenseDto>>(data);

            var operations = new DataOperations();
            if (dm.Where != null && dm.Where.Count > 0)
            {
                expenses = operations.PerformFiltering(expenses, dm.Where, "and");
            }

            if (dm.Sorted != null && dm.Sorted.Count > 0)
            {
                expenses = operations.PerformSorting(expenses, dm.Sorted);
            }

            IEnumerable groupDs = new List<ReadExpenseDto>();
            if (dm.Group != null)
            {
                groupDs = operations.PerformSelect(expenses, dm.Group);
            }

            var count = expenses.Count();

            if (dm.Skip != 0)
            {
                expenses = operations.PerformSkip(expenses, dm.Skip);
            }

            if (dm.Take != 0)
            {
                expenses = operations.PerformTake(expenses, dm.Take);
            }

            return new ReadGrudDto() { result = expenses, count = count, groupDs = groupDs };
        }
        public UpdateExpenseDto GetForEdit(int id)
        {
            var expense = AsyncHelper.RunSync(() => _expenseManager.GetByIdAsync(id));
            return ObjectMapper.Map<UpdateExpenseDto>(expense);
        }
        public async Task<ExpenseDto> CreateAsync(CreateExpenseDto input)
        {
            var currentBranch = await GetCurrentBranch();
            input.BranchId = currentBranch.Id;

            CheckBeforeCreate(input);

            var expense = ObjectMapper.Map<Expense>(input);

            var createdExpense = await _expenseManager.InsertAndGetAsync(expense);

            return ObjectMapper.Map<ExpenseDto>(createdExpense);
        }
        public async Task<ExpenseDto> UpdateAsync(UpdateExpenseDto input)
        {
            var expense = await _expenseManager.GetByIdAsync(input.Id);

            CheckBeforeUpdate(input, (int)expense.BranchId);

            ObjectMapper.Map<UpdateExpenseDto, Expense>(input, expense);

            var updatedExpense = await _expenseManager.UpdateAndGetAsync(expense);

            return ObjectMapper.Map<ExpenseDto>(updatedExpense);
        }
        public async Task DeleteAsync(int id)
        {
            await _expenseManager.DeleteAsync(id);
        }

        #region Helper methods
        private void CheckBeforeCreate(CreateExpenseDto input)
        {
            var validationResultMessage = string.Empty;

            if (_expenseManager.CheckIfNameAlreadyExist(input.Name, (int)input.BranchId))
            {
                validationResultMessage = L(ValidationResultMessage.NameAleadyExist);
            }

            if (!string.IsNullOrEmpty(validationResultMessage))
                throw new UserFriendlyException(validationResultMessage);
        }
        private void CheckBeforeUpdate(UpdateExpenseDto input, int branchId)
        {
            var validationResultMessage = string.Empty;

            if (_expenseManager.CheckIfNameAlreadyExist(input.Id, input.Name,branchId))
            {
                validationResultMessage = L(ValidationResultMessage.NameAleadyExist);
            }

            if (!string.IsNullOrEmpty(validationResultMessage))
                throw new UserFriendlyException(validationResultMessage);
        }

        #endregion
    }
}
