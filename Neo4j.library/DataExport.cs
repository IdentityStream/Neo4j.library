using Neo4j.Driver;
using Neo4j.Driver.Mapping;
using Neo4j.library.Classes;
using Neo4j.library.Classes.Nodes;
using Neo4j.library.Classes.Relationships;
using Neo4j.library.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neo4j.library
{
    public class DataExport : IDisposable
    {
        private IDriver _driver;
        public DataExport(string uri, string user, string password)
        {
            _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(user, password));
        }
        public DataExport(string uri, string user, string password, Microsoft.Extensions.Logging.ILogger logger)
        {
            _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(user, password), o => o.WithLogger(new Neo4jLoggerAdapter(logger)));
            _driver.ExecutableQuery("Match (n) return n").ExecuteAsync();
        }

        public async Task<List<IImportable>> ExportData()
        {
            var result = new List<IImportable>();

            // Get all nodes first
            var nodes = await ExportNodes();
            result.AddRange(nodes);

            // Then get all relationships
            var relationships = await ExportRelationships();
            result.AddRange(relationships);

            return result;
        }

        private async Task<List<IImportable>> ExportNodes()
        {
            var query = "MATCH (n) RETURN DISTINCT n";
            var result = new List<IImportable>();
            var session = _driver.AsyncSession();

            try
            {
                var records = await session.RunAsync(query);

                while (await records.FetchAsync())
                {
                    var node = records.Current["n"].As<INode>();
                    if (node != null)
                    {
                        var nodeEntity = CreateNodeEntity(node);
                        if (nodeEntity != null)
                        {
                            result.Add(nodeEntity);
                        }
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

            return result;
        }

        private async Task<List<IImportable>> ExportRelationships()
        {
            var query = "MATCH (n)-[r]->(m) RETURN n, r, m";
            var result = new List<IImportable>();
            var session = _driver.AsyncSession();

            try
            {
                var records = await session.RunAsync(query);

                while (await records.FetchAsync())
                {
                    var relationship = records.Current["r"].As<IRelationship>();
                    var startNode = records.Current["n"].As<INode>();
                    var endNode = records.Current["m"].As<INode>();

                    if (relationship != null)
                    {
                        var relationshipEntity = CreateRelationshipEntity(relationship, startNode, endNode);
                        if (relationshipEntity != null)
                        {
                            result.Add(relationshipEntity);
                        }
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

            return result;
        }

        private IImportable CreateNodeEntity(INode node)
        {
            try
            {
                var label = node.Labels.FirstOrDefault();
                var props = node.Properties;

                if (label == "User")
                {
                    return new User
                    {
                        UserId = Guid.Parse(props["UserId"].ToString()),
                        UserName = props["UserName"].ToString()
                    };
                }

                if (label == "Role")
                {
                    return new Role
                    {
                        RoleId = long.Parse(props["RoleId"].ToString()),
                        RoleTitle = props["RoleTitle"].ToString()
                    };
                }

                if (label == "AccessLevel")
                {
                    return new AccessLevel
                    {
                        AccessLevelId = long.Parse(props["AccessLevelId"].ToString()),
                        AccessLevelTitle = props["AccessLevelTitle"].ToString()
                    };
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        private IImportable CreateRelationshipEntity(IRelationship relationship, INode startNode, INode endNode)
        {
            try
            {
                var props = relationship.Properties;
                var type = relationship.Type;

                if (type == "IS_MEMBER_OF")
                {
                    return new UserRole
                    {
                        UserRoleId = long.Parse(props["UserRoleId"].ToString()),
                        UserId = Guid.Parse(startNode.Properties["UserId"].ToString()),
                        RoleId = long.Parse(endNode.Properties["RoleId"].ToString())
                    };
                }

                if (type == "HAS_ACCESS_LEVEL")
                {
                    return new UserAccessLevel
                    {
                        UserAccessLevelId = long.Parse(props["UserAccessLevelId"].ToString()),
                        UserId = Guid.Parse(startNode.Properties["UserId"].ToString()),
                        AccessLevelId = long.Parse(endNode.Properties["AccessLevelId"].ToString())
                    };
                }

                if (type == "GRANTS_ACCESS_LEVEL")
                {
                    return new RoleAccessLevel
                    {
                        RoleAccessLevelId = long.Parse(props["RoleAccessLevelId"].ToString()),
                        RoleId = long.Parse(startNode.Properties["RoleId"].ToString()),
                        AccessLevelId = long.Parse(endNode.Properties["AccessLevelId"].ToString())
                    };
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        private T GetPropertyValue<T>(IReadOnlyDictionary<string, object> props, string key)
        {
            if (!props.ContainsKey(key))
            {
                throw new InvalidOperationException($"Required property {key} not found");
            }

            var value = props[key];
            if (value == null)
            {
                throw new InvalidOperationException($"Property {key} is null");
            }

            return (T)Convert.ChangeType(value.ToString(), typeof(T));
        }

        public void Dispose()
        {
            _driver?.Dispose();
        }
    }
}
