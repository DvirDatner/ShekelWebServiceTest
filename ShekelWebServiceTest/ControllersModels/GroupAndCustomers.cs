namespace ShekelWebServiceTest
{
    public class GroupAndCustomers : GroupDB
    {
        public IEnumerable<GroupCustomer>? Customers { get; set; }

        public GroupAndCustomers(GroupDB group)
        {
            GroupCode = group.GroupCode;
            GroupName = group.GroupName;
        }
    }

    public class GroupCustomer
    {
        public string CustomerId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}
