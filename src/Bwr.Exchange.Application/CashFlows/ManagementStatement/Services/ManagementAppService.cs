using Bwr.Exchange.CashFlows.ManagementStatement.Dto;
using Bwr.Exchange.Shared.Dto;
using Bwr.Exchange.Transfers.OutgoingTransfers.Dto;
using Microsoft.AspNetCore.Mvc;
using Syncfusion.EJ2.Base;
using System;
using Bwr.Exchange.Reflection.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using Abp.Runtime.Session;
using Abp.Threading;

namespace Bwr.Exchange.CashFlows.ManagementStatement.Services
{
    public class ManagementAppService : ExchangeAppServiceBase, IManagementAppService
    {
        private readonly IManagementMnager _managementManager;

        public ManagementAppService(IManagementMnager managementManager)
        {
            _managementManager = managementManager;
        }

        public async Task<ManagementDto> CreateAsync(CreateManagementDto input)
        {
            var currentBranch = await GetCurrentBranch();
            input.BranchId = currentBranch.Id;

            var management = ObjectMapper.Map<Management>(input);
            management.UserId = AbpSession.GetUserId();

            var createdManagement = await _managementManager.CreateAsync(management);
            return ObjectMapper.Map<ManagementDto>(createdManagement);
        }

        public async Task<Dictionary<int,double>> GetChangesCount()
        {
            var branch = await GetCurrentBranch();

            var changes = await _managementManager.getChangesCount(branch.Id);
            return changes;
        }

        [HttpPost]
        public ReadGrudDto GetForGrid([FromBody] ManagementDataManagerRequest dm)
        {
            var user = UserManager.GetUserById(dm.userId);
            var input = new SearchManagementDto();
            IList<Management> managements = new List<Management>();

            DateTime fromDate = new DateTime(), toDate = new DateTime();

            if (!string.IsNullOrEmpty(dm.fromDate))
            {
                DateTime.TryParse(dm.fromDate, out fromDate);
                fromDate = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day, 0, 0, 0);
                input.FromDate = fromDate.ToString();
            }

            if (!string.IsNullOrEmpty(dm.toDate))
            {
                DateTime.TryParse(dm.toDate, out toDate);
                toDate = new DateTime(toDate.Year, toDate.Month, toDate.Day, 23, 59, 59);
                input.ToDate = toDate.ToString();
            }


            var dic = input.ToDictionary();

            if(user.BranchId != null)
            {
                managements = _managementManager.Get(dic, dm.type, (int)user.BranchId);
            }
            else
            {
                managements = _managementManager.Get(dic, dm.type, null);
            }

            IEnumerable<ManagementDto> data = ObjectMapper.Map<List<ManagementDto>>(managements);
            var operations = new DataOperations();

            //if (dm.Where != null && dm.Where.Count > 0)
            //{
            //    data = operations.PerformFiltering(data, dm.Where, "and");
            //}

            if (dm.Sorted != null && dm.Sorted.Count > 0)
            {
                data = operations.PerformSorting(data, dm.Sorted);
            }

            IEnumerable groupDs = new List<ReadOutgoingTransferDto>();
            if (dm.Group != null)
            {
                groupDs = operations.PerformSelect(data, dm.Group);
            }

            var count = data.Count();

            if (dm.Skip != 0)
            {
                data = operations.PerformSkip(data, dm.Skip);
            }

            if (dm.Take != 0)
            {
                data = operations.PerformTake(data, dm.Take);
            }

            return new ReadGrudDto() { result = data, count = count, groupDs = groupDs };
        }

        private WhereFilter GetWhereFilter(List<WhereFilter> filterOptions, string name)
        {
            var filter = filterOptions.FirstOrDefault(x => x.Field == name);
            if (filter != null)
                return filter;

            foreach (var option in filterOptions)
            {
                if (option.predicates != null)
                    return GetWhereFilter(option.predicates, name);
            }

            return null;
        }
    }
}
