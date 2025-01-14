using neo4j.lib.Interfaces;
using System;

namespace neo4j.lib.Classes
{
    public class RoleAccessLevel : IImportable
    {
        public Int64 RoleAccessLevelId { get; set; }
        public Int64 RoleId { get; set; }
        public Int64 AccessLevelId { get; set; }

        public string ToCypherQuery()
        {
            return
                "MATCH (role:Role {RoleId: $roleId}) " +
                "MATCH (accessLevel:AccessLevel {AccessLevelId: $accessLevelId}) " +
                "MERGE (role)-[r:GRANTS_ACCESS_LEVEL {RoleAccessLevelId: $roleAccessLevelId}]->(accessLevel) ";
        }
        public object GetParameters()
        {
            return new
            {
                roleAccessLevelId = RoleAccessLevelId,
                roleId = RoleId,
                accessLevelId = AccessLevelId
            };
        }
    }
}
