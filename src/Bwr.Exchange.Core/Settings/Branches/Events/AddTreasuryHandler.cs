using Abp.Dependency;
using Abp.Events.Bus.Handlers;
using Bwr.Exchange.Settings.Treasuries;
using Bwr.Exchange.Settings.Treasuries.Services;
using System;
using System.Threading.Tasks;

namespace Bwr.Exchange.Settings.Branches.Events
{
    public class AddTreasuryHandler : IAsyncEventHandler<AddTreasuryEventData>, ITransientDependency
    {
        private readonly ITreasuryManager _treasuryManager;

        public AddTreasuryHandler(ITreasuryManager treasuryManager)
        {
            _treasuryManager = treasuryManager;
        }

        public async Task HandleEventAsync(AddTreasuryEventData eventData)
        {
            var treasury = new Treasury();
            treasury.Name = eventData.Name;
            treasury.BranchId = (int)eventData.BranchId;
            await _treasuryManager.CreateMainTreasuryAsync(treasury);
        }
    }
}
