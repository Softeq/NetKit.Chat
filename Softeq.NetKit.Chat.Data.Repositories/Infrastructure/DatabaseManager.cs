// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using SimpleMigrations;
using SimpleMigrations.DatabaseProvider;

namespace Softeq.NetKit.Chat.Data.Repositories.Infrastructure
{
    public class DatabaseManager : IDatabaseManager
    {
        private static readonly string MasterCatalog = "master";

        private readonly string _databaseName;
        private readonly string _sqlConnectionString;
        private readonly string _masterSqlConnectionString;

        public DatabaseManager(DatabaseConfig connectionSettings)
        {
            _sqlConnectionString = connectionSettings.ConnectionString;
            var masterSqlConnectionStringBuilder = new SqlConnectionStringBuilder(connectionSettings.ConnectionString);
            _databaseName = masterSqlConnectionStringBuilder.InitialCatalog;
            masterSqlConnectionStringBuilder.InitialCatalog = MasterCatalog;
            _masterSqlConnectionString = masterSqlConnectionStringBuilder.ConnectionString;
        }

        public async Task CreateEmptyDatabaseIfNotExistsAsync()
        {
            using (var connection = new SqlConnection(_masterSqlConnectionString))
            {
                connection.Open();
                var isDatabaseExists = await IsDatabaseExistsAsync(connection);
                if (!isDatabaseExists)
                {
                    await CreateDatabaseAsync(connection);
                }
            }
        }

        public void MigrateToLatestVersion()
        {
            using (var connection = new SqlConnection(_sqlConnectionString))
            {
                connection.Open();

                var migrationsAssembly = typeof(DatabaseManager).Assembly;
                var databaseProvider = new MssqlDatabaseProvider(connection);
                var migrator = new SimpleMigrator(migrationsAssembly, databaseProvider);
                migrator.Load();
                migrator.MigrateToLatest();
            }
        }

        private async Task<bool> IsDatabaseExistsAsync(SqlConnection connection)
        {
            var databaseId = await connection.ExecuteScalarAsync(@"SELECT database_id 
                                                                   FROM master.sys.databases 
                                                                   WHERE name = @dbName", new { dbName = _databaseName });
            return databaseId != null;
        }

        private async Task CreateDatabaseAsync(SqlConnection connection)
        {
            await connection.ExecuteScalarAsync($"CREATE DATABASE [{_databaseName}]");
        }
    }
}