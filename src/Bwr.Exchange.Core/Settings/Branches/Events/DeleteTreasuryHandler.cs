using Abp.Dependency;
using Abp.Events.Bus.Handlers;
using Bwr.Exchange.Settings.Treasuries.Services;
using System.Threading.Tasks;

namespace Bwr.Exchange.Settings.Branches.Events
{
    public class DeleteTreasuryHandler : IAsyncEventHandler<DeleteTreasuryEventData>, ITransientDependency
    {
        private readonly ITreasuryManager _treasuryManager;

        public DeleteTreasuryHandler(ITreasuryManager treasuryManager)
        {
            _treasuryManager = treasuryManager;
        }

        public async Task HandleEventAsync(DeleteTreasuryEventData eventData)
        {
            var treasury = await _treasuryManager.GetByBranchIdAsync((int)eventData.BranchId);
            if (treasury != null)
            {
                await _treasuryManager.DeleteAsync(treasury.Id);
            }
        }
    }
}
