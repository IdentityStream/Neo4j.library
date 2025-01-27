using Neo4j.library.Classes.Base;
using System.Collections.Generic;

namespace Neo4j.library.Classes.Nodes
{
    public class Role : ImportableBase
    {
        public long RoleId { get; set; }
        public string RoleTitle { get; set; }

        public override string ToCypherQuery()
        {
            return "MERGE (r:Role {RoleId: $RoleId}) " +
                   "SET r += $parameters";
        }

        public override string ToCypherBatchQuery()
        {
            return "UNWIND $batch as params " +
                   "MERGE (r:Role {RoleId: params.RoleId}) " +
                   "SET r += params.parameters ";
        }

        public override object GetParameters()
        {
            var parameterDict = new Dictionary<string, object>(Parameters)
            {
                { "RoleTitle", RoleTitle },
            };
            return new
            {
                RoleId,
                parameters = parameterDict
            };
        }
    }
}
