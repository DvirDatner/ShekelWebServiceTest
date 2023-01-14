using System.Data;
using System.Data.SqlClient;

namespace ShekelWebServiceTest
{
    public class ConnectionManager
    {
        private readonly IConfiguration _config;
        
        public ConnectionManager(IConfiguration config)
        {
            _config = config;
        }

        public IDbConnection GetConnection()
        {
            var connection = new SqlConnection(_config.GetConnectionString("SqlConnection"));
            connection.Open();
            return connection;
        }
    }
}
