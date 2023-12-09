using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using Bwr.Exchange.Authorization.Roles;
using Bwr.Exchange.Authorization.Users;
using Bwr.Exchange.MultiTenancy;
using Bwr.Exchange.Authorization.Roles;
using Bwr.Exchange.Authorization.Users;
using Bwr.Exchange.MultiTenancy;
using Bwr.Exchange.Settings.Countries;
using Bwr.Exchange.Settings.Treasuries;
using Bwr.Exchange.Settings.Currencies;
using Bwr.Exchange.Settings.Incomes;
using Bwr.Exchange.Settings.Expenses;
using Bwr.Exchange.Settings.Clients;
using Bwr.Exchange.Settings.Companies;
using Bwr.Exchange.Settings;
using Bwr.Exchange.Customers;
using Bwr.Exchange.Transfers;
using Bwr.Exchange.CashFlows.ClientCashFlows;
using Bwr.Exchange.CashFlows.CompanyCashFlows;
using Bwr.Exchange.CashFlows.TreasuryCashFlows;
using Bwr.Exchange.TreasuryActions;
using Bwr.Exchange.Transfers.IncomeTransfers;
using Bwr.Exchange.Settings.ExchangePrices;
using Bwr.Exchange.ExchangeCurrencies;
using Bwr.Exchange.CashFlows.CashFlowMatchings;
using Bwr.Exchange.Settings.GeneralSettings;
using Bwr.Exchange.Settings.Branches;
using Bwr.Exchange.CashFlows.ManagementStatement;

namespace Bwr.Exchange.EntityFrameworkCore
{
    public class ExchangeDbContext : AbpZeroDbContext<Tenant, Role, User, ExchangeDbContext>
    {
        /* Define a DbSet for each entity of the application */
        
        public ExchangeDbContext(DbContextOptions<ExchangeDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CashFlows.Transaction>()
                .HasKey(x => new { x.TransactionId, x.TransactionType });

        }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<CompanyBalance> CompanyBalances { get; set; }
        public virtual DbSet<Province> Provinces { get; set; }
        public virtual DbSet<Treasury> Treasuries { get; set; }
        public virtual DbSet<TreasuryBalance> TreasuryBalances { get; set; }
        public virtual DbSet<Currency> Currencies { get; set; }
        public virtual DbSet<Income> Incomes { get; set; }
        public virtual DbSet<Expense> Expenses { get; set; }
        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<ClientBalance> ClientBalances { get; set; }
        public virtual DbSet<ClientPhone> ClientPhones { get; set; }
        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<Commision> Commisions { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<CustomerImage> CustomerImages { get; set; }
        public virtual DbSet<OutgoingTransfer> OutgoingTransfers { get; set; }
        public virtual DbSet<IncomeTransfer> IncomeTransfers { get; set; }
        public virtual DbSet<IncomeTransferDetail> IncomeTransferDetails { get; set; }
        public virtual DbSet<ClientCashFlow> ClientCashFlows { get; set; }
        public virtual DbSet<CompanyCashFlow> CompanyCashFlows { get; set; }
        public virtual DbSet<TreasuryCashFlow> TreasuryCashFlows { get; set; }
        public virtual DbSet<CashFlowMatching> CashFlowMatchings { get; set; }
        public virtual DbSet<TreasuryAction> TreasuryActions { get; set; }
        public virtual DbSet<ExchangePrice> ExchangePrices { get; set; }
        public virtual DbSet<ExchangeCurrency> ExchangeCurrencies { get; set; }
        public virtual DbSet<GeneralSetting> GeneralSettings { get; set; }
        public virtual DbSet<ExchangeCurrencyHistory> ExchangeCurrencyHistories { get; set; }
        public virtual DbSet<Branch> Branches { get; set; }
        public virtual DbSet<Management> ManagementStatement { get; set; }
    }
}
