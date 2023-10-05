using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bwr.Exchange.Settings.Countries.Services
{
    public class CountryManager : ICountryManager
    {
        private readonly IRepository<Country> _countryRepository;
        private readonly IRepository<Province> _provinceRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        public CountryManager(
            IRepository<Country> countryRepository,
            IRepository<Province> provinceRepository,
            IUnitOfWorkManager unitOfWorkManager
            )
        {
            _countryRepository = countryRepository;
            _provinceRepository = provinceRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public bool CheckIfNameAlreadyExist(string name)
        {
            var country = _countryRepository.FirstOrDefault(x => x.Name.Trim().Equals(name.Trim()));
            return country != null ? true : false;
        }

        public bool CheckIfNameAlreadyExist(int id, string name)
        {
            var country = _countryRepository.FirstOrDefault(x => x.Id != id && x.Name.Trim().Equals(name.Trim()));
            return country != null ? true : false;
        }

        public async Task DeleteAsync(int id)
        {
            var country = await GetByIdAsync(id);
            if (country != null)
                await _countryRepository.DeleteAsync(country);
        }

        public IList<Country> GetAll()
        {
            return _countryRepository.GetAll().ToList();
        }

        public async Task<IList<Country>> GetAllAsync()
        {
            return await _countryRepository.GetAllListAsync();
        }

        public IList<Country> GetAllWithDetail()
        {
            var countries = _countryRepository.GetAllIncluding(x => x.Provinces);
            return countries.ToList();
        }

        public Country GetByIdWithDetail(int id)
        {
            var country = GetAllWithDetail().FirstOrDefault(x=>x.Id == id);
            return country;
        }

        public async Task<Country> GetByIdAsync(int id)
        {
            return await _countryRepository.GetAsync(id);
        }

        public async Task<Country> InsertAndGetAsync(Country country)
        {
            return await _countryRepository.InsertAsync(country);
        }

        public async Task<Country> UpdateAndGetAsync(Country country)
        {
            using (var unitOfWork = _unitOfWorkManager.Begin())
            {
                var updatedCountry = await _countryRepository.UpdateAsync(country);

                //Update provinces
                var provinces = country.Provinces.ToList();//Don't remove ToList()
                await RemoveProvinces(updatedCountry, provinces);
                await AddNewProvinces(updatedCountry, provinces);

                unitOfWork.Complete();
            }
            return await GetByIdAsync(country.Id);
        }

        #region Helper Methods
        private async Task RemoveProvinces(Country country, IList<Province> newProvinces)
        {
            var oldProvinces = await _provinceRepository.GetAllListAsync(x => x.CountryId == country.Id);

            foreach (var oldProvince in oldProvinces)
            {
                var isExist = newProvinces.Any(x => x.Name == oldProvince.Name.Trim());
                if (!isExist)
                {
                    await _provinceRepository.DeleteAsync(oldProvince);
                }
            }
        }

        private async Task AddNewProvinces(Country country, IList<Province> newProvinces)
        {
            var oldProvinces = await _provinceRepository.GetAllListAsync(x => x.CountryId == country.Id);
            foreach (var newProvince in newProvinces)
            {
                var isExist = oldProvinces.Any(x => x.Name == newProvince.Name.Trim());
                if (!isExist)
                {
                    await _provinceRepository.InsertAsync(newProvince);
                }
            }
        }
        #endregion
    }
}
