using Neo4j.Driver;
using System;
using System.Threading.Tasks;

namespace Neo4j.library
{
    public class HelloWorldExample : IDisposable
    {
        private readonly IDriver _driver;

        public HelloWorldExample(string uri, string user, string password)
        {
            _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(user, password));
        }

        public async Task PrintGreetingAsync(string message)
        {
            var session = _driver.AsyncSession();
            try
            {
                var greeting = await session.ExecuteWriteAsync(
                    async tx =>
                    {
                        var result = await tx.RunAsync(
                            "MERGE (a:Greeting) " +
                            "SET a.message = $message " +
                            "RETURN a.message + ', from node ' + id(a)",
                            new { message });

                        var record = await result.SingleAsync();
                        return record[0].As<string>();
                    });

                Console.WriteLine(greeting);
            }
            finally
            {
                if (session != null)
                {
                    await session.DisposeAsync();
                }
            }
        }

        public void Dispose()
        {
            _driver?.Dispose();
        }
    }
}