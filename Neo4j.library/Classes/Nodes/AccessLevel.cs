using Neo4j.library.Classes.Base;

namespace Neo4j.library.Classes.Nodes
{
    public class AccessLevel : ImportableBase
    {
        public long AccessLevelId { get; set; }
        public string AccessLevelTitle { get; set; }

        public override string ToCypherQuery()
        {
            return $"{BuildBaseMergeQuery("AccessLevel", "AccessLevelId")} " +
                   $"{BuildSetClause("AccessLevelTitle")}";
        }

        public override string ToCypherBatchQuery()
        {
            return "UNWIND $batch as params " +
                   "MERGE (al:AccessLevel {AccessLevelId: params.AccessLevelId}) " +
                   "SET al.AccessLevelTitle = params.AccessLevelTitle ";
        }

        public override object GetParameters()
        {
            return new
            {
                AccessLevelId,
                AccessLevelTitle
            };
        }
    }
}
