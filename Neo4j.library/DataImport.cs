using Neo4j.Driver;
using Neo4j.library.Classes;
using Neo4j.library.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neo4j.library
{
    public class DataImport : IDisposable
    {
        private readonly IDriver _driver;

        public DataImport(string uri, string user, string password)
        {
            _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(user, password));
        }
        public DataImport(string uri, string user, string password, Microsoft.Extensions.Logging.ILogger logger)
        {
            _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(user, password), o => o.WithLogger(new Neo4jLoggerAdapter(logger)));
        }

        public async Task ImportSingleAsync<T>(T entity) where T : IImportable
        {
            var query = entity.ToCypherQuery();
            var parameters = entity.GetParameters();

            var session = _driver.AsyncSession();
            try
            {
                await session.RunAsync(query, parameters);
            }
            finally
            {
                if (session != null)
                {
                    await session.DisposeAsync();
                }
            }
        }

        public async Task ImportBatchAsync<T>(IEnumerable<T> entities) where T : IImportable
        {
            if (!entities.Any()) return;

            var query = entities.First().ToCypherBatchQuery();
            var parameters = new { batch = entities.Select(e => e.GetParameters()) };

            var session = _driver.AsyncSession();
            try
            {
                await session.RunAsync(query, parameters);
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
