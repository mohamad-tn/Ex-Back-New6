using Abp.Events.Bus;

namespace Bwr.Exchange.Settings.Branches.Events
{
    public class DeleteTreasuryEventData: EventData
    {
        public DeleteTreasuryEventData(int? branchId)
        {
            BranchId = branchId;
        }
        public int? BranchId { get; set; }
    }
}
