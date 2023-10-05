using Abp.Application.Services;
using Bwr.Exchange.Shared.DataManagerRequests;
using Bwr.Exchange.Shared.Dto;
using Bwr.Exchange.Transfers.IncomeTransfers.Dto;
using Microsoft.AspNetCore.Mvc;
using Syncfusion.EJ2.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bwr.Exchange.Transfers.IncomeTransfers
{
    public interface IIncomeTransferDetailAppService : IApplicationService
    {
        IList<IncomeTransferDetailDto> GetNotReceived(int currencyId);
        IList<IncomeTransferDetailDto> GetAllNotReceived();
        IList<IncomeTransferDetailDto> GetAllDirectTransfers();
        Task<IncomeTransferDetailDto> ChangeStatusAsync(IncomeTransferDetailChangeStatusInput input);
        ReadGrudDto GetDirectTransferForGrid([FromBody] TreasuryCashFlowDataManagerRequest dm);
        Task<IList<FileUploadDto>> GetDirectTransferImagesAsync(int incomeTransferDetailId);
    }
}
