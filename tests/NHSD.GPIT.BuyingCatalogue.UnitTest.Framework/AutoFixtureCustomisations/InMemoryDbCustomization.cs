using System;
using AutoFixture;
using AutoFixture.Kernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Identity;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations
{
    [ExcludesAutoCustomization]
    internal sealed class InMemoryDbCustomization : ICustomization
    {
        private readonly MockingFramework mockingFramework;
        private readonly DbContextOptions<BuyingCatalogueDbContext> dbContextOptions;

        public InMemoryDbCustomization(string dbName, MockingFramework mockingFramework)
        {
            this.mockingFramework = mockingFramework;
            dbContextOptions = new DbContextOptionsBuilder<BuyingCatalogueDbContext>()
                .EnableSensitiveDataLogging()
                .UseInMemoryDatabase(dbName)
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;
        }

        public void Customize(IFixture fixture)
        {
            ISpecimenBuilder identityServiceBuilder = mockingFramework == MockingFramework.Moq
                ? new MoqIdentityServiceSpecimenBuilder()
                : new IdentityServiceSpecimenBuilder();

            fixture.Customize<BuyingCatalogueDbContext>(_ => identityServiceBuilder);
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

                dbContext.Roles.Add(new() { Name = OrganisationFunction.Buyer.Name, NormalizedName = OrganisationFunction.Buyer.Name.ToUpperInvariant() });
                dbContext.Roles.Add(new() { Name = OrganisationFunction.Authority.Name, NormalizedName = OrganisationFunction.Authority.Name.ToUpperInvariant() });
                dbContext.Roles.Add(new() { Name = OrganisationFunction.AccountManager.Name, NormalizedName = OrganisationFunction.AccountManager.Name.ToUpperInvariant() });
                dbContext.SaveChanges();

                return dbContext;
            }
        }
    }
}
