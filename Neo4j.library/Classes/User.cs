using neo4j.lib.Interfaces;
using Neo4j.Driver;
using Neo4j.Driver.Mapping;
using System;

namespace neo4j.lib.Classes
{
    public class User : IImportable
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public Int64 TenantId { get; set; }

        public string ToCypherQuery()
        {
            return
                "MERGE (u:User {UserId: $userId}) " +
                "ON CREATE " +
                "SET u.UserName = $userName " +
                ", u.TenantId = $tenantId " +
                "ON MATCH " +
                "SET u.UserName = $userName " +
                ", u.TenantId = $tenantId ";
        }
        public object GetParameters()
        {
            return new
            {
                userId = UserId.ToString(),
                userName = UserName,
                tenantId = TenantId,
            };
        }
        //public User ResultToObject(IRecord result)
        //{
        //    return new User
        //    {
        //        UserId = Guid.Parse(result.UserId),
        //        UserName = result.UserName
        //    };
        //}
    }

}
