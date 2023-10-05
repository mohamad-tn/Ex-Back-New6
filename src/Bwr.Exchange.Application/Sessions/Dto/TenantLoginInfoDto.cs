using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Bwr.Exchange.MultiTenancy;

namespace Bwr.Exchange.Sessions.Dto
{
    [AutoMapFrom(typeof(Tenant))]
    public class TenantLoginInfoDto : EntityDto
    {
        public string TenancyName { get; set; }

        public string Name { get; set; }
    }
}
