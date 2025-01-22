using Neo4j.library.Classes.Base;

namespace Neo4j.library.Classes.Relationships
{
    public class RoleAccessLevel : ImportableBase
    {
        public long RoleAccessLevelId { get; set; }
        public long RoleId { get; set; }
        public long AccessLevelId { get; set; }

        public override string ToCypherQuery()
        {
            return
                "MATCH (role:Role {RoleId: $roleId}) " +
                "MATCH (accessLevel:AccessLevel {AccessLevelId: $accessLevelId}) " +
                "MERGE (role)-[r:GRANTS_ACCESS_LEVEL {RoleAccessLevelId: $roleAccessLevelId}]->(accessLevel) ";
        }
        public override object GetParameters()
        {
            return new
            {
                roleAccessLevelId = RoleAccessLevelId,
                roleId = RoleId,
                accessLevelId = AccessLevelId
            };
        }
        public override string ToCypherBatchQuery()
        {
            return
                "UNWIND $batch AS params " +
                "MATCH (role:Role {RoleId: params.roleId}) " +
                "MATCH (accessLevel:AccessLevel {AccessLevelId: params.accessLevelId}) " +
                "MERGE (role)-[r:GRANTS_ACCESS_LEVEL {RoleAccessLevelId: params.roleAccessLevelId}]->(accessLevel) ";
        }
    }
}
