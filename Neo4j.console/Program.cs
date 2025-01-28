using Microsoft.Extensions.Logging;
using neo4j.console;
using Neo4j.library;
using Neo4j.library.Interfaces;
using Newtonsoft.Json.Linq;
using Neo4j.library.Classes.Nodes;
using Neo4j.library.Classes.Relationships;
using System.Diagnostics;
using System.Reflection.Metadata;
using CsvHelper;
using System.Globalization;

var credentials = JObject.Parse(File.ReadAllText("Credentials.json"));
if (credentials == null)
{
    Console.WriteLine("Credentials file not found.");
    return;
}
var auraUri = credentials["uri"].ToString();
var dockerUri = "bolt://localhost:7687";
var user = credentials["username"].ToString();
var password = credentials["password"].ToString();

//var testData = TestDataReader.GetTestData();
//var users = testData.Item1;
//var roles = testData.Item2;
//var accessLevels = testData.Item3;
//var roleAccessLevels = testData.Item4;
//var userAccessLevels = testData.Item5;
//var userRoles = testData.Item6;

//var nodeData = new List<IImportable>();
//var relationshipData = new List<IImportable>();
//nodeData.AddRange(users);
//nodeData.AddRange(roles);
//nodeData.AddRange(accessLevels);
//relationshipData.AddRange(roleAccessLevels);
//relationshipData.AddRange(userAccessLevels);
//relationshipData.AddRange(userRoles);

//var allData = new List<IImportable>();
//allData.AddRange(nodeData);
//allData.AddRange(relationshipData);

//var logger = new Logger();
//logger.SetLogLevel(LogLevel.Debug);
//var importer = new DataImport(uri: auraUri, user: user, password: password, logger: logger);

//await importer.ImportBatchBetterAsync(allData);

//await importer.ImportBatchAsync(users);
//await importer.ImportBatchAsync(roles);
//await importer.ImportBatchAsync(accessLevels);
//await importer.ImportBatchAsync(roleAccessLevels);
//await importer.ImportBatchAsync(userAccessLevels);
//await importer.ImportBatchAsync(userRoles);

//var exporter = new DataExport(uri: auraUri, user: user, password: password, logger: logger);
//var result = await exporter.ExportData();

//var init = new InitializeDB(uri: dockerUri, user: user, password: password);
//await init.CreateConstraints();

var testLogger = new Logger();

var testUsers = new List<IImportable>()
{
    new User
    {
        UserId = Guid.NewGuid(),
        UserName = "John Doe",
        Parameters = new()
        {
            { "Email", "john.d@example.com" },
            { "Phone", "555-555-5555" }
        }
    },
    new User
    {
        UserId = Guid.NewGuid(),
        UserName = "Jane Doe",
        Parameters = new()
        {
            { "Email", "jane@doodle.no" },
            { "Phone", "125-555-5555" }
        }
    },
    new User
    {
        UserId = Guid.NewGuid(),
        UserName = "Jim Doe",
        Parameters = new()
        {
            { "Email", "jimmy.doe@cool.space.com" },
            { "Phone", "555-589-55615" }
        }
    }
};

var testImporter = new DataImport(uri: auraUri, user: user, password: password, logger: testLogger);
//await testImporter.ImportBatchBetterAsync(testUsers);

var test = new ImportPerformanceTest(1000000, 60, new DataImport(uri: dockerUri, user: user, password: password, logger: testLogger));
await test.RunTest();

//await testImporter.ImportUsingCsvAsync("csv/nodes.csv", "csv/relationships.csv");

Console.WriteLine("Program.cs done.");


public class Logger : ILogger<IImportable>
{
    public IDisposable? BeginScope<TState>(TState state) => null;

    public bool IsEnabled(LogLevel logLevel) => logLevel >= _currentLogLevel;

    private LogLevel _currentLogLevel = LogLevel.Information;

    private readonly Dictionary<LogLevel, ConsoleColor> _logLevelColors = new()
    {
        { LogLevel.Trace, ConsoleColor.Gray },
        { LogLevel.Debug, ConsoleColor.Cyan },
        { LogLevel.Information, ConsoleColor.Green },
        { LogLevel.Warning, ConsoleColor.Yellow },
        { LogLevel.Error, ConsoleColor.Red },
        { LogLevel.Critical, ConsoleColor.Magenta }
    };
    public void SetLogLevel(LogLevel logLevel)
    {
        _currentLogLevel = logLevel;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        ConsoleColor color = _logLevelColors.ContainsKey(logLevel)
            ? _logLevelColors[logLevel]
            : ConsoleColor.White;

        Console.ForegroundColor = color;
        Console.WriteLine(formatter(state, exception));
        Console.ResetColor();

        // Reset to default color
        Console.ResetColor();
    }
}




public class ImportPerformanceTest
{
    public int NodeCount { get; set; }
    public int RelationshipPercentage { get; set; }
    public DataImport DataImport { get; set; }

    public ImportPerformanceTest(int nodeCount, int relationshipPercentage, DataImport dataImport)
    {
        NodeCount = nodeCount;
        RelationshipPercentage = relationshipPercentage;
        DataImport = dataImport;
    }

