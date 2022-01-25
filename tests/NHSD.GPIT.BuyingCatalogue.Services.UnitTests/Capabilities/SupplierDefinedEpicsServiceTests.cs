using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Services.Capabilities;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Capabilities
{
    public static class SupplierDefinedEpicsServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(SupplierDefinedEpicsService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetSupplierDefinedEpics_ReturnsExpectedResult(
            Capability capability,
            List<Epic> epics,
            [Frozen] BuyingCatalogueDbContext context,
            SupplierDefinedEpicsService service)
        {
            epics.ForEach(e =>
            {
                e.SupplierDefined = true;
                e.CapabilityId = capability.Id;
            });

            context.Capabilities.Add(capability);
            context.Epics.AddRange(epics);
            await context.SaveChangesAsync();

            var supplierDefinedEpics = await service.GetSupplierDefinedEpics();

            supplierDefinedEpics.Count.Should().Be(epics.Count);
        }
    }
}
