using Neo4j.library.Interfaces;
using System;

namespace Neo4j.library.Classes
{
    public class UserAccessLevel : IImportable
    {
        public long UserAccessLevelId { get; set; }
        public Guid UserId { get; set; }
        public long AccessLevelId { get; set; }

        public string ToCypherQuery()
        {
            return
                "MATCH (user:User {UserId: $userId}) " +
                "MATCH (accessLevel:AccessLevel {AccessLevelId: $accessLevelId}) " +
                "MERGE (user)-[r:HAS_ACCESS_LEVEL {UserAccessLevelId: $userAccessLevelId}]->(accessLevel) ";
        }
        public object GetParameters()
        {
            return new
            {
                userAccessLevelId = UserAccessLevelId,
                userId = UserId.ToString(),
                accessLevelId = AccessLevelId
            };
        }
    }
}
