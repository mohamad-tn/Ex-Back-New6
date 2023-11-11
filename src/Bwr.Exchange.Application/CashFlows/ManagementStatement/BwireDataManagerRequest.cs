using Newtonsoft.Json;
using Syncfusion.EJ2.Base;
using System;

namespace Bwr.Exchange.CashFlows.ManagementStatement
{
    public class BwireDataManagerRequest : DataManagerRequest
    {
        [JsonProperty("type")]
        public int type { get; set; }
        public string fromDate { get; set; }
        public string toDate { get; set; }
    }
}
