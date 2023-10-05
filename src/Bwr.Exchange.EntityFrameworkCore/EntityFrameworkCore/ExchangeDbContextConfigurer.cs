using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace Bwr.Exchange.EntityFrameworkCore
{
    public static class ExchangeDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<ExchangeDbContext> builder, string connectionString)
        {
            var serverVersion = ServerVersion.AutoDetect(connectionString);
            builder.UseMySql(connectionString, serverVersion);
        }

        public static void Configure(DbContextOptionsBuilder<ExchangeDbContext> builder, DbConnection connection)
        {
            var serverVersion = ServerVersion.AutoDetect(connection.ConnectionString);
            builder.UseMySql(connection, serverVersion);
        }

        //public static void Configure(DbContextOptionsBuilder<ExchangeDbContext> builder, string connectionString)
        //{
        //    builder.UseSqlServer(connectionString);
        //}

        //public static void Configure(DbContextOptionsBuilder<ExchangeDbContext> builder, DbConnection connection)
        //{
        //    builder.UseSqlServer(connection);
        //}
    }
}
