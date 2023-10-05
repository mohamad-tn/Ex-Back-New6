using Abp.UI;
using Bwr.Exchange.Settings.Countries.Dto;
using Bwr.Exchange.Settings.Countries.Services;
using Bwr.Exchange.Shared.DataManagerRequests;
using Bwr.Exchange.Shared.Dto;
using Bwr.Exchange.Shared.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Syncfusion.EJ2.Base;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bwr.Exchange.Settings.Countries
{
    public class CountryAppService : ExchangeAppServiceBase, ICountryAppService
    {
        private readonly ICountryManager _countryManager;
        public CountryAppService(CountryManager countryManager)
        {
            _countryManager = countryManager;
        }

        public async Task<IList<CountryDto>> GetAllAsync()
        {
            var countries = await _countryManager.GetAllAsync();

            return ObjectMapper.Map<List<CountryDto>>(countries);
        }

        public IList<CountryDto> GetAllWithDetail()
        {
            var countries = _countryManager.GetAllWithDetail();

            return ObjectMapper.Map<List<CountryDto>>(countries);
        }

        [HttpPost]
        public ReadGrudDto GetForGrid([FromBody] BWireDataManagerRequest dm)
        {
            var data = _countryManager.GetAll();
            IEnumerable<ReadCountryDto> countries = ObjectMapper.Map<List<ReadCountryDto>>(data);

            var operations = new DataOperations();
            if (dm.Where != null && dm.Where.Count > 0)
            {
                countries = operations.PerformFiltering(countries, dm.Where, "and");
            }

            if (dm.Sorted != null && dm.Sorted.Count > 0)
            {
                countries = operations.PerformSorting(countries, dm.Sorted);
            }

            IEnumerable groupDs = new List<ReadCountryDto>();
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
        public UpdateCountryDto GetForEdit(int id)
        {
            var country =  _countryManager.GetByIdWithDetail(id);
            return ObjectMapper.Map<UpdateCountryDto>(country);
        }
        public async Task<CountryDto> CreateAsync(CreateCountryDto input)
        {
            CheckBeforeCreate(input);

            var country = ObjectMapper.Map<Country>(input);

            var createdCountry = await _countryManager.InsertAndGetAsync(country);

            return ObjectMapper.Map<CountryDto>(createdCountry);
        }
        public async Task<CountryDto> UpdateAsync(UpdateCountryDto input)
        {
            CheckBeforeUpdate(input);

            var country = await _countryManager.GetByIdAsync(input.Id);

            ObjectMapper.Map<UpdateCountryDto, Country>(input, country);

            var updatedCountry = await _countryManager.UpdateAndGetAsync(country);

            return ObjectMapper.Map<CountryDto>(updatedCountry);
        }
        public async Task DeleteAsync(int id)
        {
            await _countryManager.DeleteAsync(id);
        }

        #region Helper methods
        private void CheckBeforeCreate(CreateCountryDto input)
        {
            var validationResultMessage = string.Empty;

            if (_countryManager.CheckIfNameAlreadyExist(input.Name))
            {
                validationResultMessage = L(ValidationResultMessage.NameAleadyExist);
            }

            if (!string.IsNullOrEmpty(validationResultMessage))
                throw new UserFriendlyException(validationResultMessage);
        }
        private void CheckBeforeUpdate(UpdateCountryDto input)
        {
            var validationResultMessage = string.Empty;

            if (_countryManager.CheckIfNameAlreadyExist(input.Id, input.Name))
            {
                validationResultMessage = L(ValidationResultMessage.NameAleadyExist);
            }

            if (!string.IsNullOrEmpty(validationResultMessage))
                throw new UserFriendlyException(validationResultMessage);
        }

        

        #endregion
    }
}
