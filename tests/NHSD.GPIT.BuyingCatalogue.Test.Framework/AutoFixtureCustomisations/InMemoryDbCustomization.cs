using System;
using AutoFixture;
using AutoFixture.Kernel;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Identity;

namespace NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations
{
    public sealed class InMemoryDbCustomization : ICustomization
    {
        private readonly DbContextOptions<BuyingCatalogueDbContext> dbContextOptions;

        public InMemoryDbCustomization(string dbName)
        {
            dbContextOptions = new DbContextOptionsBuilder<BuyingCatalogueDbContext>()
                .EnableSensitiveDataLogging()
                .UseInMemoryDatabase(dbName)
                .Options;
        }

        public void Customize(IFixture fixture)
        {
            fixture.Customize<BuyingCatalogueDbContext>(_ => new MockIdentityServiceSpecimenBuilder());
            fixture.Customize<BuyingCatalogueDbContext>(_ => new ApplicationDbContextSpecimenBuilder(dbContextOptions));
        }

        private sealed class ApplicationDbContextSpecimenBuilder : ISpecimenBuilder
        {
            private readonly DbContextOptions<BuyingCatalogueDbContext> dbContextOptions;

            internal ApplicationDbContextSpecimenBuilder(DbContextOptions<BuyingCatalogueDbContext> dbContextOptions) =>
                this.dbContextOptions = dbContextOptions;

            public object Create(object request, ISpecimenContext context)
            {
                if (!(request as Type == typeof(BuyingCatalogueDbContext)))
                    return new NoSpecimen();

                var identityService = context.Create<IIdentityService>();

                var dbContext = new BuyingCatalogueDbContext(dbContextOptions, identityService);
                dbContext.Database.EnsureDeleted();
                dbContext.Database.EnsureCreated();

                return dbContext;
            }
        }
    }
}
