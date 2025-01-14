using Neo4j.Driver;
using System;
using System.Collections.Generic;
using neo4j.lib.Interfaces;
using System.Threading.Tasks;
using neo4j.lib.Classes;

namespace neo4j.lib
{
    public class DataImport : IDisposable
    {
        private readonly IDriver _driver;

        public DataImport(string uri, string user, string password)
        {
            _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(user, password));
        }

        public async Task ImportData(List<IImportable> import)
        {
            var counters = new Dictionary<string, int>
            {
                { "NodesCreated", 0 },
                { "RelationshipsCreated", 0 }
            };
            
            var session = _driver.AsyncSession();
            try
            {
                await session.ExecuteWriteAsync(
                async tx =>
                {
                    var results = new List<string>();
                    foreach (var importable in import)
                    {
                        var result = await tx.RunAsync(importable.ToCypherQuery(), importable.GetParameters());
                        var summary = await result.ConsumeAsync();


                        if (summary.Counters.NodesCreated > 0)
                        {
                            counters["NodesCreated"] += summary.Counters.NodesCreated;
                        }
                        if (summary.Counters.RelationshipsCreated > 0)
                        {
                            counters["RelationshipsCreated"] += summary.Counters.RelationshipsCreated;
                        }
                    }
                });
                if (counters.Count > 0) {
                    Console.WriteLine("Import results:");
                    foreach (var count in counters)
                    {
                        Console.WriteLine($"{count.Key}: {count.Value}");
                    }
                }
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
