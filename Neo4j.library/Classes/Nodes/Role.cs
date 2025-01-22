using Neo4j.library.Classes.Base;

namespace Neo4j.library.Classes.Nodes
{
    public class Role : ImportableBase
    {
        public long RoleId { get; set; }
        public string RoleTitle { get; set; }

        public override string ToCypherQuery()
        {
            return $"{BuildBaseMergeQuery("Role", "RoleId")} " +
                   $"{BuildSetClause("RoleTitle")}";
        }

        public override string ToCypherBatchQuery()
        {
            return "UNWIND $batch as params " +
                   "MERGE (r:Role {RoleId: params.RoleId}) " +
                   "SET r.RoleTitle = params.RoleTitle ";
        }

        public override object GetParameters()
        {
            return new
            {
                RoleId,
                RoleTitle
            };
        }
    }
}
