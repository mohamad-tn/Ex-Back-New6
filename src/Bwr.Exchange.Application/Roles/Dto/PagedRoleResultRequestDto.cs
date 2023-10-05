using Abp.Application.Services.Dto;

namespace Bwr.Exchange.Roles.Dto
{
    public class PagedRoleResultRequestDto : PagedResultRequestDto
    {
        public string Keyword { get; set; }
    }
}

