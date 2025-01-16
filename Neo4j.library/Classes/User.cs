using Neo4j.library.Interfaces;
using System;

namespace Neo4j.library.Classes
{
    public class User : IImportable
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }

        public string ToCypherQuery()
        {
            return
                "MERGE (u:User {UserId: $userId}) " +
                "ON CREATE " +
                "SET u.UserName = $userName " +
                "ON MATCH " +
                "SET u.UserName = $userName ";
        }
        public object GetParameters()
        {
            return new
            {
                userId = UserId.ToString(),
                userName = UserName
            };
        }
        //public User ResultToObject(IRecord result)
        //{
        //    return new User
        //    {
        //        UserId = Guid.Parse(result.UserId),
        //        UserName = result.UserName
        //    };
        //}
    }

}
