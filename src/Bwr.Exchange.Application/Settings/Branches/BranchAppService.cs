using Bwr.Exchange.Settings.Branches.Dto;
using Bwr.Exchange.Settings.Branches.Services;
using Bwr.Exchange.Shared.Dto;
using Microsoft.AspNetCore.Mvc;
using Syncfusion.EJ2.Base;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.UI;
using Bwr.Exchange.Shared.DataManagerRequests;

namespace Bwr.Exchange.Settings.Branches
{
    public class BranchAppService : ExchangeAppServiceBase, IBranchAppService
    {
        private readonly IBranchManager _branchManager;

        public BranchAppService(IBranchManager branchManager)
        {
            _branchManager = branchManager;
        }

        public async Task<BranchDto> CreateAsync(CreateBranchDto input)
        {
            CheckBeforeCreate(input);
            var branch = ObjectMapper.Map<Branch>(input);

            var createdBranch = await _branchManager.InsertAndGetAsync(branch);

            return ObjectMapper.Map<BranchDto>(createdBranch);
        }

        public async Task DeleteAsync(int id)
        {
            await _branchManager.DeleteAsync(id);
        }

        public async Task<IList<BranchDto>> GetAllAsync()
        {
            var branches = await _branchManager.GetAllAsync();

            return ObjectMapper.Map<List<BranchDto>>(branches);
        }

        public UpdateBranchDto GetForEdit(int id)
        {
            var branch = _branchManager.GetByIdWithDetail(id);
            return ObjectMapper.Map<UpdateBranchDto>(branch);
        }

        [HttpPost]
        public ReadGrudDto GetForGrid([FromBody] BWireDataManagerRequest dm)
        {
            var data = _branchManager.GetAll();
            IEnumerable<ReadBranchDto> branches = ObjectMapper.Map<List<ReadBranchDto>>(data);

            var operations = new DataOperations();
            if (dm.Where != null && dm.Where.Count > 0)
            {
                branches = operations.PerformFiltering(branches, dm.Where, "and");
            }

            if (dm.Sorted != null && dm.Sorted.Count > 0)
            {
                branches = operations.PerformSorting(branches, dm.Sorted);
            }

            IEnumerable groupDs = new List<ReadBranchDto>();
            if (dm.Group != null)
            {
                groupDs = operations.PerformSelect(branches, dm.Group);
            }

            var count = branches.Count();

            if (dm.Skip != 0)
            {
                branches = operations.PerformSkip(branches, dm.Skip);
            }

            if (dm.Take != 0)
            {
                branches = operations.PerformTake(branches, dm.Take);
            }

            return new ReadGrudDto() { result = branches, count = count, groupDs = groupDs };

        }

        public async Task<BranchDto> UpdateAsync(UpdateBranchDto input)
        {
            CheckBeforeUpdate(input);

            var branch = await _branchManager.GetByIdAsync(input.Id);

            ObjectMapper.Map<UpdateBranchDto, Branch>(input, branch);

            var updatedBranch = await _branchManager.UpdateAndGetAsync(branch);

            return ObjectMapper.Map<BranchDto>(updatedBranch);
        }

        #region Helper methods
        private void CheckBeforeCreate(CreateBranchDto input)
        {
            var validationResultMessage = string.Empty;

            if (_branchManager.CheckIfNameAlreadyExist(input.Name))
            {
                validationResultMessage = L(ValidationResultMessage.NameAleadyExist);
            }

            if (!string.IsNullOrEmpty(validationResultMessage))
                throw new UserFriendlyException(validationResultMessage);
        }
        private void CheckBeforeUpdate(UpdateBranchDto input)
        {
            var validationResultMessage = string.Empty;

            if (_branchManager.CheckIfNameAlreadyExist(input.Id, input.Name))
            {
                validationResultMessage = L(ValidationResultMessage.NameAleadyExist);
            }

            if (!string.IsNullOrEmpty(validationResultMessage))
                throw new UserFriendlyException(validationResultMessage);
        }
        #endregion
    }
}
