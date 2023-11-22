using Microsoft.Data.SqlClient;
using System.Data;

namespace PokemonAPI.Data
{
    public class DapperContext
    {
        private readonly string _connectionString;
        public DapperContext(string connectionString) => _connectionString = connectionString;

        public IDbConnection OpenDbConnection() => new SqlConnection(this._connectionString);

        public string GetDbConnectionString()
        {
            return this._connectionString;
        }
    }
}
