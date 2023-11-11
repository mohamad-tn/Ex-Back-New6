using Abp.Domain.Services;
using Bwr.Exchange.Transfers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bwr.Exchange.CashFlows.ManagementStatement.Services
{
    public interface IManagementMnager : IDomainService
    {
        Task<Management> CreateAsync(Management input);
        Task<Dictionary<int,double>> getChangesCount();
        IList<Management> Get(Dictionary<string, object> dic, int type);
    }
}
