using neo4j.lib.Interfaces;
using System;

namespace neo4j.lib.Classes
{
    public class UserAccessLevel : IImportable
    {
        public Int64 UserAccessLevelId { get; set; }
        public Guid UserId { get; set; }
        public Int64 AccessLevelId { get; set; }

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
