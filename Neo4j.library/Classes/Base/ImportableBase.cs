using Neo4j.library.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Neo4j.library.Classes.Base
{
    public abstract class ImportableBase : IImportable
    {
        public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();
        protected string BuildBaseMatchQuery(string label, string idField)
        {
            return $"MATCH (n:{label} {{{idField}: $params.{idField}}})";
        }

        protected string BuildBaseMergeQuery(string label, string idField)
        {
            return $"MERGE (n:{label} {{{idField}: $params.{idField}}})";
        }

        protected string BuildSetClause(params string[] properties)
        {
            var setStatements = properties.Select(prop => $"n.{prop} = $params.{prop}");
            return $"SET {string.Join(", ", setStatements)}";
        }

        public abstract string ToCypherQuery();
        public abstract string ToCypherBatchQuery();
        public abstract object GetParameters();
    }
}
