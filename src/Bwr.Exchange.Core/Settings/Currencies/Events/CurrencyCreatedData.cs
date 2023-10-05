using Abp.Events.Bus;

namespace Bwr.Exchange.Settings.Currencies.Events
{
    public class CurrencyCreatedData : EventData
    {
        public int CurrencyId { get; set; }
    }
}
