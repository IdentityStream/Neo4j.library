using neo4j.lib.Interfaces;
using Neo4j.Driver;
using Neo4j.Driver.Mapping;
using System;

namespace neo4j.lib.Classes
{
    public class Tenant : IImportable
    {
        public Int64 TenantId { get; set; }
        public string TenantName { get; set; }

        public string ToCypherQuery()
        {
            return
                "MERGE (u:Tenant {TenantId: $tenantId}) " +
                "ON CREATE " +
                "SET u.TenantName = $tenantName " +
                "ON MATCH " +
                "SET u.TenantName = $tenantName ";
        }
        public object GetParameters()
        {
            return new
            {
                tenantId = TenantId,
                tenantName = TenantName
            };
        }
    }

}
