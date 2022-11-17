using System;
using AutoFixture;
using AutoFixture.Kernel;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;

namespace OrganisationImporterTests.AutoFixtureCustomizations;

public sealed class InMemoryDbCustomization : ICustomization
    {
        private readonly DbContextOptions<BuyingCatalogueDbContext> _dbContextOptions;

        public InMemoryDbCustomization()
        {
            var sqliteConnection = new SqliteConnection("DataSource=:memory:");
            sqliteConnection.Open();

            _dbContextOptions = new DbContextOptionsBuilder<BuyingCatalogueDbContext>()
                .EnableSensitiveDataLogging()
                .UseSqlite(sqliteConnection)
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                .Options;
        }

        public void Customize(IFixture fixture)
        {
            fixture.Customize<BuyingCatalogueDbContext>(_ => new ApplicationDbContextSpecimenBuilder(_dbContextOptions));
        }

        private sealed class ApplicationDbContextSpecimenBuilder : ISpecimenBuilder
        {
            private readonly DbContextOptions<BuyingCatalogueDbContext> _dbContextOptions;

            internal ApplicationDbContextSpecimenBuilder(DbContextOptions<BuyingCatalogueDbContext> dbContextOptions) =>
                this._dbContextOptions = dbContextOptions;

            public object Create(object request, ISpecimenContext context)
            {
                if (!(request as Type == typeof(BuyingCatalogueDbContext)))
                    return new NoSpecimen();

                var dbContext = new BuyingCatalogueDbContext(_dbContextOptions);
                dbContext.Database.EnsureDeleted();
                dbContext.Database.EnsureCreated();

                return dbContext;
            }
        }
    }
