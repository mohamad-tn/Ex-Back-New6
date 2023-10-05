using Abp.Application.Services;
using Bwr.Exchange.MultiTenancy.Dto;

namespace Bwr.Exchange.MultiTenancy
{
    public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedTenantResultRequestDto, CreateTenantDto, TenantDto>
    {
    }
}

