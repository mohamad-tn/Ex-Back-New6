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
            var management = ObjectMapper.Map<Management>(input);
            management.UserId = AbpSession.GetUserId();

            var createdManagement = await _managementManager.CreateAsync(management);
            return ObjectMapper.Map<ManagementDto>(createdManagement);
        }

        public async Task<Dictionary<int,double>> GetChangesCount()
        {
            var changes = await _managementManager.getChangesCount();
            return changes;
        }

        [HttpPost]
        public ReadGrudDto GetForGrid([FromBody] BwireDataManagerRequest dm)
        {
            var input = new SearchManagementDto();

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
            var managements = _managementManager.Get(dic,dm.type);

            IEnumerable<ManagementDto> data = ObjectMapper.Map<List<ManagementDto>>(managements);
            var operations = new DataOperations();

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
