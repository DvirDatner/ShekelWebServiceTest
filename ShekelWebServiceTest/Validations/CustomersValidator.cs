using Dapper;
using System.Data;

namespace ShekelWebServiceTest
{
    public class CustomersValidator
    {
        public static async Task<bool> IsNewCustomerValid(IDbConnection connection, NewCustomer customer)
        {
            if (customer == null || string.IsNullOrEmpty(customer.CustomerId))
                return false;

            // checks if there is no customer with same id
            var exists = await connection.ExecuteScalarAsync<bool>("select count(1) from Customers where customerId = @CustomerId",
                new { customer.CustomerId });
            if (exists)
                return false;

            // checks if there is group with the given group code
            exists = await connection.ExecuteScalarAsync<bool>("select count(1) from Groups where groupCode = @GroupCode",
                new {customer.GroupCode});
            if (!exists)
                return false;

            // checks if there is factory with the given factory code
            exists = await connection.ExecuteScalarAsync<bool>("select count(1) from Factories where factoryCode = @FactoryCode",
                new { customer.FactoryCode });
            if (!exists)
                return false;

            return true;
        }
    }
}
