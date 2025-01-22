using Neo4j.library.Classes.Base;
using System;

namespace Neo4j.library.Classes.Nodes
{
    public class User : ImportableBase
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }

        public override string ToCypherQuery()
        {
            return $"{BuildBaseMergeQuery("User", "UserId")} " +
                   $"{BuildSetClause("UserTitle")}";
        }

        public override string ToCypherBatchQuery()
        {
            return "UNWIND $batch as params " +
                   "MERGE (u:User {UserId: params.UserId}) " +
                   "SET u.UserName = params.UserName ";
        }

        public override object GetParameters()
        {
            return new
            {
                UserId = UserId.ToString(),
                UserName
            };
        }
    }
}
