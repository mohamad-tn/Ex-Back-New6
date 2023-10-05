using Abp.Application.Services.Dto;
using Bwr.Exchange.Users.Dto;
using System.Collections.Generic;

namespace Bwr.Exchange.Settings.Branches.Dto
{
    public class BranchDto: EntityDto
    {
        public BranchDto()
        {
            Users = new List<UserDto>();
        }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public int CountryId { get; set; }

        #region Users
        public List<UserDto> Users { get; set; }
        #endregion
    }
}
