using AutoMapper;
using Bwr.Exchange.Settings.Branches.Dto;

namespace Bwr.Exchange.Settings.Branches.Map
{
    public class BranchMapProfile: Profile
    {
        public BranchMapProfile()
        {
            CreateMap<Branch, BranchDto>();
            CreateMap<Branch, ReadBranchDto>();
            CreateMap<Branch, CreateBranchDto>();
            CreateMap<CreateBranchDto, Branch>();
            CreateMap<Branch, UpdateBranchDto>();
            CreateMap<UpdateBranchDto, Branch>();
        }
    }
}
