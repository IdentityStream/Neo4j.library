using Neo4j.library.Classes.Base;
using System.Collections.Generic;

namespace Neo4j.library.Classes.Nodes
{
    public class AccessLevel : ImportableBase
    {
        public long AccessLevelId { get; set; }
        public string AccessLevelTitle { get; set; }

        public override string ToCypherQuery()
        {
            return "MERGE (al:AccessLevel {AccessLevelId: $AccessLevelId}) " +
                   "SET al += $parameters";
        }

        public override string ToCypherBatchQuery()
        {
            return "UNWIND $batch as params " +
                   "MERGE (al:AccessLevel {AccessLevelId: params.AccessLevelId}) " +
                   "SET al += params.parameters ";
        }

        public override object GetParameters()
        {
            var parameterDict = new Dictionary<string, object>(Parameters)
            {
                { "AccessLevelTitle", AccessLevelTitle },
            };
            return new
            {
                AccessLevelId,
                parameters = parameterDict
            };
        }
    }
}
