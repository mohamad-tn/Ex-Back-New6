using Abp.Application.Services.Dto;
using Bwr.Exchange.Settings.Clients.Dto.ClientBalances;
using Bwr.Exchange.Settings.Clients.Dto.ClientPhones;
using System.Collections.Generic;

namespace Bwr.Exchange.Settings.Clients.Dto
{
    public class UpdateClientDto : EntityDto
    {
        public UpdateClientDto()
        {
            ClientBalances = new List<ClientBalanceDto>();
            ClientPhones = new List<ClientPhoneDto>();
        }
        public string Name { get; set; }
        public string Address { get; set; }
        public bool Activated { get; set; }
        public int ProvinceId { get; set; }

        public virtual IList<ClientBalanceDto> ClientBalances { get; set; }
        public virtual IList<ClientPhoneDto> ClientPhones { get; set; }
    }
}
