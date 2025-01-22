using Neo4j.library.Classes.Base;
using System;

namespace Neo4j.library.Classes.Relationships
{
    public class UserAccessLevel : ImportableBase
    {
        public long UserAccessLevelId { get; set; }
        public Guid UserId { get; set; }
        public long AccessLevelId { get; set; }

        public override string ToCypherQuery()
        {
            return
                "MATCH (user:User {UserId: $userId} " +
                "MATCH (accessLevel:AccessLevel {AccessLevelId: $accessLevelId}) " +
                "MERGE (user)-[r:HAS_ACCESS_LEVEL {UserAccessLevelId: $userAccessLevelId}]->(accessLevel) ";
        }

        public override string ToCypherBatchQuery()
        {
            return
                "UNWIND $batch AS params " +
                "MATCH (user:User {UserId: params.userId}) " +
                "MATCH (accessLevel:AccessLevel {AccessLevelId: params.accessLevelId}) " +
                "MERGE (user)-[r:HAS_ACCESS_LEVEL {UserAccessLevelId: params.userAccessLevelId}]->(accessLevel) ";
        }

        public override object GetParameters()
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
