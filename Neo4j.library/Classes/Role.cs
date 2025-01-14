using neo4j.lib.Interfaces;
using System;

namespace neo4j.lib.Classes
{
    public class Role : IImportable
    {
        public Int64 RoleId { get; set; }
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
