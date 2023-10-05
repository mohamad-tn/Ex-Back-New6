using Abp.Threading;
using Abp.UI;
using Bwr.Exchange.Settings.Clients;
using Bwr.Exchange.Settings.Currencies.Dto;
using Bwr.Exchange.Settings.Currencies.Services;
using Bwr.Exchange.Shared.DataManagerRequests;
using Bwr.Exchange.Shared.Dto;
using Bwr.Exchange.Shared.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Syncfusion.EJ2.Base;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bwr.Exchange.Settings.Currencies
{
    public class CurrencyAppService : ExchangeAppServiceBase, ICurrencyAppService
    {
        private readonly ICurrencyManager _currencyManager;
        public CurrencyAppService(CurrencyManager currencyManager)
        {
            _currencyManager = currencyManager;
        }

        public async Task<IList<CurrencyDto>> GetAllAsync()
        {
            IList<Currency> currencies = new List<Currency>();
            var user = await GetCurrentUserAsync();
            if (user.BranchId != null)
            {
                currencies = await _currencyManager.GetAllForCurrentBranchAsync((int)user.BranchId);
            }
            else
            {
                currencies = await _currencyManager.GetAllAsync();
            }

            return ObjectMapper.Map<List<CurrencyDto>>(currencies);
        }

        [HttpPost]
        public ReadGrudDto GetForGrid([FromBody] BWireDataManagerRequest dm)
        {
            IList<Currency> data = new List<Currency>();
            var user = UserManager.GetUserById(dm.userId);
            if (user.BranchId == null)
            {
                data = _currencyManager.GetAll();
            }
            else
            {
                data = _currencyManager.GetAll().Where(x => x.BranchId == user.BranchId).ToList();
            }
                
            IEnumerable<ReadCurrencyDto> countries = ObjectMapper.Map<List<ReadCurrencyDto>>(data);

            var operations = new DataOperations();
            if (dm.Where != null && dm.Where.Count > 0)
            {
                countries = operations.PerformFiltering(countries, dm.Where, "and");
            }

            if (dm.Sorted != null && dm.Sorted.Count > 0)
            {
                countries = operations.PerformSorting(countries, dm.Sorted);
            }

            IEnumerable groupDs = new List<ReadCurrencyDto>();
            if (dm.Group != null)
            {
                groupDs = operations.PerformSelect(countries, dm.Group);
            }

            var count = countries.Count();

            if (dm.Skip != 0)
            {
                countries = operations.PerformSkip(countries, dm.Skip);
            }

            if (dm.Take != 0)
            {
                countries = operations.PerformTake(countries, dm.Take);
            }

            return new ReadGrudDto() { result = countries, count = count, groupDs = groupDs };
        }
        public UpdateCurrencyDto GetForEdit(int id)
        {
            var currency = AsyncHelper.RunSync(() => _currencyManager.GetByIdAsync(id));
            return ObjectMapper.Map<UpdateCurrencyDto>(currency);
        }
        public async Task<CurrencyDto> CreateAsync(CreateCurrencyDto input)
        {
            var treasury = await GetTreasuryByBranchIdAsync();
            //var currentBranch = await GetCurrentBranch();
            input.BranchId = treasury.BranchId;

            CheckBeforeCreate(input);

            var currency = ObjectMapper.Map<Currency>(input);

            var createdCurrency = await _currencyManager.InsertAndGetAsync(currency,treasury.Id,treasury.BranchId);

            return ObjectMapper.Map<CurrencyDto>(createdCurrency);
        }
        public async Task<CurrencyDto> UpdateAsync(UpdateCurrencyDto input)
        {
            var currency = await _currencyManager.GetByIdAsync(input.Id);

            CheckBeforeUpdate(input, (int)currency.BranchId);

            ObjectMapper.Map<UpdateCurrencyDto, Currency>(input, currency);

            var updatedCurrency = await _currencyManager.UpdateAndGetAsync(currency);

            return ObjectMapper.Map<CurrencyDto>(updatedCurrency);
        }
        public async Task DeleteAsync(int id)
        {
            await _currencyManager.DeleteAsync(id);
        }

        #region Helper methods
        private void CheckBeforeCreate(CreateCurrencyDto input)
        {
            var validationResultMessage = string.Empty;

            if (_currencyManager.CheckIfNameAlreadyExist(input.Name, (int)input.BranchId))
            {
                validationResultMessage = L(ValidationResultMessage.NameAleadyExist);
            }

            if (!string.IsNullOrEmpty(validationResultMessage))
                throw new UserFriendlyException(validationResultMessage);
        }
        private void CheckBeforeUpdate(UpdateCurrencyDto input, int branchId)
        {
            var validationResultMessage = string.Empty;

            if (_currencyManager.CheckIfNameAlreadyExist(input.Id, input.Name, branchId))
            {
                validationResultMessage = L(ValidationResultMessage.NameAleadyExist);
            }

            if (!string.IsNullOrEmpty(validationResultMessage))
                throw new UserFriendlyException(validationResultMessage);
        }

        #endregion
    }
}
