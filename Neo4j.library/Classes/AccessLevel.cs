using neo4j.lib.Interfaces;
using System;

namespace neo4j.lib.Classes
{
    public class AccessLevel : IImportable
    {
        public Int64 AccessLevelId { get; set; }
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
