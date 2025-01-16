using neo4j.console;
using neo4j.lib;
using neo4j.lib.Classes;
using neo4j.lib.Interfaces;


var testData = TestDataReader.GetTestData();
var users = testData.Item1;
var roles = testData.Item2;
var accessLevels = testData.Item3;
var roleAccessLevels = testData.Item4;
var userAccessLevels = testData.Item5;
var userRoles = testData.Item6;
var tenants = testData.Item7;

var nodeData = new List<IImportable>();
var relationshipData = new List<IImportable>();
nodeData.AddRange(users);
nodeData.AddRange(roles);
nodeData.AddRange(accessLevels);
nodeData.AddRange(tenants);
relationshipData.AddRange(roleAccessLevels);
relationshipData.AddRange(userAccessLevels);
relationshipData.AddRange(userRoles);


var auraUri = "neo4j+s://28b70b84.databases.neo4j.io";
var user = "neo4j";
var password = "SMbachelor254!";


var importer = new DataImport(user: user, password: password, uri: auraUri);
Console.WriteLine("Importing nodes...");
await importer.ImportData(nodeData);
Console.WriteLine("Importing relationships...");
await importer.ImportData(relationshipData);


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