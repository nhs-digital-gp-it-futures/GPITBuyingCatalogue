using System;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Database.Configuration;
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
