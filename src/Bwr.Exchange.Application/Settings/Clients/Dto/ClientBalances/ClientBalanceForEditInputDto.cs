using Abp.Application.Services.Dto;

namespace Bwr.Exchange.Settings.Clients.Dto
{
    public class ClientBalanceForEditInputDto: EntityDto
    {
        public int TransactionType { get; set; }
    }
}
