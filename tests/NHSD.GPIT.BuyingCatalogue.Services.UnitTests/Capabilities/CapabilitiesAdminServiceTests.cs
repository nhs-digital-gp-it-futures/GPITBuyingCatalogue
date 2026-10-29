using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.Services.Capabilities;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Capabilities
{
    public static class CapabilitiesAdminServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(CapabilitiesAdminService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetPagedCapabilitiesAsync_ReturnsOrderedResultsAcrossPages(
            [Frozen] BuyingCatalogueDbContext context,
            CapabilitiesAdminService service)
        {
            var category = new CapabilityCategory
            {
                Id = 1,
                Name = "Core",
                Description = "Core category",
            };

            var capabilities = new List<Capability>
            {
                CreateCapability(1, "CAP-003", "gamma", category),
                CreateCapability(2, "CAP-001", "alpha", category),
                CreateCapability(3, "CAP-005", "epsilon", category),
                CreateCapability(4, "CAP-002", "beta", category),
                CreateCapability(5, "CAP-004", "delta", category),
            };

            context.Capabilities.AddRange(capabilities);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var page1 = await service.GetPagedCapabilitiesAsync(new PageOptions("1", 2));
            var page2 = await service.GetPagedCapabilitiesAsync(new PageOptions("2", 2));
            var page3 = await service.GetPagedCapabilitiesAsync(new PageOptions("3", 2));

            page1.Items.Select(x => x.Name).Should().Equal("alpha", "beta");
            page2.Items.Select(x => x.Name).Should().Equal("delta", "epsilon");
            page3.Items.Select(x => x.Name).Should().Equal("gamma");

            page1.Options.TotalNumberOfItems.Should().Be(5);
            page1.Options.NumberOfPages.Should().Be(3);
        }

        private static Capability CreateCapability(
            int id,
            string capabilityRef,
            string name,
            CapabilityCategory category) =>
            new()
            {
                Id = id,
                CapabilityRef = capabilityRef,
                Name = name,
                Description = $"Description for {name}",
                Category = category,
                CategoryId = category.Id,
                Status = CapabilityStatus.Effective,
            };
    }
}
