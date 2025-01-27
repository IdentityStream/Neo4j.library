using System.Collections.Generic;

namespace Neo4j.library.Interfaces
{
    public interface IImportable
    {
        Dictionary<string, object> Parameters { get; set; }
        string ToCypherQuery();
        string ToCypherBatchQuery();
        object GetParameters();
    }
}
