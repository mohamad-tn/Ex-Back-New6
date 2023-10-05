using Abp.Application.Services;
using System.Threading.Tasks;

namespace Bwr.Exchange.Settings.Treasury
{
    public interface ITreasuryAppService : IApplicationService
    {
        Task CreateMainTreasuryAsync();
    }
}
