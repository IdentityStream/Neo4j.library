using Neo4j.library.Interfaces;

namespace Neo4j.library.Classes
{
    public class AccessLevel : IImportable
    {
        public long AccessLevelId { get; set; }
        public string AccessLevelTitle { get; set; }

        public string ToCypherQuery()
        {
            return
                "MERGE (al:AccessLevel {AccessLevelId: $accessLevelId}) " +
                "ON CREATE " +
                "SET al.AccessLevelTitle = $accessLevelTitle " +
                "ON MATCH " +
                "SET al.AccessLevelTitle = $accessLevelTitle ";
        }
        public object GetParameters()
        {
            return new
            {
                accessLevelId = AccessLevelId,
                accessLevelTitle = AccessLevelTitle
            };
        }
    }
}
