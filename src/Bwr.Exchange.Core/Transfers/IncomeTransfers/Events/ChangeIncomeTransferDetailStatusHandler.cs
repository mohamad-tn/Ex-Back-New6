using Abp.Dependency;
using Abp.Events.Bus.Handlers;
using Bwr.Exchange.Transfers.IncomeTransfers.Services.Interfaces;
using System.Threading.Tasks;

namespace Bwr.Exchange.Transfers.IncomeTransfers.Events
{
    public class ChangeIncomeTransferDetailStatusHandler : IAsyncEventHandler<ChangeIncomeTransferDetailStatusEventData>, ITransientDependency
    {
        private readonly IIncomeTransferDetailManager _incomeTransferDetailManager;

        public ChangeIncomeTransferDetailStatusHandler(IIncomeTransferDetailManager incomeTransferDetailManager)
        {
            _incomeTransferDetailManager = incomeTransferDetailManager;
        }

        public async Task HandleEventAsync(ChangeIncomeTransferDetailStatusEventData eventData)
        {
            await _incomeTransferDetailManager.UpdateAsync(eventData.IncomeTransferDetail);
        }
    }
}
