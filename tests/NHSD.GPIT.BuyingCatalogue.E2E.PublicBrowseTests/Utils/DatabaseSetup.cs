using System;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.SeedData;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using Testcontainers.MsSql;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Utils
{
    public class DatabaseSetup
    {
        private MsSqlContainer sqlContainer;

        public async Task StartAsync()
        {
            sqlContainer = new MsSqlBuilder()
                .WithName(Guid.NewGuid().ToString())
                .WithPassword("Abc123Abc123")
                .WithCleanUp(true)
                .Build();

            await sqlContainer.StartAsync();
        }

        public string GetConnectionString() => sqlContainer.GetConnectionString();

        public async ValueTask DisposeAsync()
        {
            await sqlContainer.DisposeAsync();
        }

        internal static void InvokeDataSeeder<T>(BuyingCatalogueDbContext context)
            where T : ISeedData
        {
            T.Initialize(context).GetAwaiter().GetResult();
        }
    }
}
