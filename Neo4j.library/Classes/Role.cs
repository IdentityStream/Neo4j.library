using Neo4j.library.Interfaces;

namespace Neo4j.library.Classes
{
    public class Role : IImportable
    {
        public long RoleId { get; set; }
        public string RoleTitle { get; set; }

        public string ToCypherQuery()
        {
            return
                "MERGE (r:Role {RoleId: $roleId}) " +
                "ON CREATE " +
                "SET r.RoleTitle = $roleTitle " +
                "ON MATCH " +
                "SET r.RoleTitle = $roleTitle ";
        }
        public object GetParameters()
        {
            return new
            {
                roleId = RoleId,
                roleTitle = RoleTitle
            };
        }
    }
}
