using Abp.Events.Bus;
using System;
using System.Collections.Generic;

namespace Bwr.Exchange.CashFlows.CompanyCashFlows.Events
{
    public class CompanyCashFlowCreatedMultiEventData : EventData
    {
        public CompanyCashFlowCreatedMultiEventData()
        {
            Data = new List<CompanyCashFlowCreatedEventData>();
        }
        public IList<CompanyCashFlowCreatedEventData> Data { get; set; }

        internal bool Any()
        {
            throw new NotImplementedException();
        }
    }
}
