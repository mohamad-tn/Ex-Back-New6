using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Abp.Application.Services;
using Abp.IdentityFramework;
using Abp.Runtime.Session;
using Bwr.Exchange.Authorization.Users;
using Bwr.Exchange.MultiTenancy;
using Bwr.Exchange.Settings.Treasuries.Services;
using Bwr.Exchange.Settings.Treasuries;
using Abp.UI;
using Bwr.Exchange.Settings.Branches;
using Bwr.Exchange.Settings.Branches.Services;

namespace Bwr.Exchange
{
    /// <summary>
    /// Derive your application services from this class.
    /// </summary>
    public abstract class ExchangeAppServiceBase : ApplicationService
    {
        public TenantManager TenantManager { get; set; }

        public UserManager UserManager { get; set; }
        public BranchManager BranchManager { get; set; }
        public TreasuryManager TreasuryManager { get; set; }

        protected ExchangeAppServiceBase()
        {
            LocalizationSourceName = ExchangeConsts.LocalizationSourceName;
        }

        protected virtual async Task<User> GetCurrentUserAsync()
        {
            var user = await UserManager.FindByIdAsync(AbpSession.GetUserId().ToString());
            if (user == null)
            {
                throw new Exception("There is no current user!");
            }

            return user;
        }

        protected virtual Task<Tenant> GetCurrentTenantAsync()
        {
            return TenantManager.GetByIdAsync(AbpSession.GetTenantId());
        }

        protected virtual async Task<Branch> GetCurrentBranch()
        {
            var user = await GetCurrentUserAsync();
            if (user.BranchId == null)
            {
                throw new UserFriendlyException(L(ValidationResultMessage.CurrentUserIsNotAssociatedWithBranchAndTreasury));
            }
            var branch = await BranchManager.GetByIdAsync((int)user.BranchId);
            return branch;
        }

        protected virtual async Task<Treasury> GetTreasuryByBranchIdAsync()
        {
            var user = await GetCurrentUserAsync();
            if (user.BranchId == null)
            {
                throw new UserFriendlyException(L(ValidationResultMessage.CurrentUserIsNotAssociatedWithBranchAndTreasury));
            }
            if (user == null)
            {
                throw new Exception("There is no current user!");
            }

            var treasury = await TreasuryManager.GetByBranchIdAsync((int)user.BranchId);
            return treasury;
        }

        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
