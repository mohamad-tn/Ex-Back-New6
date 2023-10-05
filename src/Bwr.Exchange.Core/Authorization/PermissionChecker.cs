using Abp.Authorization;
using Bwr.Exchange.Authorization.Roles;
using Bwr.Exchange.Authorization.Users;

namespace Bwr.Exchange.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {
        }
    }
}
