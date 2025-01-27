using Neo4j.library.Classes.Base;
using System;
using System.Collections.Generic;

namespace Neo4j.library.Classes.Nodes
{
    public class User : ImportableBase
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }

        public override string ToCypherQuery()
        {
            return "MERGE (u:User {UserId: $UserId}) " +
                   "SET u += $parameters";
        }

        public override string ToCypherBatchQuery()
        {
            return "UNWIND $batch as params " +
                   "MERGE (u:User {UserId: params.UserId}) " +
                   "SET u += params.parameters ";
        }

        public override object GetParameters()
        {
            var parameterDict = new Dictionary<string, object>(Parameters)
            {
                { "UserName", UserName.ToString() },
            };
            return new
            {
                UserId = UserId.ToString(),
                parameters = parameterDict
            };
        }
    }
}
