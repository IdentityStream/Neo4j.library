using Microsoft.Extensions.Logging;
using neo4j.console;
using Neo4j.library;
using Neo4j.library.Interfaces;
using Newtonsoft.Json.Linq;

var credentials = JObject.Parse(File.ReadAllText("Credentials.json"));
if (credentials == null)
{
    Console.WriteLine("Credentials file not found.");
    return;
}
var auraUri = credentials["uri"].ToString();
var user = credentials["username"].ToString();
var password = credentials["password"].ToString();

var testData = TestDataReader.GetTestData();
var users = testData.Item1;
var roles = testData.Item2;
var accessLevels = testData.Item3;
var roleAccessLevels = testData.Item4;
var userAccessLevels = testData.Item5;
var userRoles = testData.Item6;

var nodeData = new List<IImportable>();
var relationshipData = new List<IImportable>();
nodeData.AddRange(users);
nodeData.AddRange(roles);
nodeData.AddRange(accessLevels);
relationshipData.AddRange(roleAccessLevels);
relationshipData.AddRange(userAccessLevels);
relationshipData.AddRange(userRoles);

var logger = new Logger();
var importer = new DataImport(uri: auraUri, user: user, password: password, logger: logger);

await importer.ImportBatchAsync(users);
await importer.ImportBatchAsync(roles);
await importer.ImportBatchAsync(accessLevels);
await importer.ImportBatchAsync(roleAccessLevels);
await importer.ImportBatchAsync(userAccessLevels);
await importer.ImportBatchAsync(userRoles);

var exporter = new DataExport(uri: auraUri, user: user, password: password, logger: logger);
var result = await exporter.ExportData();

Console.WriteLine("Program.cs done.");
public class Logger : ILogger<IImportable>
{
    public IDisposable? BeginScope<TState>(TState state) => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        Console.WriteLine(formatter(state, exception));
    }
}


//var testImport = new List<IImportable>
//{
//    new UserRole { UserRoleId = 1421, UserId = Guid.NewGuid(), RoleId = 1512 }
//};
//await importer.ImportData(testImport);

//var exporter = new DataExport(user: user, password: password, uri: auraUri);
//while (true)
//{
//    Console.WriteLine("Enter a Cypher query to run:");
//    var cypherQuery = Console.ReadLine();
//    if (cypherQuery == "exit")
//    {
//        break;
//    }
//    var result = await exporter.ExportToIImportable(cypherQuery);
//    Console.WriteLine(result);
//}

//var testImport = new List<IImportable>
//{
//    new User { UserId = Guid.NewGuid(), UserName = "John Doe" }
//};
////Console.WriteLine(testImport[0].ToCypherQuery());
////Console.WriteLine(testImport[0].GetParameters());
//await importer.ImportData(testImport);

//var test = new HelloWorldExample(uri: auraUri, user: user, password: password);
//test.PrintGreetingAsync("Hello, world!").Wait();