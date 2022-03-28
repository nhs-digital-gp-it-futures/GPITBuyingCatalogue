using System;
using System.Linq;
using System.Reflection;
using DbUp;

namespace NHSD.GPITBuyingCatalogue.Database.PostDeployment
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            var connectionString = args.FirstOrDefault() ?? Environment.GetEnvironmentVariable("DBUP_CONNECTIONSTRING");

            var upgrader =
                DeployChanges.To
                    .SqlDatabase(connectionString)
                    .JournalToSqlTable("versioning", "SchemaVersions")
                    .WithTransactionPerScript()
                    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                    .LogToConsole()
                    .Build();

            var result = upgrader.PerformUpgrade();

            if (!result.Successful)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(result.Error);
                Console.ResetColor();
                return -1;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Success!");
            Console.ResetColor();
            return 0;
        }
    }
}
