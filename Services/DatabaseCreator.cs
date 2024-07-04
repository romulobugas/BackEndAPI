using Npgsql;
using Microsoft.Extensions.Configuration;

namespace BackEndAPI.Services
{
    public class DatabaseCreator
    {
        private readonly IConfiguration _configuration;

        public DatabaseCreator(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void EnsureDatabaseExists()
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            // Remover o nome do banco de dados da string de conexão
            var masterConnectionString = new NpgsqlConnectionStringBuilder(connectionString)
            {
                Database = "postgres" // Conecta ao banco de dados padrão do PostgreSQL
            }.ConnectionString;

            using (var connection = new NpgsqlConnection(masterConnectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT 1 FROM pg_database WHERE datname = 'GestDB'";
                    var exists = command.ExecuteScalar() != null;

                    if (!exists)
                    {
                        command.CommandText = "CREATE DATABASE \"GestDB\"";
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
