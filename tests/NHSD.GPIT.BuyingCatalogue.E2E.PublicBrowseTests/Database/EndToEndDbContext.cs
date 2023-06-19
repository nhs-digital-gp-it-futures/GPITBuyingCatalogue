using System.Reflection;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Identity;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Database
{
    internal sealed class EndToEndDbContext : BuyingCatalogueDbContext
    {
        public EndToEndDbContext(DbContextOptions<EndToEndDbContext> options, IIdentityService identityService)
            : base(options, identityService)
        {
        }

        public EndToEndDbContext(DbContextOptions<EndToEndDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
