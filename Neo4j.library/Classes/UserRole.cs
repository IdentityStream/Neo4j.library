using Neo4j.Driver;
using Neo4j.library.Interfaces;
using System;

namespace Neo4j.library.Classes
{
    public class UserRole : IImportable
    {
        public long UserRoleId { get; set; }
        public Guid UserId { get; set; }
        public long RoleId { get; set; }

        public string ToCypherQuery()
        {
            const string queryString =
                "MATCH(user:User {UserId: $userId}) " +
                "MATCH(role:Role {RoleId: $roleId}) " +
                "MERGE(user)-[r:IS_MEMBER_OF {UserRoleId: $userRoleId}]->(role) ";

            return queryString;
        }
        public object GetParameters()
        {
            return new
            {
                userRoleId = UserRoleId,
                userId = UserId.ToString(),
                roleId = RoleId
            };
        }

        public UserRole ResultToObject(IRecord result)
        {
            return result.As<UserRole>();
        }
    }
}
