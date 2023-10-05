using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Microsoft.AspNetCore.Identity;

namespace Bwr.Exchange.Controllers
{
    public abstract class ExchangeControllerBase: AbpController
    {
        protected ExchangeControllerBase()
        {
            LocalizationSourceName = ExchangeConsts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
