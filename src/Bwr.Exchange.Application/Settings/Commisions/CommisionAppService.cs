using Abp.Threading;
using Abp.UI;
using Bwr.Exchange.Settings.Commisions.Dto;
using Bwr.Exchange.Settings.Commisions.Services;
using Bwr.Exchange.Shared.DataManagerRequests;
using Bwr.Exchange.Shared.Dto;
using Bwr.Exchange.Shared.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Syncfusion.EJ2.Base;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bwr.Exchange.Settings.Commisions
{
    public class CommisionAppService : ExchangeAppServiceBase, ICommisionAppService
    {
        private readonly ICommisionManager _commisionManager;
        public CommisionAppService(CommisionManager commisionManager)
        {
            _commisionManager = commisionManager;
        }

        public async Task<IList<CommisionDto>> GetAllAsync()
        {
            var commisions = await _commisionManager.GetAllAsync();

            return ObjectMapper.Map<List<CommisionDto>>(commisions);
        }
        [HttpPost]
        public ReadGrudDto GetForGrid([FromBody] BWireDataManagerRequest dm)
        {
            var data = _commisionManager.GetAllWithDetails();
            IEnumerable<ReadCommisionDto> commisions = ObjectMapper.Map<List<ReadCommisionDto>>(data);

            var operations = new DataOperations();
            if (dm.Where != null && dm.Where.Count > 0)
            {
                commisions = operations.PerformFiltering(commisions, dm.Where, "and");
            }

            if (dm.Sorted != null && dm.Sorted.Count > 0)
            {
                commisions = operations.PerformSorting(commisions, dm.Sorted);
            }

            IEnumerable groupDs = new List<ReadCommisionDto>();
            if (dm.Group != null)
            {
                groupDs = operations.PerformSelect(commisions, dm.Group);
            }

            var count = commisions.Count();

            if (dm.Skip != 0)
            {
                commisions = operations.PerformSkip(commisions, dm.Skip);
            }

            if (dm.Take != 0)
            {
                commisions = operations.PerformTake(commisions, dm.Take);
            }

            return new ReadGrudDto() { result = commisions, count = count, groupDs = groupDs };
        }
        public UpdateCommisionDto GetForEdit(int id)
        {
            var commision = AsyncHelper.RunSync(() => _commisionManager.GetByIdAsync(id));
            return ObjectMapper.Map<UpdateCommisionDto>(commision);
        }
        public async Task<CommisionDto> CreateAsync(CreateCommisionDto input)
        {
            //CheckBeforeCreate(input);

            var commision = ObjectMapper.Map<Commision>(input);

            var createdCommision = await _commisionManager.InsertAndGetAsync(commision);

            return ObjectMapper.Map<CommisionDto>(createdCommision);
        }
        public async Task<CommisionDto> UpdateAsync(UpdateCommisionDto input)
        {
            //CheckBeforeUpdate(input);

            var commision = await _commisionManager.GetByIdAsync(input.Id);

            ObjectMapper.Map<UpdateCommisionDto, Commision>(input, commision);

            var updatedCommision = await _commisionManager.UpdateAndGetAsync(commision);

            return ObjectMapper.Map<CommisionDto>(updatedCommision);
        }
        public async Task DeleteAsync(int id)
        {
            await _commisionManager.DeleteAsync(id);
        }

        #region Helper methods
        private void CheckBeforeCreate(CreateCommisionDto input)
        {
            var validationResultMessage = string.Empty;

            if (_commisionManager.CheckIfCommisionAlreadyExist(input.CurrencyId))
            {
                validationResultMessage = L(ValidationResultMessage.CommusionAleadyExistForThisCurrency);
            }

            if (!string.IsNullOrEmpty(validationResultMessage))
                throw new UserFriendlyException(validationResultMessage);
        }
        private void CheckBeforeUpdate(UpdateCommisionDto input)
        {
            var validationResultMessage = string.Empty;

            if (_commisionManager.CheckIfCommisionAlreadyExist(input.Id, input.CurrencyId))
            {
                validationResultMessage = L(ValidationResultMessage.CommusionAleadyExistForThisCurrency);
            }

            if (!string.IsNullOrEmpty(validationResultMessage))
                throw new UserFriendlyException(validationResultMessage);
        }

        #endregion
    }
}
