using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Bwr.Exchange.Roles.Dto;
using Bwr.Exchange.Shared.Dto;
using Bwr.Exchange.Users.Dto;
using Microsoft.AspNetCore.Mvc;
using Syncfusion.EJ2.Base;

namespace Bwr.Exchange.Users
{
    public interface IUserAppService : IAsyncCrudAppService<UserDto, long, PagedUserResultRequestDto, CreateUserDto, UserDto>
    {
        Task<ListResultDto<RoleDto>> GetRoles();
        Task<ReadGrudDto> GetForGrid([FromBody] DataManagerRequest dm);
        Task ChangeLanguage(ChangeUserLanguageDto input);
        Task<bool> ChangePassword(ChangePasswordDto input);
        Task<IList<UserForDropdownDto>> GetUsersForDropdown();
    }
}
