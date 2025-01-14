using CsvHelper;
using CsvHelper.Configuration;
using neo4j.lib.Classes;
using System.Globalization;

namespace neo4j.console
{
    public static class TestDataReader
    {
        public static List<T> ReadCsv<T>(string filePath)
        {
            var workDir = Directory.GetCurrentDirectory();
            var csvFilePath = Path.Combine(workDir, "..", "..", "..", "csv", filePath);
            using (var reader = new StreamReader(csvFilePath))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
            {
                return new List<T>(csv.GetRecords<T>());
            }
        }

        public static Tuple<List<User>, List<Role>, List<AccessLevel>, List<RoleAccessLevel>, List<UserAccessLevel>, List<UserRole>> GetTestData()
        {
            // File paths
            string usersFile = "Users.csv"; // Replace with the actual file path
            string rolesFile = "Roles.csv";
            string accessLevelsFile = "AccessLevels.csv";
            string roleAccessLevelsFile = "RoleAccessLevels.csv";
            string userAccessLevelsFile = "UserAccessLevels.csv";
            string userRolesFile = "UserRoles.csv";

            // Read CSV files
            List<User> users = ReadCsv<User>(usersFile);
            List<Role> roles = ReadCsv<Role>(rolesFile);
            List<AccessLevel> accessLevels = ReadCsv<AccessLevel>(accessLevelsFile);
            List<RoleAccessLevel> roleAccessLevels = ReadCsv<RoleAccessLevel>(roleAccessLevelsFile);
            List<UserAccessLevel> userAccessLevels = ReadCsv<UserAccessLevel>(userAccessLevelsFile);
            List<UserRole> userRoles = ReadCsv<UserRole>(userRolesFile);

            return new (users, roles, accessLevels, roleAccessLevels, userAccessLevels, userRoles);
        }
    }
}
