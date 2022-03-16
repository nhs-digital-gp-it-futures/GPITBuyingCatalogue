using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Services.Solutions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Solutions
{
    public static class SolutionsFilterServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(SolutionsFilterService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        public static async Task GetCapabilityNamesWithEpics_NullOrEmpty_ReturnsEmptyDictionary(
            string capabilities,
            SolutionsFilterService service)
        {
            var result = await service.GetCapabilityNamesWithEpics(capabilities);

            result.Should().BeEquivalentTo(new Dictionary<string, int>());
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetCapabilityNamesWithEpics_Valid_Returns(
            List<Capability> seedCapabilities,
            [Frozen] BuyingCatalogueDbContext context,
            SolutionsFilterService service)
        {
            var capabilities = seedCapabilities.Take(2).ToArray();
            capabilities[0].CapabilityRef = "C1";
            capabilities[1].CapabilityRef = "C10";

            context.Capabilities.AddRange(capabilities);
            context.SaveChanges();

            var capabilitiesSearch = string.Join('|', capabilities.Select(s => s.CapabilityRef));
            var result = await service.GetCapabilityNamesWithEpics(capabilitiesSearch);

            var expectedCapabilities = capabilities.ToDictionary(c => c.Name, c => 0);

            result.Should().BeEquivalentTo(expectedCapabilities);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetCapabilityNamesWithEpics_Valid_ReturnsEpicCount(
            Epic epic,
            Capability capability,
            [Frozen] BuyingCatalogueDbContext context,
            SolutionsFilterService service)
        {
            epic.Id = "E1";
            capability.CapabilityRef = "C1";

            capability.Epics.Add(epic);

            context.Epics.Add(epic);
            context.Capabilities.Add(capability);

            context.SaveChanges();

            var capabilitiesSearch = string.Join(string.Empty, capability.CapabilityRef, epic.Id);
            var result = await service.GetCapabilityNamesWithEpics(capabilitiesSearch);

            result.Should().BeEquivalentTo(new Dictionary<string, int> { { capability.Name, 1 } });
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetCapabilityNamesWithEpics_Foundation_ReturnsFoundationCapabilityName(
            SolutionsFilterService service)
        {
            var result = await service.GetCapabilityNamesWithEpics("FC");

            result.Should().Contain(new KeyValuePair<string, int>("Foundation", 0));
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        public static async Task GetFrameworkName_NullOrEmpty_Returns(
            string frameworkId,
            SolutionsFilterService service)
        {
            var result = await service.GetFrameworkName(frameworkId);

            result.Should().Be(frameworkId);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetFrameworkName_All_ReturnsAll(
            SolutionsFilterService service)
        {
            var result = await service.GetFrameworkName("All");

            result.Should().Be("All");
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetFrameworkName_ValidFramework_ReturnsShortName(
            EntityFramework.Catalogue.Models.Framework framework,
            [Frozen] BuyingCatalogueDbContext context,
            SolutionsFilterService service)
        {
            context.Frameworks.Add(framework);
            context.SaveChanges();

            var result = await service.GetFrameworkName(framework.Id);

            result.Should().Be(framework.ShortName);
        }
    }
}
