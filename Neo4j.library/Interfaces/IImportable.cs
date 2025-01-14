using System;
using System.Collections.Generic;
using System.Text;

namespace neo4j.lib.Interfaces
{
    /// <summary>
    /// Interface for classes that can be imported into Neo4j.
    /// </summary>
    /// <remarks>
    /// Example:
    /// <code>
    /// driver.WriteTransaction(tx => tx.Run(obj.ToCypherQuery(), obj.GetParameters()));
    /// </code>
    /// </remarks>
    public interface IImportable
    {
        /// <summary>
        /// Generates the Cypher query string for the object.
        /// </summary>
        /// <returns>A string representing the Cypher query.</returns>
        string ToCypherQuery();

        /// <summary>
        /// Retrieves the parameters for the Cypher query.
        /// </summary>
        /// <returns>An object representing the query parameters.</returns>
        object GetParameters();

        //object ResultToObject();
    }
}
