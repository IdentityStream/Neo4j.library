using Microsoft.Extensions.Logging;
using Neo4j.Driver;
using Neo4j.library.Classes;
using Neo4j.library.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
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

        public class BatchResult
        {
            public bool Success { get; set; }
            public int BatchSize { get; set; }
            public Exception Exception { get; set; }
            public List<object> FailedParameters { get; set; }
            public string FailedQuery { get; set; }
        }

        public class FailedBatch
        {
            public string GroupKey { get; set; }
            public string Query { get; set; }
            public List<object> Parameters { get; set; }
            public string ErrorMessage { get; set; }
            public DateTime Timestamp { get; set; }
        }
        public class ImportSummary
        {
            public string GroupName { get; set; }
            public int TotalProcessed { get; set; }
            public int SuccessfulItems { get; set; }
            public int FailedItems { get; set; }
            public TimeSpan Duration { get; set; }
            public double ItemsPerSecond { get; set; }
        }
        public async Task ImportBatchBetterAsync<T>(IEnumerable<T> entities, int initialBatchSize = 500) where T : IImportable
        {
            if (!entities.Any()) return;

            var failedBatches = new List<FailedBatch>();
            var summaries = new List<ImportSummary>();
            var groupedItems = entities
                .GroupBy(e => e.GetType().Name)
                .ToDictionary(group => group.Key, group => group.ToList());

            DateTime importStartTime = DateTime.Now;
            IAsyncSession session = _driver.AsyncSession();
            try
            {
                foreach (var group in groupedItems)
                {
                    try
                    {
                        _logger.LogInformation($"Starting import for group: {group.Key}");
                        var items = group.Value.ToList();
                        var totalItemsInGroup = items.Count;

                        int currentBatchSize = initialBatchSize;
                        int maxBatchSize = initialBatchSize * 8;
                        int lastProgressLog = 0;
                        int successfulItems = 0;

                        int currentIndex = 0;
                        DateTime groupStartTime = DateTime.Now;
                        DateTime lastLogTime = DateTime.Now;

                        while (currentIndex < items.Count)
                        {
                            var batchItems = items.Skip(currentIndex).Take(currentBatchSize).ToList();
                            var batchResult = await ProcessBatchWithSizeAsync(session, batchItems);

                            currentIndex += Math.Min(currentBatchSize, batchItems.Count);
                            if (batchResult.Success)
                            {
                                successfulItems += batchItems.Count;
                                int currentProgress = (int)((double)currentIndex / totalItemsInGroup * 100);
                                if (currentProgress >= lastProgressLog + 25 || (DateTime.Now - lastLogTime).TotalSeconds >= 15)
                                {
                                    TimeSpan elapsed = DateTime.Now - groupStartTime;

                                    _logger.LogInformation(
                                        string.Format("{0}: {1}% complete ({2:N0}/{3:N0} items)",
                                            group.Key,
                                            currentProgress,
                                            currentIndex,
                                            totalItemsInGroup));

                                    lastProgressLog = (currentProgress * 5) / 5;
                                    lastLogTime = DateTime.Now;
                                }
                            }
                            else
                            {
                                // Store the failed items
                                failedBatches.Add(new FailedBatch
                                {
                                    GroupKey = group.Key,
                                    Query = batchResult.FailedQuery,
                                    Parameters = batchResult.FailedParameters ?? new List<object>(),
                                    ErrorMessage = batchResult.Exception != null ? batchResult.Exception.Message : "Unknown error",
                                    Timestamp = DateTime.Now
                                });
                            }
                        }

                        _logger.LogInformation($"Completed import for group: {group.Key}");

                        TimeSpan groupDuration = DateTime.Now - groupStartTime;
                        double avgItemsPerSecond = successfulItems / Math.Max(1, groupDuration.TotalSeconds);

                        summaries.Add(new ImportSummary
                        {
                            GroupName = group.Key,
                            TotalProcessed = totalItemsInGroup,
                            SuccessfulItems = successfulItems,
                            FailedItems = totalItemsInGroup - successfulItems,
                            Duration = groupDuration,
                            ItemsPerSecond = avgItemsPerSecond
                        });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Batch import error: {ex.Message}");
                    }
                }
            }
            finally
            {
                if (session != null)
                {
                    await session.CloseAsync();
                }
            }


            // Log the summary
            TimeSpan totalDuration = DateTime.Now - importStartTime;
            _logger.LogInformation("\n=== Import Summary ===");
            _logger.LogInformation($"Total Duration: {totalDuration.Minutes}m {totalDuration.Seconds}s");

            foreach (var summary in summaries)
            {
                var successRate = (double)summary.SuccessfulItems / summary.TotalProcessed * 100;
                _logger.LogInformation(
                    $"\n{summary.GroupName}:" +
                    $"\n  Total Items: {summary.TotalProcessed:N0}" +
                    $"\n  Successful: {summary.SuccessfulItems:N0} ({successRate:F1}%)" +
                    $"\n  Failed: {summary.FailedItems:N0}" +
                    $"\n  Duration: {summary.Duration.Minutes}m {summary.Duration.Seconds}s" +
                    $"\n  Speed: {summary.ItemsPerSecond:F1} items/sec");
            }

            var totalItems = summaries.Sum(s => s.TotalProcessed);
            var totalSuccessful = summaries.Sum(s => s.SuccessfulItems);
            var totalFailed = summaries.Sum(s => s.FailedItems);
            var overallSuccessRate = (double)totalSuccessful / totalItems * 100;

            _logger.LogInformation(
                $"\nOverall Statistics:" +
                $"\n  Total Items Processed: {totalItems:N0}" +
                $"\n  Total Successful: {totalSuccessful:N0} ({overallSuccessRate:F1}%)" +
                $"\n  Total Failed: {totalFailed:N0}" +
                $"\n  Average Speed: {totalItems / Math.Max(1, totalDuration.TotalSeconds):F1} items/sec");

            // Save the failed batches to a file
            if (failedBatches.Any())
            {
                try
                {
                    var jsonSettings = new Newtonsoft.Json.JsonSerializerSettings
                    {
                        Formatting = Newtonsoft.Json.Formatting.Indented
                    };
                    var failedBatchesJson = Newtonsoft.Json.JsonConvert.SerializeObject(failedBatches, jsonSettings);
                    var filename = string.Format("failed_batches_{0}.json",
                        DateTime.Now.ToString("yyyyMMdd_HHmmss"));

                    File.WriteAllText(filename, failedBatchesJson);
                    _logger.LogWarning($"Saved {failedBatches.Count} failed batches to {filename}");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error saving failed batches: {ex.Message}");
                }
            }
        }

        private async Task<BatchResult> ProcessBatchWithSizeAsync<T>(
            IAsyncSession session,
            List<T> batchItems) where T : IImportable
        {
            try
            {
                var query = batchItems[0].ToCypherBatchQuery();
                var parameters = batchItems.ConvertAll(e => e.GetParameters());

                await session.ExecuteWriteAsync(async tx =>
                {
                    await tx.RunAsync(query, new { batch = parameters });
                });

                return new BatchResult
                {
                    Success = true,
                    BatchSize = batchItems.Count
                };
            }
            catch (Exception ex)
            {
                return new BatchResult
                {
                    Success = false,
                    BatchSize = batchItems.Count,
                    Exception = ex,
                    FailedQuery = batchItems[0].ToCypherBatchQuery(),
                    FailedParameters = batchItems.ConvertAll(e => e.GetParameters())
                };
            }
        }

        public void Dispose()
        {
            _driver?.Dispose();
        }
    }
}
