using Abp.Application.Services;
using Bwr.Exchange.CashFlows.ManagementStatement.Dto;
using Bwr.Exchange.Shared.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bwr.Exchange.CashFlows.ManagementStatement.Services
{
    public interface IManagementAppService : IApplicationService
    {
        Task<ManagementDto> CreateAsync(CreateManagementDto input);
        Task<Dictionary<int, double>> GetChangesCount();
        ReadGrudDto GetForGrid([FromBody] BwireDataManagerRequest dm);
    }
}
