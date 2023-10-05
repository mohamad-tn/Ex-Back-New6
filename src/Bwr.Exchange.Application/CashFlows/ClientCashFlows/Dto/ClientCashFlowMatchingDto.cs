using Bwr.Exchange.CashFlows.Shared.Dto;
using System.Collections.Generic;

namespace Bwr.Exchange.CashFlows.ClientCashFlows.Dto
{
    public class ClientCashFlowMatchingDto
    {
        public ClientCashFlowMatchingDto()
        {
            Items = new List<MatchingItemDto>();
        }
        public string MatchingBy { get; set; }
        public string MatchingWith { get; set; }
        public string Description { get; set; }
        public IList<MatchingItemDto> Items { get; set; }
    }
}
