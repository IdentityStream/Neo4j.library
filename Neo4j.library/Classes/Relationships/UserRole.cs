using Neo4j.library.Classes.Base;
using System;

namespace Neo4j.library.Classes.Relationships
{
    public class UserRole : ImportableBase
    {
        public long UserRoleId { get; set; }
        public Guid UserId { get; set; }
        public long RoleId { get; set; }

        public override string ToCypherQuery()
        {
            return
                "MATCH(user:User {UserId: $userId}) " +
                "MATCH(role:Role {RoleId: $roleId}) " +
                "MERGE(user)-[r:IS_MEMBER_OF {UserRoleId: $userRoleId}]->(role) " +
                "SET r += params.parameters ";

        }
        public override string ToCypherBatchQuery()
        {
            return
                "UNWIND $batch AS params " +
                "MATCH(user:User {UserId: params.userId}) " +
                "MATCH(role:Role {RoleId: params.roleId}) " +
                "MERGE(user)-[r:IS_MEMBER_OF {UserRoleId: params.userRoleId}]->(role) " +
                "SET r += params.parameters ";
        }
        public override object GetParameters()
        {
            return new
            {
                userRoleId = UserRoleId,
                userId = UserId.ToString(),
                roleId = RoleId,
                parameters = Parameters
            };
        }
    }
}
