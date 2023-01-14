using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Linq;

namespace ShekelWebServiceTest
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly ConnectionManager _connectionManager;
        public GroupController(IConfiguration config)
        {
            _connectionManager = new ConnectionManager(config);
        }

        [HttpGet]
        public async Task<ActionResult<List<GroupAndCustomers>>> GetGroupsAndCustomers()
        {
            List<GroupAndCustomers> groupsAndCustomers = new List<GroupAndCustomers>();

            using var connection = _connectionManager.GetConnection();

            IEnumerable<GroupDB> groups = await SelectAllGroups(connection);

            foreach (var group in groups)
            {
                var groupAndCustomers = new GroupAndCustomers(group);

                groupAndCustomers.Customers = await SelectAllCustomersForGroup(connection, group.GroupCode);

                groupsAndCustomers.Add(groupAndCustomers);
            }

            return Ok(groupsAndCustomers);
        }

        private static async Task<IEnumerable<GroupDB>> SelectAllGroups(IDbConnection connection)
        {
            return await connection.QueryAsync<GroupDB>("select * from Groups");
        }

        private static async Task<IEnumerable<GroupCustomer>> SelectAllCustomersForGroup(IDbConnection connection, int groupCode)
        {
            return await connection.QueryAsync<GroupCustomer>("select Customers.customerId, Customers.name from Customers left join FactoriesToCustomer on Customers.customerId = FactoriesToCustomer.customerId where FactoriesToCustomer.groupCode = @GroupCode", 
                new { GroupCode = groupCode });
        }
    }
}