    public async Task<ImportStatistics> RunTest()
    {
        var users = new List<User>();
        var roles = new List<Role>();
        var accessLevels = new List<AccessLevel>();
        var userRoles = new List<UserRole>();
        var roleAccessLevels = new List<RoleAccessLevel>();
        var userAccessLevels = new List<UserAccessLevel>();

        var random = new Random();

        // Generate Users
        for (int i = 0; i < NodeCount / 3; i++)
        {
            users.Add(new User
            {
                UserId = Guid.NewGuid(),
                UserName = $"User_{i}",
            });
        }

        // Generate Roles
        for (int i = 0; i < NodeCount / 3; i++)
        {
            roles.Add(new Role
            {
                RoleId = i,
                RoleTitle = $"Role_{i}",
            });
        }

        // Generate AccessLevels
        for (int i = 0; i < NodeCount / 3; i++)
        {
            accessLevels.Add(new AccessLevel
            {
                AccessLevelId = i,
                AccessLevelTitle = $"AccessLevel_{i}",
            });
        }

        // Generate Relationships
        for (int i = 0; i < users.Count; i++)
        {
            // UserRole Relationships
            if (random.Next(0, 100) < RelationshipPercentage)
            {
                var roleIndex = random.Next(0, roles.Count);
                userRoles.Add(new UserRole
                {
                    UserRoleId = i,
                    UserId = users[i].UserId,
                    RoleId = roles[roleIndex].RoleId
                });
            }

            // UserAccessLevel Relationships
            if (random.Next(0, 100) < RelationshipPercentage)
            {
                var accessLevelIndex = random.Next(0, accessLevels.Count);
                userAccessLevels.Add(new UserAccessLevel
                {
                    UserAccessLevelId = i,
                    UserId = users[i].UserId,
                    AccessLevelId = accessLevels[accessLevelIndex].AccessLevelId
                });
            }
        }

        // Generate RoleAccessLevel Relationships
        for (int i = 0; i < roles.Count; i++)
        {
            if (random.Next(0, 100) < RelationshipPercentage)
            {
                var accessLevelIndex = random.Next(0, accessLevels.Count);
                roleAccessLevels.Add(new RoleAccessLevel
                {
                    RoleAccessLevelId = i,
                    RoleId = roles[i].RoleId,
                    AccessLevelId = accessLevels[accessLevelIndex].AccessLevelId
                });
            }
        }

        // Combine all importable items
        var data = new List<IImportable>();
        data.AddRange(users);
        data.AddRange(accessLevels);
        data.AddRange(roles);
        data.AddRange(userRoles);
        data.AddRange(roleAccessLevels);
        data.AddRange(userAccessLevels);

        Console.WriteLine("Import Statistics:");
        Console.WriteLine($"Total Nodes: {users.Count + roles.Count + accessLevels.Count}");
        Console.WriteLine($"Total Relationships: {userRoles.Count + roleAccessLevels.Count + userAccessLevels.Count}");


        var stopwatch = Stopwatch.StartNew();

        await DataImport.ImportBatchBetterAsync(data);

        //System.IO.Directory.CreateDirectory("csv");

        //await using (var writer = new StreamWriter("csv/users.csv"))
        //await using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        //{
        //    csv.WriteRecords(users);
        //}

        //await using (var writer = new StreamWriter("csv/roles.csv"))
        //await using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        //{
        //    csv.WriteRecords(roles);
        //}

        //await using (var writer = new StreamWriter("csv/accessLevels.csv"))
        //await using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        //{
        //    csv.WriteRecords(accessLevels);
        //}

        //await using (var writer = new StreamWriter("csv/userRoles.csv"))
        //await using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        //{
        //    csv.WriteRecords(userRoles);
        //}

        //await using (var writer = new StreamWriter("csv/roleAccessLevels.csv"))
        //await using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        //{
        //    csv.WriteRecords(roleAccessLevels);
        //}

        //await using (var writer = new StreamWriter("csv/userAccessLevels.csv"))
        //await using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        //{
        //    csv.WriteRecords(userAccessLevels);
        //}


        //await DataImport.ImportBatchBetterAsync(data, 700);

        stopwatch.Stop();


        var statistics = new ImportStatistics
        {
            TotalNodes = users.Count + roles.Count + accessLevels.Count,
            TotalRelationships = userRoles.Count + roleAccessLevels.Count + userAccessLevels.Count,
            ImportDuration = stopwatch.Elapsed,
        };

        //Console.WriteLine("Statistics: ");
        //Console.WriteLine($"    Total Nodes: {statistics.TotalNodes}");
        //Console.WriteLine($"    Total Relationships: {statistics.TotalRelationships}");
        //Console.WriteLine($"    Import Duration: {statistics.ImportDuration}");

        return statistics;
    }


    public void GenerateImportableCsv(string filepath, List<IImportable> data)
    {
        using (var writer = new StreamWriter(filepath))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(data);
        }
    }

}
public class ImportStatistics
{
    public int TotalNodes { get; set; }
    public int TotalRelationships { get; set; }
    public TimeSpan ImportDuration { get; set; }
    public double NodesPerSecond { get; set; }
    public double RelationshipsPerSecond { get; set; }
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