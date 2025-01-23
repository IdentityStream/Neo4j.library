using Microsoft.Extensions.Logging;
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
        private readonly Microsoft.Extensions.Logging.ILogger _logger;

        public DataImport(string uri, string user, string password)
        {
            _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(user, password));
        }
        public DataImport(string uri, string user, string password, Microsoft.Extensions.Logging.ILogger logger)
        {
            _logger = logger;
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

        public async Task ImportBatchBetterAsync<T>(IEnumerable<T> entities, int batchSize = 500) where T : IImportable
        {
            if (!entities.Any()) return;

            var groupedItems = entities
               .GroupBy(e => e.GetType().Name)
               .ToDictionary(group => group.Key, group => group.ToList());

            foreach (var group in groupedItems)
            {
                _logger.LogInformation($"Starting import for group: {group.Key}");

                for (int i = 0; i < group.Value.Count; i += batchSize)
                {
                    var batchItems = group.Value.Skip(i).Take(batchSize).ToList();
                    int batchNumber = (i / batchSize) + 1;

                    _logger.LogInformation($"Importing {group.Key} - Batch {batchNumber}/{Math.Ceiling((double)group.Value.Count / batchSize)}");

                    var session = _driver.AsyncSession();
                    try
                    {
                        var query = batchItems[0].ToCypherBatchQuery();
                        var parameters = new { batch = batchItems.Select(e => e.GetParameters()) };
                        await session.RunAsync(query, parameters);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Batch import error: {ex.Message}");
                    }
                    finally
                    {
                        if (session != null)
                        {
                            await session.DisposeAsync();
                        }
                    }
                }

                _logger.LogInformation($"Completed import for group: {group.Key}");
            }
        }

        public void Dispose()
        {
            _driver?.Dispose();
        }
    }
}
