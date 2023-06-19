using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Database;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.SeedData;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Fixtures
{
    public class DatabaseFixture : IAsyncLifetime
    {
        private DatabaseSetup databaseSetup;

        public async Task InitializeAsync()
        {
            databaseSetup = new DatabaseSetup();
            await databaseSetup.StartAsync();

            var dbContext = GetDbContext();
            dbContext.Database.EnsureCreated();
            DatabaseSetup.InvokeDataSeeder<BuyingCatalogueSeedData>(dbContext);
        }

        public async Task DisposeAsync()
        {
            await databaseSetup.DisposeAsync().ConfigureAwait(false);
        }

        internal EndToEndDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<EndToEndDbContext>()
                        .UseSqlServer(databaseSetup.GetConnectionString())
                        .EnableSensitiveDataLogging()
                        .Options;

            return new EndToEndDbContext(options);
        }
    }
}
