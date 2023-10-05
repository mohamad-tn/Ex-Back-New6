using Abp.Application.Services.Dto;
using Bwr.Exchange.Users.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bwr.Exchange.Settings.Branches.Dto
{
    public class ReadBranchDto: EntityDto
    {
        public ReadBranchDto()
        {
            users = new List<UserDto>();
        }
        public string name { get; set; }
        public bool isActive { get; set; }
        public int countryId { get; set; }

        #region Users
        public List<UserDto> users { get; set; }
        #endregion
    }
}
