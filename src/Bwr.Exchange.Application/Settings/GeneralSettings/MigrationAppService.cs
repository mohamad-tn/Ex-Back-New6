//using Bwr.Exchange.CashFlows.ClientCashFlows.Services;
//using Bwr.Exchange.CashFlows.CompanyCashFlows.Services;
//using Bwr.Exchange.CashFlows.TreasuryCashFlows.Services;
//using Bwr.Exchange.Settings.Clients.Services;
//using Bwr.Exchange.Settings.Companies.Services;
//using Bwr.Exchange.Settings.GeneralSettings.Dto;
//using Bwr.Exchange.Settings.Treasuries.Services;
//using System.Data.SqlClient;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace Bwr.Exchange.Settings.GeneralSettings
//{
//    public class MigrationAppService : ExchangeAppServiceBase, IMigrationAppService
//    {
//        private readonly IClientManager _clientManager;
//        private readonly IClientCashFlowManager _clientCashFlowManager;
//        private readonly ICompanyManager _companyManager;
//        private readonly ICompanyCashFlowManager _companyCashFlowManager;
//        private readonly ITreasuryManager _treasuryManager;
//        private readonly ITreasuryCashFlowManager _treasuryCashFlowManager;
//        private readonly string _connectionSerting;
//        public MigrationAppService(
//            IClientManager clientManager, 
//            IClientCashFlowManager clientCashFlowManager, 
//            ICompanyManager companyManager, 
//            ICompanyCashFlowManager companyCashFlowManager, 
//            ITreasuryManager treasuryManager, 
//            ITreasuryCashFlowManager treasuryCashFlowManager)
//        {
//            _clientManager = clientManager;
//            _clientCashFlowManager = clientCashFlowManager;
//            _companyManager = companyManager;
//            _companyCashFlowManager = companyCashFlowManager;
//            _treasuryManager = treasuryManager;
//            _treasuryCashFlowManager = treasuryCashFlowManager;
//        }

//        public MigrationOutputDto Migrate(MigrationInputDto input)
//        {
//            try
//            {
//                MigrateClient();
//                return new MigrationOutputDto(false);
//            }
//            catch(Exception ex)
//            {
//                return new MigrationOutputDto(false);
//            }

//        }

//        private bool MigrateClient()
//        {
//            var clientIds = new List<int>();
//            var clients = _clientManager.GetAllAsync();
//            foreach (var item in clientIds)
//            {
//                using(var sqlConnection = SqlConnection)
//            }
//        }
//    }
//}
