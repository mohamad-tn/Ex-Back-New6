using Abp.Application.Services;
using Bwr.Exchange.Settings.Branches.Dto;
using Bwr.Exchange.Shared.Interfaces;

namespace Bwr.Exchange.Settings.Branches
{
    public interface IBranchAppService: IApplicationService,
        ICrudEjAppService<BranchDto, CreateBranchDto, UpdateBranchDto>
    {
    }
}
