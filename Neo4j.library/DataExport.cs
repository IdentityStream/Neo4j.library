using neo4j.lib.Interfaces;
using Neo4j.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Collections;
using neo4j.lib.Classes;
using Neo4j.Driver.Mapping;

namespace neo4j.lib
{
    public class DataExport : IDisposable
    {
        private IDriver _driver;
        public DataExport(string uri, string user, string password)
        {
            _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(user, password));
        }

        public async Task ExportData(string query)
        {
            var session = _driver.AsyncSession();
            try
            {
                await session.ExecuteReadAsync(
                async tx =>
                {
                    var results = await tx.RunAsync(query);
                    var records = await results.ToListAsync();
                });
            }
            finally
            {
                if (session != null)
                {
                    await session.DisposeAsync();
                }
            }
        }

        public async Task<List<IImportable>> ExportToIImportable(string query)
        {
            var result = new List<IImportable>();

            var session = _driver.AsyncSession();
            try
            {
                await session.ExecuteReadAsync(
                async tx =>
                {
                    var results = await tx.RunAsync(query);
                    var records = await results.ToListAsync();

                    foreach (var record in records)
                    {
                        if (record.GetType() == typeof(User))
                        {
                            result.Add(record.AsObject<User>());
                        }


                        result.Add(record.As<IImportable>());
                    }
                });
            }
            finally
            {
                if (session != null)
                {
                    await session.DisposeAsync();
                }
            }

            return result;
        }


        public void Dispose()
        {
            _driver?.Dispose();
        }
    }
}
