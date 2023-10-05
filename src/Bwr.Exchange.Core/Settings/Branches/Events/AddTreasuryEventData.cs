using Abp.Events.Bus;

namespace Bwr.Exchange.Settings.Branches.Events
{
    public class AddTreasuryEventData: EventData
    {
        public AddTreasuryEventData(string name, int branchId)
        {
            Name = name;
            BranchId = branchId;
        }
        public string Name { get; set; }
        public int? BranchId { get; set; }
    }
}
