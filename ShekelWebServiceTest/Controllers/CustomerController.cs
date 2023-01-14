using Dapper;
using Dapper.Transaction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Transactions;

namespace ShekelWebServiceTest
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ConnectionManager _connectionManager;
        public CustomerController(IConfiguration config) 
        {
            _connectionManager = new ConnectionManager(config);
        }

        [HttpPost]
        public async Task<ActionResult<CustomerDB>> CreateCustomer(NewCustomer customer)
        {
            using var connection = _connectionManager.GetConnection();

            try
            {

                if (!await CustomersValidator.IsNewCustomerValid(connection, customer))
                {
                    return BadRequest("Validation Error");
                }

                using var transaction = connection.BeginTransaction();

                int rowsAffected = await transaction.ExecuteAsync("insert into Customers (customerId, name, address, phone) values (@CustomerId, @Name, @Address, @Phone)", customer);
                if (rowsAffected == 0)
                {
                    transaction.Rollback();
                    return StatusCode(500, "Error in save new customer");
                }

                rowsAffected = await transaction.ExecuteAsync("insert into FactoriesToCustomer (groupCode, factoryCode, customerId) values (@GroupCode, @FactoryCode, @CustomerId)", customer);

                if (rowsAffected == 0)
                {
                    transaction.Rollback();
                    return StatusCode(500, "Error in save new customer");
                }

                transaction.Commit();
                return Ok(await SelectAllHeroes(connection));
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        private static async Task<IEnumerable<CustomerDB>> SelectAllHeroes(IDbConnection connection)
        {
            return await connection.QueryAsync<CustomerDB>("select * from Customers");
        }
    }
}
