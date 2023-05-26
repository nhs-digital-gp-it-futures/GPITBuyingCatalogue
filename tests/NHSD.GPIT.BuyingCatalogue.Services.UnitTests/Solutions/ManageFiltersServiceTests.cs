using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.Services.Solutions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Solutions
{
    public static class ManageFiltersServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(ManageFiltersService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData(" ")]
        public static Task FilterExists_NullOrEmptyFilterName_ThrowsException(
            string name,
            ManageFiltersService service,
            int organisationId)
        {
            return Assert.ThrowsAsync<ArgumentNullException>(() => service.FilterExists(name, organisationId));
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task FilterExists_NameAlreadyExists_DifferentOrganisation_ReturnsFalse(
            [Frozen] BuyingCatalogueDbContext context,
            Filter filter,
            ManageFiltersService service)
        {
            context.Filters.Add(filter);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var result = await service.FilterExists(filter.Name, filter.OrganisationId + 1);

            result.Should().BeFalse();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task FilterExists_NameAlreadyExists_ReturnsTrue(
            [Frozen] BuyingCatalogueDbContext context,
            Filter filter,
            ManageFiltersService service)
        {
            context.Filters.Add(filter);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var result = await service.FilterExists(filter.Name, filter.OrganisationId);

            result.Should().BeTrue();
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData(" ")]
        public static void AddFilter_NullOrEmptyName_ThrowsException(
            string name,
            string description,
            int organisationId,
            List<int> capabilityIds,
            List<string> epicIds,
            string frameworkId,
            List<ClientApplicationType> clientApplicationTypes,
            List<HostingType> hostingTypes,
            ManageFiltersService service)
        {
            FluentActions
                .Awaiting(
                    () => service.AddFilter(
                        name,
                        description,
                        organisationId,
                        capabilityIds,
                        epicIds,
                        frameworkId,
                        clientApplicationTypes,
                        hostingTypes))
                .Should()
                .ThrowAsync<ArgumentNullException>(nameof(name));
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData(" ")]
        public static void AddFilter_NullOrEmptyDescription_ThrowsException(
            string description,
            string name,
            int organisationId,
            List<int> capabilityIds,
            List<string> epicIds,
            string frameworkId,
            List<ClientApplicationType> clientApplicationTypes,
            List<HostingType> hostingTypes,
            ManageFiltersService service)
        {
            FluentActions
                .Awaiting(
                    () => service.AddFilter(
                        name,
                        description,
                        organisationId,
                        capabilityIds,
                        epicIds,
                        frameworkId,
                        clientApplicationTypes,
                        hostingTypes))
                .Should()
                .ThrowAsync<ArgumentNullException>(nameof(description));
        }

        [Theory]
        [InMemoryDbAutoData]
        public static void AddFilter_NullOrganisation_ThrowsException(
            string description,
            string name,
            int organisationId,
            List<int> capabilityIds,
            List<string> epicIds,
            string frameworkId,
            List<ClientApplicationType> clientApplicationTypes,
            List<HostingType> hostingTypes,
            ManageFiltersService service)
        {
            FluentActions
                .Awaiting(
                    () => service.AddFilter(
                        name,
                        description,
                        organisationId,
                        capabilityIds,
                        epicIds,
                        frameworkId,
                        clientApplicationTypes,
                        hostingTypes))
                .Should()
                .ThrowAsync<ArgumentException>("organisationId");
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddFilter_ValidParameters_FilterCreated(
            [Frozen] BuyingCatalogueDbContext context,
            string description,
            string name,
            Organisation organisation,
            EntityFramework.Catalogue.Models.Framework framework,
            ManageFiltersService service)
        {
            context.Organisations.Add(organisation);
            await context.SaveChangesAsync();

            context.Frameworks.Add(framework);
            await context.SaveChangesAsync();

            var result = await service.AddFilter(
                name,
                description,
                organisation.Id,
                null,
                null,
                framework.Id,
                null,
                null);
            result.Should().NotBe(0);

            var newFilter = await context.Filters.FirstAsync(f => f.Id == result);
            newFilter.Should().NotBeNull();

            newFilter.Name.Should().Be(name);
            newFilter.Description.Should().Be(description);
            newFilter.OrganisationId.Should().Be(organisation.Id);
            newFilter.FrameworkId.Should().Be(framework.Id);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddFilterCapabilities_NullCapabilityIds_NoCapabilitiesAdded(
            [Frozen] BuyingCatalogueDbContext context,
            Filter filter,
            ManageFiltersService service)
        {
            filter.Capabilities.Clear();
            context.Filters.Add(filter);
            await context.SaveChangesAsync();

            await service.AddFilterCapabilities(filter.Id, null);
            context.ChangeTracker.Clear();

            var result = await context.Filters.FirstAsync(f => f.Id == filter.Id);
            result.Should().NotBeNull();

            result.Capabilities.Should().BeEmpty();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddFilterCapabilities_EmptyCapabilityIds_NoCapabilitiesAdded(
            [Frozen] BuyingCatalogueDbContext context,
            Filter filter,
            ManageFiltersService service)
        {
            filter.Capabilities.Clear();
            context.Filters.Add(filter);
            await context.SaveChangesAsync();

            await service.AddFilterCapabilities(filter.Id, new List<int>());
            context.ChangeTracker.Clear();

            var result = await context.Filters.FirstAsync(f => f.Id == filter.Id);
            result.Should().NotBeNull();

            result.Capabilities.Should().BeEmpty();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddFilterCapabilities_NullFilter_NoCapabilitiesAdded(
            [Frozen] BuyingCatalogueDbContext context,
            List<int> capabilityIds,
            int invalidFilterId,
            ManageFiltersService service)
        {
            await service.AddFilterCapabilities(invalidFilterId, capabilityIds);
            context.ChangeTracker.Clear();

            var filter = await context.Filters.FirstOrDefaultAsync(f => f.Id == invalidFilterId);
            filter.Should().BeNull();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddFilterCapabilities_ValidParameters_CapabilitiesAdded(
            [Frozen] BuyingCatalogueDbContext context,
            Filter filter,
            List<Capability> capabilities,
            ManageFiltersService service)
        {
            filter.Capabilities.Clear();
            context.Filters.Add(filter);
            await context.SaveChangesAsync();

            context.Capabilities.AddRange(capabilities);
            await context.SaveChangesAsync();

            var capabilityIds = capabilities.Select(c => c.Id).ToList();

            await service.AddFilterCapabilities(filter.Id, capabilityIds);
            context.ChangeTracker.Clear();

            var result = await context.Filters.Include(f => f.Capabilities).FirstAsync(f => f.Id == filter.Id);
            result.Should().NotBeNull();

            result.Capabilities.Should().NotBeNullOrEmpty();
            result.Capabilities.Count.Should().Be(capabilities.Count);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddFilterEpics_NullEpicIds_NoEpicsAdded(
            [Frozen] BuyingCatalogueDbContext context,
            Filter filter,
            ManageFiltersService service)
        {
            filter.Epics.Clear();
            context.Filters.Add(filter);
            await context.SaveChangesAsync();

            await service.AddFilterCapabilities(filter.Id, null);
            context.ChangeTracker.Clear();

            var result = await context.Filters.FirstAsync(f => f.Id == filter.Id);
            result.Should().NotBeNull();

            result.Epics.Should().BeEmpty();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddFilterEpics_EmptyEpicIds_NoEpicsAdded(
            [Frozen] BuyingCatalogueDbContext context,
            Filter filter,
            ManageFiltersService service)
        {
            filter.Epics.Clear();
            context.Filters.Add(filter);
            await context.SaveChangesAsync();

            await service.AddFilterEpics(filter.Id, new List<string>());
            context.ChangeTracker.Clear();

            var result = await context.Filters.FirstAsync(f => f.Id == filter.Id);
            result.Should().NotBeNull();

            result.Epics.Should().BeEmpty();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddFilterEpics_NullFilter_NoEpicsAdded(
            [Frozen] BuyingCatalogueDbContext context,
            List<string> epicIds,
            int invalidFilterId,
            ManageFiltersService service)
        {
            await service.AddFilterEpics(invalidFilterId, epicIds);
            context.ChangeTracker.Clear();

            var filter = await context.Filters.FirstOrDefaultAsync(f => f.Id == invalidFilterId);
            filter.Should().BeNull();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddFilterEpics_ValidParameters_EpicsAdded(
            [Frozen] BuyingCatalogueDbContext context,
            Filter filter,
            List<Epic> epics,
            ManageFiltersService service)
        {
            filter.Epics.Clear();
            context.Filters.Add(filter);
            await context.SaveChangesAsync();

            context.Epics.AddRange(epics);
            await context.SaveChangesAsync();

            var epicIds = epics.Select(c => c.Id).ToList();

            await service.AddFilterEpics(filter.Id, epicIds);
            context.ChangeTracker.Clear();

            var result = await context.Filters.Include(f => f.Epics).FirstOrDefaultAsync(f => f.Id == filter.Id);
            result.Should().NotBeNull();

            result.Epics.Should().NotBeNullOrEmpty();
            result.Epics.Count.Should().Be(epics.Count);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddFilterClientApplicationTypes_NullCatIds_NoCatsAdded(
            [Frozen] BuyingCatalogueDbContext context,
            Filter filter,
            ManageFiltersService service)
        {
            filter.FilterClientApplicationTypes.Clear();
            context.Filters.Add(filter);
            await context.SaveChangesAsync();

            await service.AddFilterClientApplicationTypes(filter.Id, null);
            context.ChangeTracker.Clear();

            var result = await context.Filters.FirstAsync(f => f.Id == filter.Id);
            result.Should().NotBeNull();

            result.FilterClientApplicationTypes.Should().BeEmpty();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddFilterClientApplicationTypes_EmptyCatIds_NoCatsAdded(
            [Frozen] BuyingCatalogueDbContext context,
            Filter filter,
            ManageFiltersService service)
        {
            filter.FilterClientApplicationTypes.Clear();
            context.Filters.Add(filter);
            await context.SaveChangesAsync();

            await service.AddFilterClientApplicationTypes(filter.Id, new List<ClientApplicationType>());
            context.ChangeTracker.Clear();

            var result = await context.Filters.FirstAsync(f => f.Id == filter.Id);
            result.Should().NotBeNull();

            result.FilterClientApplicationTypes.Should().BeEmpty();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddFilterClientApplicationTypes_NullFilter_NoCatsAdded(
            [Frozen] BuyingCatalogueDbContext context,
            List<ClientApplicationType> cats,
            int invalidFilterId,
            ManageFiltersService service)
        {
            await service.AddFilterClientApplicationTypes(invalidFilterId, cats);
            context.ChangeTracker.Clear();

            var filter = await context.Filters.FirstOrDefaultAsync(f => f.Id == invalidFilterId);
            filter.Should().BeNull();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddFilterClientApplicationTypes_ValidParameters_CatsAdded(
            [Frozen] BuyingCatalogueDbContext context,
            Filter filter,
            List<ClientApplicationType> cats,
            ManageFiltersService service)
        {
            filter.FilterClientApplicationTypes.Clear();
            context.Filters.Add(filter);
            await context.SaveChangesAsync();

            await service.AddFilterClientApplicationTypes(filter.Id, cats);
            context.ChangeTracker.Clear();

            var result = await context.Filters.Include(f => f.FilterClientApplicationTypes).FirstOrDefaultAsync(f => f.Id == filter.Id);
            result.Should().NotBeNull();

            result.FilterClientApplicationTypes.Should().NotBeNullOrEmpty();
            result.FilterClientApplicationTypes.Count.Should().Be(cats.Count);

            foreach (var x in result.FilterClientApplicationTypes)
            {
                x.FilterId.Should().Be(filter.Id);
                cats.Should().Contain(x.ClientApplicationType);
            }
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddFilterHostingTypes_NullHostingTypeIds_NoHostingTypesAdded(
            [Frozen] BuyingCatalogueDbContext context,
            Filter filter,
            ManageFiltersService service)
        {
            filter.FilterHostingTypes.Clear();
            context.Filters.Add(filter);
            await context.SaveChangesAsync();

            await service.AddFilterHostingTypes(filter.Id, null);
            context.ChangeTracker.Clear();

            var result = await context.Filters.FirstAsync(f => f.Id == filter.Id);
            result.Should().NotBeNull();

            result.FilterHostingTypes.Should().BeEmpty();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddFilterHostingTypes_EmptyHostingTypeIds_NoHostingTypesAdded(
            [Frozen] BuyingCatalogueDbContext context,
            Filter filter,
            ManageFiltersService service)
        {
            filter.FilterHostingTypes.Clear();
            context.Filters.Add(filter);
            await context.SaveChangesAsync();

            await service.AddFilterHostingTypes(filter.Id, new List<HostingType>());
            context.ChangeTracker.Clear();

            var result = await context.Filters.FirstAsync(f => f.Id == filter.Id);
            result.Should().NotBeNull();

            result.FilterHostingTypes.Should().BeEmpty();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddFilterHostingTypes_NullFilter_NoHostingTypesAdded(
            [Frozen] BuyingCatalogueDbContext context,
            List<HostingType> hostingTypes,
            int invalidFilterId,
            ManageFiltersService service)
        {
            await service.AddFilterHostingTypes(invalidFilterId, hostingTypes);
            context.ChangeTracker.Clear();

            var filter = await context.Filters.FirstOrDefaultAsync(f => f.Id == invalidFilterId);
            filter.Should().BeNull();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddFilterHostingTypes_ValidParameters_HostingTypesAdded(
            [Frozen] BuyingCatalogueDbContext context,
            Filter filter,
            List<HostingType> hostingTypes,
            ManageFiltersService service)
        {
            filter.FilterHostingTypes.Clear();
            context.Filters.Add(filter);
            await context.SaveChangesAsync();

            await service.AddFilterHostingTypes(filter.Id, hostingTypes);
            context.ChangeTracker.Clear();

            var result = await context.Filters.Include(f => f.FilterHostingTypes).FirstOrDefaultAsync(f => f.Id == filter.Id);
            result.Should().NotBeNull();

            result.FilterHostingTypes.Should().NotBeNullOrEmpty();
            result.FilterHostingTypes.Count.Should().Be(hostingTypes.Count);

            foreach (var x in result.FilterHostingTypes)
            {
                x.FilterId.Should().Be(filter.Id);
                hostingTypes.Should().Contain(x.HostingType);
            }
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetFilters_ReturnsExpectedResults(
            Filter filter,
            Organisation organisation,
            [Frozen] BuyingCatalogueDbContext context,
            ManageFiltersService service)
        {
            context.Organisations.Add(organisation);
            await context.SaveChangesAsync();

            filter.Organisation = organisation;
            filter.OrganisationId = organisation.Id;

            context.Filters.Add(filter);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var result = await service.GetFilters(organisation.Id);

            result.Should().NotBeNull();
            result.Count.Should().Be(1);
            result[0].OrganisationId.Should().Be(organisation.Id);
            result[0].Name.Should().Be(filter.Name);
            result[0].Description.Should().Be(filter.Description);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetFilter_ReturnsExpectedResults(
            Filter filter,
            Organisation organisation,
            EntityFramework.Catalogue.Models.Framework framework,
            [Frozen] BuyingCatalogueDbContext context,
            ManageFiltersService service)
        {
            context.Organisations.Add(organisation);
            await context.SaveChangesAsync();

            context.Frameworks.Add(framework);
            await context.SaveChangesAsync();

            filter.Organisation = organisation;
            filter.OrganisationId = organisation.Id;
            filter.Framework = framework;
            filter.FrameworkId = framework.Id;

            await context.SaveChangesAsync();

            context.Filters.Add(filter);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var result = await service.GetFilter(organisation.Id, filter.Id);

            result.Should().NotBeNull();
            result.Id.Should().Be(filter.Id);
            result.OrganisationId.Should().Be(filter.OrganisationId);
            result.Framework.Should().BeEquivalentTo(filter.Framework);
        }
    }
}
