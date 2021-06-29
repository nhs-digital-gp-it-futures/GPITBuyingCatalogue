using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Database.Configuration;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Database
{
    internal sealed class EndToEndDbContext : GPITBuyingCatalogueDbContext
    {
        public EndToEndDbContext(DbContextOptions<EndToEndDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new CatalogueItemTypeEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CataloguePriceTypeEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CapabilityStatusEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CompliancyLevelEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ProvisioningTypeEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PublicationStatusEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new TimeUnitEntityTypeConfiguration());
        }
    }
}
