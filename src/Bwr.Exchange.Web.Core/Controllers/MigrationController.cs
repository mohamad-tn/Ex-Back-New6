using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Bwr.Exchange.Models.Migrate;
using Microsoft.Data.SqlClient;
using Bwr.Exchange.Settings.Clients.Services;
using Bwr.Exchange.CashFlows.ClientCashFlows.Services;
using Bwr.Exchange.Settings.Companies.Services;
using Bwr.Exchange.CashFlows.CompanyCashFlows.Services;
using Bwr.Exchange.Settings.Treasuries.Services;
using Bwr.Exchange.CashFlows.TreasuryCashFlows.Services;
using Bwr.Exchange.Settings.Currencies.Services;
using System.Threading;
using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace Bwr.Exchange.Controllers
{
    [Route("api/[controller]/[action]")]
    public class MigrationController : ExchangeControllerBase
    {
        private readonly ICurrencyManager _currencyManager;
        private readonly IClientManager _clientManager;
        private readonly IClientCashFlowManager _clientCashFlowManager;
        private readonly ICompanyManager _companyManager;
        private readonly ICompanyCashFlowManager _companyCashFlowManager;
        private readonly ITreasuryManager _treasuryManager;
        private readonly ITreasuryCashFlowManager _treasuryCashFlowManager;
        private IConfiguration _configuration;
        private readonly string _connectionSerting;
        public MigrationController(
            ICurrencyManager currencyManager,
            IClientManager clientManager,
            IClientCashFlowManager clientCashFlowManager,
            ICompanyManager companyManager,
            ICompanyCashFlowManager companyCashFlowManager,
            ITreasuryManager treasuryManager,
            ITreasuryCashFlowManager treasuryCashFlowManager,
            IConfiguration configuration)
        {
            _connectionSerting = "server=127.0.0.1;uid=root;pwd='';database=ex3";
            _clientManager = clientManager;
            _currencyManager = currencyManager;
            _clientCashFlowManager = clientCashFlowManager;
            _companyManager = companyManager;
            _companyCashFlowManager = companyCashFlowManager;
            _treasuryManager = treasuryManager;
            _treasuryCashFlowManager = treasuryCashFlowManager;
            _configuration = configuration;
        }

        [HttpPost]
        public MigrationOutput Migrate(MigrationInput input)
        {
            
            return new MigrationOutput(MigrateClient());
        }

        [HttpPost]
        public ClearDatabaseOutput ClearDatabase(ClearDatabaseInput input)
        {
            var connectionSerting = _configuration.GetConnectionString("Default");
            var query = "delete from TreasuryActions;"+
                        "delete from IncomeTransferDetails;" +
                        "delete from IncomeTransfers;" +
                        "delete from OutgoingTransfers;" +
                        "delete from TreasuryCashFlows;" +
                        "delete from CompanyCashFlows;" +
                        "delete from ClientCashFlows;" +
                        "delete from ExchangeCurrencyHistories;" +
                        "delete from ExchangeCurrencies;" +
                        "delete from ExchangePrices;" +
                        "delete from TreasuryBalances;" +
                        "delete from ClientBalances;" +
                        "delete from CompanyBalances;" +
                        "delete from Companies;" +
                        "delete from Clients;" +
                        "delete from currencies;" +
                        "delete from CustomerImages;" +
                        "delete from customers;";

            return new ClearDatabaseOutput(ClearDb(query, connectionSerting));
        }

        private bool MigrateClient()
        {
            var success = true;
            var currencyIds = new List<int>();
            var currencies = _currencyManager.GetAll();
            using (var connection = new MySqlConnection(_connectionSerting))
            {
                connection.Open();
                var transaction = connection.BeginTransaction();
                var command = connection.CreateCommand();
                command.Transaction = transaction;

                try
                {
                    foreach (var currency in currencies)
                    {
                        var date = $"{DateTime.Now.Year}-01-01";
                        var isMain = currency.IsMainCurrency == true ? "1" : "0";
                        var cmd = "INSERT INTO [dbo].[Currencies]" +
                                           "([CreationTime]" +
                                           ",[CreatorUserId]" +
                                           ",[IsDeleted]" +
                                           ",[Name]" +
                                           ",[IsMainCurrency])" +
                                     "VALUES" +
                                           "('"+ date + "'" +
                                           ",1" +
                                           ",0" +
                                           ",N'"+ currency.Name + "'" +
                                           "," + isMain +
                                           ");select SCOPE_IDENTITY();";

                        command.CommandText = cmd;
                        //command.Parameters.AddWithValue("@creationTime", DateTime.Now);
                        //command.Parameters.AddWithValue("@CreatorUserId", 1);
                        //command.Parameters.AddWithValue("@isDeleted", false);
                        //command.Parameters.AddWithValue("@name", currency.Name);
                        //command.Parameters.AddWithValue("@isMainCurrency", currency.IsMainCurrency);

                        var result = command.ExecuteScalar().ToString();
                        int id = 0;
                        if (int.TryParse(result, out id))
                        {
                            currencyIds.Add(id);
                        }
                    }
                    transaction.Commit();

                }
                catch (Exception ex)
                {
                    try
                    {
                        success = false;
                        transaction.Rollback();
                    }
                    catch (Exception rollbackException)
                    {

                    }
                }
                finally
                {
                    connection.Close();
                }
            }

            return success;
        }

        private bool ClearDb(string query,string connectionSerting)
        {
            var success = true;
            using (var connection = new MySqlConnection(connectionSerting))
            {
                connection.Open();
                var transaction = connection.BeginTransaction();
                var command = connection.CreateCommand();
                command.Transaction = transaction;

                try
                {
                    var cmd = query;
                    command.CommandText = cmd;
                    command.ExecuteNonQuery();

                    transaction.Commit();

                }
                catch (Exception ex)
                {
                    try
                    {
                        success = false;
                        transaction.Rollback();
                    }
                    catch (Exception rollbackException)
                    {

                    }
                }
                finally
                {
                    connection.Close();
                }
            }

            return success;
        }
    }
}
