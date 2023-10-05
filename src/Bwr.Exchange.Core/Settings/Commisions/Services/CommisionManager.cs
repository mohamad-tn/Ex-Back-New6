using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bwr.Exchange.Settings.Commisions.Services
{
    public class CommisionManager: ICommisionManager
    {
        private readonly IRepository<Commision> _commisionRepository;

        public CommisionManager(IRepository<Commision> commisionRepository)
        {
            _commisionRepository = commisionRepository;
        }

        public bool CheckIfCommisionAlreadyExist(int currencyId)
        {
            var commision = _commisionRepository.FirstOrDefault(x => x.CurrencyId == currencyId);
            return commision != null ? true : false;
        }

        public bool CheckIfCommisionAlreadyExist(int commisionId, int currencyId)
        {
            var commision = _commisionRepository.FirstOrDefault(x => x.CurrencyId == currencyId && x.Id != commisionId);
            return commision != null ? true : false;
        }

        public async Task DeleteAsync(int id)
        {
            var commision = await GetByIdAsync(id);
            if (commision != null)
            {
                await _commisionRepository.DeleteAsync(commision);
            }
        }

        public IList<Commision> GetAll()
        {
            return _commisionRepository.GetAllList();
        }

        public async Task<IList<Commision>> GetAllAsync()
        {
            return await _commisionRepository.GetAllListAsync();
        }

        public IList<Commision> GetAllWithDetails()
        {
            return _commisionRepository.GetAllIncluding(c => c.Currency).ToList();
        }

        public async Task<Commision> GetByIdAsync(int id)
        {
            return await _commisionRepository.FirstOrDefaultAsync(id);
        }

        public async Task<Commision> InsertAndGetAsync(Commision commision)
        {
            return await _commisionRepository.InsertAsync(commision);
        }

        public async Task<Commision> UpdateAndGetAsync(Commision commision)
        {
            return await _commisionRepository.UpdateAsync(commision);
        }
    }
}
