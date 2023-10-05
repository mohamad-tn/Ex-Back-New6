using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Events.Bus;
using Bwr.Exchange.Settings.Branches.Events;
using Bwr.Exchange.Settings.Treasuries;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bwr.Exchange.Settings.Branches.Services
{
    public class BranchManager : IBranchManager
    {
        private readonly IRepository<Branch> _branchRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<Treasury> _treasuryRepository;
        public BranchManager(
            IRepository<Branch> branchRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<Treasury> treasuryRepository
            )
        {
            _branchRepository = branchRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _treasuryRepository = treasuryRepository;
        }

        public async Task<IList<Branch>> GetAllAsync()
        {
            var branches = await _branchRepository.GetAllListAsync();
            return branches;
        }
        public IList<Branch> GetAll()
        {
            return _branchRepository.GetAll().ToList();
        }
        public async Task<Branch> GetByIdAsync(int id)
        {
            return await _branchRepository.GetAsync(id);
        }
        public IList<Branch> GetAllWithDetail()
        {
            var branches = _branchRepository
                .GetAllIncluding(u => u.User);
            return branches.ToList();
        }
        public Branch GetByIdWithDetail(int id)
        {
            var branch = GetAllWithDetail().FirstOrDefault(x => x.Id == id);
            return branch;
        }
        public async Task DeleteAsync(int id)
        {
            var branch = await GetByIdAsync(id);
            if (branch != null)
            {
                using (var unitOfWork = _unitOfWorkManager.Begin())
                {
                    EventBus.Default.Trigger(new DeleteTreasuryEventData(branch.Id));
                    await _branchRepository.DeleteAsync(branch);
                    unitOfWork.Complete();
                }
            }
        }
        public async Task<Branch> InsertAndGetAsync(Branch branch)
        {
            int branchId = 0;
            using (var unitOfWork = _unitOfWorkManager.Begin())
            {
                branchId = await _branchRepository.InsertAndGetIdAsync(branch);
                if (branchId != 0)
                    await EventBus.Default.TriggerAsync(new AddTreasuryEventData(branch.Name, branchId));
                unitOfWork.Complete();
            }
            return await GetByIdAsync(branchId);
        }
        public async Task<Branch> UpdateAndGetAsync(Branch branch)
        {
            using (var unitOfWork = _unitOfWorkManager.Begin())
            {
                var treasury = await _treasuryRepository.FirstOrDefaultAsync(x => x.BranchId == branch.Id);
                treasury.Name = "الصندوق" + "-" + branch.Name;
                var updatedCountry = await _branchRepository.UpdateAsync(branch);
                await _treasuryRepository.UpdateAsync(treasury);

                unitOfWork.Complete();
            }
            return await GetByIdAsync(branch.Id);
        }
        public bool CheckIfNameAlreadyExist(string name)
        {
            var country = _branchRepository.FirstOrDefault(x => x.Name.Trim().Equals(name.Trim()));
            return country != null ? true : false;
        }
        public bool CheckIfNameAlreadyExist(int id, string name)
        {
            var country = _branchRepository.FirstOrDefault(x => x.Id != id && x.Name.Trim().Equals(name.Trim()));
            return country != null ? true : false;
        }

    }
}
