using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bwr.Exchange.Settings.Branches.Dto
{
    public class UpdateBranchDto: EntityDto
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public int CountryId { get; set; }
    }
}
