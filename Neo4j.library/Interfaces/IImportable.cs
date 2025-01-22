namespace Neo4j.library.Interfaces
{
    public interface IImportable
    {
        string ToCypherQuery();
        string ToCypherBatchQuery();
        object GetParameters();
    }
}
