using Neo4j.library.Interfaces;

namespace Neo4j.library.Classes
{
    public class RoleAccessLevel : IImportable
    {
        public long RoleAccessLevelId { get; set; }
        public long RoleId { get; set; }
        public long AccessLevelId { get; set; }

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
