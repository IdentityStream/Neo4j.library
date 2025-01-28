using Neo4j.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Neo4j.library
{
    public class InitializeDB
    {

        private readonly IDriver _driver;


        public InitializeDB(string uri, string user, string password)
        {
            _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(user, password));
        }

        public async Task CreateConstraints()
        {
            var session = _driver.AsyncSession();
            try
            {
                await session.RunAsync("CREATE CONSTRAINT unique_userId FOR (u:User) REQUIRE u.UserId IS UNIQUE");
                await session.RunAsync("CREATE CONSTRAINT unique_roleId FOR (r:Role) REQUIRE r.RoleId IS UNIQUE");
                await session.RunAsync("CREATE CONSTRAINT unique_accessLevelId FOR (aL:AccessLevel) REQUIRE aL.AccessLevelId IS UNIQUE");
            }
            finally
            {
                if (session != null)
                {
                    await session.DisposeAsync();
                }
            }
        }
    }
}
