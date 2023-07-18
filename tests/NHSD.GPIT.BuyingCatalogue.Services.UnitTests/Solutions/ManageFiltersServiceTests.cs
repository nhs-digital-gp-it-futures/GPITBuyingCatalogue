using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using LinqKit;
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
            Organisation organisation,
            Filter filter,
            [Frozen] BuyingCatalogueDbContext context,
            ManageFiltersService service)
        {
            context.Organisations.Add(organisation);
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
            Dictionary<int, string[]> capabilityAndEpics,
            string frameworkId,
            List<ApplicationType> applicationTypes,
            List<HostingType> hostingTypes,
            ManageFiltersService service)
        {
            FluentActions
                .Awaiting(
                    () => service.AddFilter(
                        name,
                        description,
                        organisationId,
                        capabilityAndEpics,
                        frameworkId,
                        applicationTypes,
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
            Dictionary<int, string[]> capabilityAndEpics,
            string frameworkId,
            List<ApplicationType> applicationTypes,
            List<HostingType> hostingTypes,
            ManageFiltersService service)
        {
            FluentActions
                .Awaiting(
                    () => service.AddFilter(
                        name,
                        description,
                        organisationId,
                        capabilityAndEpics,
                        frameworkId,
                        applicationTypes,
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
            Dictionary<int, string[]> capabilityAndEpics,
            string frameworkId,
            List<ApplicationType> applicationTypes,
            List<HostingType> hostingTypes,
            ManageFiltersService service)
        {
            FluentActions
                .Awaiting(
                    () => service.AddFilter(
                        name,
                        description,
                        organisationId,
                        capabilityAndEpics,
                        frameworkId,
                        applicationTypes,
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
                framework.Id,
                null,
                null);
            result.Should().NotBe(0);
            context.ChangeTracker.Clear();

            var newFilter = await context.Filters
                .Include(f => f.Capabilities)
                .Include(f => f.FilterCapabilityEpics)
                .FirstAsync(f => f.Id == result);

            newFilter.Should().NotBeNull();
            newFilter.Name.Should().Be(name);
            newFilter.Description.Should().Be(description);
            newFilter.OrganisationId.Should().Be(organisation.Id);
            newFilter.FrameworkId.Should().Be(framework.Id);
            newFilter.Capabilities.Should().BeEmpty();
            newFilter.FilterCapabilityEpics.Should().BeEmpty();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddFilter_ValidParameters_With_Capability_FilterCreated(
            [Frozen] BuyingCatalogueDbContext context,
            string description,
            string name,
            Organisation organisation,
            Capability capability,
            EntityFramework.Catalogue.Models.Framework framework,
            ManageFiltersService service)
        {
            context.Organisations.Add(organisation);
            context.Capabilities.Add(capability);
            await context.SaveChangesAsync();

            context.Frameworks.Add(framework);
            await context.SaveChangesAsync();

            var result = await service.AddFilter(
                name,
                description,
                organisation.Id,
                new Dictionary<int, string[]>() { { capability.Id, null } },
                framework.Id,
                null,
                null);
            result.Should().NotBe(0);
            context.ChangeTracker.Clear();

            var newFilter = await context.Filters
                .Include(f => f.Capabilities)
                .Include(f => f.FilterCapabilityEpics)
                .FirstAsync(f => f.Id == result);

            newFilter.Should().NotBeNull();
            newFilter.Name.Should().Be(name);
            newFilter.Description.Should().Be(description);
            newFilter.OrganisationId.Should().Be(organisation.Id);
            newFilter.FrameworkId.Should().Be(framework.Id);
            newFilter.Capabilities.Count().Should().Be(1);
            newFilter.FilterCapabilityEpics.Should().BeEmpty();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddFilter_ValidParameters_With_Capability_And_Epic_FilterCreated(
            [Frozen] BuyingCatalogueDbContext context,
            string description,
            string name,
            Organisation organisation,
            Capability capability,
            EntityFramework.Catalogue.Models.Framework framework,
            ManageFiltersService service)
        {
            context.Organisations.Add(organisation);
            context.Capabilities.Add(capability);
            await context.SaveChangesAsync();

            context.Frameworks.Add(framework);
            await context.SaveChangesAsync();

            var result = await service.AddFilter(
                name,
                description,
                organisation.Id,
                new Dictionary<int, string[]>() { { capability.Id, new string[] { capability.Epics.First().Id } } },
                framework.Id,
                null,
                null);
            result.Should().NotBe(0);
            context.ChangeTracker.Clear();

            var newFilter = await context.Filters
                .Include(f => f.Capabilities)
                .Include(f => f.FilterCapabilityEpics)
                .FirstAsync(f => f.Id == result);

            newFilter.Should().NotBeNull();
            newFilter.Name.Should().Be(name);
            newFilter.Description.Should().Be(description);
            newFilter.OrganisationId.Should().Be(organisation.Id);
            newFilter.FrameworkId.Should().Be(framework.Id);
            newFilter.Capabilities.Count().Should().Be(1);
            newFilter.FilterCapabilityEpics.Count().Should().Be(1);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddFilterCapabilities_NullCapabilityIds_NoCapabilitiesAdded(
            Organisation organisation,
            Filter filter,
            [Frozen] BuyingCatalogueDbContext context,
            ManageFiltersService service)
        {
            filter.Capabilities.Clear();

            context.Organisations.Add(organisation);
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
            Organisation organisation,
            Filter filter,
            [Frozen] BuyingCatalogueDbContext context,
            ManageFiltersService service)
        {
            filter.Capabilities.Clear();

            context.Organisations.Add(organisation);
            context.Filters.Add(filter);
            await context.SaveChangesAsync();

            await service.AddFilterCapabilities(filter.Id, new Dictionary<int, string[]>());
            context.ChangeTracker.Clear();

            var result = await context.Filters.FirstAsync(f => f.Id == filter.Id);
            result.Should().NotBeNull();

            result.Capabilities.Should().BeEmpty();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddFilterCapabilities_NullFilter_NoCapabilitiesAdded(
            [Frozen] BuyingCatalogueDbContext context,
            Dictionary<int, string[]> capabilityEpicIds,
            int invalidFilterId,
            ManageFiltersService service)
        {
            await service.AddFilterCapabilities(invalidFilterId, capabilityEpicIds);
            context.ChangeTracker.Clear();

            var filter = await context.Filters.FirstOrDefaultAsync(f => f.Id == invalidFilterId);
            filter.Should().BeNull();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddFilterCapabilities_ValidParameters_CapabilitiesAdded(
            Organisation organisation,
            Capability capability,
            Filter filter,
            [Frozen] BuyingCatalogueDbContext context,
            ManageFiltersService service)
        {
            filter.Capabilities.Clear();
            context.Organisations.Add(organisation);
            context.Capabilities.Add(capability);
            context.Filters.Add(filter);

            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            await service.AddFilterCapabilities(filter.Id, new Dictionary<int, string[]>() { { capability.Id, null } });

            var result = await context.Filters.Include(f => f.Capabilities).FirstAsync(f => f.Id == filter.Id);
            result.Should().NotBeNull();

            result.Capabilities.Should().NotBeNullOrEmpty();
            result.Capabilities.Count.Should().Be(1);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddFilterCapabilityEpics_NullCapabilityIds_NoCapabilitiesAdded(
            Organisation organisation,
            Filter filter,
            [Frozen] BuyingCatalogueDbContext context,
            ManageFiltersService service)
        {
            filter.FilterCapabilityEpics.Clear();

            context.Organisations.Add(organisation);
            context.Filters.Add(filter);
            await context.SaveChangesAsync();

            await service.AddFilterCapabilityEpics(filter.Id, null);
            context.ChangeTracker.Clear();

            var result = await context.Filters.FirstAsync(f => f.Id == filter.Id);
            result.Should().NotBeNull();

            result.FilterCapabilityEpics.Should().BeEmpty();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddFilterCapabilityEpics_EmptyCapabilityIds_NoCapabilitiesAdded(
            Organisation organisation,
            Filter filter,
            [Frozen] BuyingCatalogueDbContext context,
            ManageFiltersService service)
        {
            filter.FilterCapabilityEpics.Clear();

            context.Organisations.Add(organisation);
            context.Filters.Add(filter);
            await context.SaveChangesAsync();

            await service.AddFilterCapabilityEpics(filter.Id, new Dictionary<int, string[]>());
            context.ChangeTracker.Clear();

            var result = await context.Filters.FirstAsync(f => f.Id == filter.Id);
            result.Should().NotBeNull();

            result.FilterCapabilityEpics.Should().BeEmpty();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddFilterCapabilityEpics_NullFilter_NoCapabilitiesAdded(
            [Frozen] BuyingCatalogueDbContext context,
            Dictionary<int, string[]> capabilityEpicIds,
            int invalidFilterId,
            ManageFiltersService service)
        {
            await service.AddFilterCapabilityEpics(invalidFilterId, capabilityEpicIds);
            context.ChangeTracker.Clear();

            var filter = await context.Filters.FirstOrDefaultAsync(f => f.Id == invalidFilterId);
            filter.Should().BeNull();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddFilterCapabilityEpics_ValidParameters_CapabilityEpics_JustCapabilities_Added(
            Organisation organisation,
            Filter filter,
            [Frozen] BuyingCatalogueDbContext context,
            ManageFiltersService service)
        {
            filter.FilterCapabilityEpics.Clear();

            context.Organisations.Add(organisation);
            context.Filters.Add(filter);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            await service.AddFilterCapabilityEpics(filter.Id, new Dictionary<int, string[]>() { { 1, null } });
            context.ChangeTracker.Clear();

            var result = await context.Filters
                .Include(f => f.FilterCapabilityEpics)
                .FirstOrDefaultAsync(f => f.Id == filter.Id);

            result.Should().NotBeNull();
            result.FilterCapabilityEpics.Should().BeEmpty();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddFilterCapabilityEpics_ValidParameters_CapabilityEpicsAdded(
            List<Epic> epics,
            Organisation organisation,
            Filter filter,
            [Frozen] BuyingCatalogueDbContext context,
            ManageFiltersService service)
        {
            filter.FilterCapabilityEpics.Clear();

            context.Epics.AddRange(epics);
            context.Organisations.Add(organisation);
            context.Filters.Add(filter);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var epicIds = epics.Select(c => c.Id).ToList();

            await service.AddFilterCapabilityEpics(filter.Id, new Dictionary<int, string[]>() { { 1, epicIds.ToArray() } });
            context.ChangeTracker.Clear();

            var result = await context.Filters
                .Include(f => f.FilterCapabilityEpics)
                .FirstOrDefaultAsync(f => f.Id == filter.Id);

            result.Should().NotBeNull();
            result.FilterCapabilityEpics.Should().NotBeNullOrEmpty();
            result.FilterCapabilityEpics.Count.Should().Be(epics.Count);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddFilterCapabilityEpics_ValidParameters_SameEpics_DifferentCapabilties_Added(
            List<Epic> epics,
            Organisation organisation,
            Filter filter,
            [Frozen] BuyingCatalogueDbContext context,
            ManageFiltersService service)
        {
            filter.FilterCapabilityEpics.Clear();

            context.Epics.AddRange(epics);
            context.Organisations.Add(organisation);
            context.Filters.Add(filter);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var epicIds = epics.Select(c => c.Id).ToList();

            await service.AddFilterCapabilityEpics(filter.Id, new Dictionary<int, string[]>() { { 1, epicIds.ToArray() }, { 2, epicIds.ToArray() } });
            context.ChangeTracker.Clear();

            var result = await context.Filters
                .Include(f => f.FilterCapabilityEpics)
                .FirstOrDefaultAsync(f => f.Id == filter.Id);

            result.Should().NotBeNull();
            result.FilterCapabilityEpics.Should().NotBeNullOrEmpty();
            result.FilterCapabilityEpics.Count.Should().Be(epics.Count * 2);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddFilterApplicationTypes_NullCatIds_NoCatsAdded(
            [Frozen] BuyingCatalogueDbContext context,
            Organisation organisation,
            Filter filter,
            ManageFiltersService service)
        {
            filter.FilterApplicationTypes.Clear();
            context.Organisations.Add(organisation);
            context.Filters.Add(filter);
            await context.SaveChangesAsync();

            await service.AddFilterApplicationTypes(filter.Id, null);
            context.ChangeTracker.Clear();

            var result = await context.Filters.FirstAsync(f => f.Id == filter.Id);
            result.Should().NotBeNull();

            result.FilterApplicationTypes.Should().BeEmpty();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddFilterApplicationTypes_EmptyCatIds_NoCatsAdded(
            [Frozen] BuyingCatalogueDbContext context,
            Organisation organisation,
            Filter filter,
            ManageFiltersService service)
        {
            filter.FilterApplicationTypes.Clear();
            context.Organisations.Add(organisation);
            context.Filters.Add(filter);
            await context.SaveChangesAsync();

            await service.AddFilterApplicationTypes(filter.Id, new List<ApplicationType>());
            context.ChangeTracker.Clear();

            var result = await context.Filters.FirstAsync(f => f.Id == filter.Id);
            result.Should().NotBeNull();

            result.FilterApplicationTypes.Should().BeEmpty();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddFilterApplicationTypes_NullFilter_NoCatsAdded(
            [Frozen] BuyingCatalogueDbContext context,
            List<ApplicationType> cats,
            int invalidFilterId,
            ManageFiltersService service)
        {
            await service.AddFilterApplicationTypes(invalidFilterId, cats);
            context.ChangeTracker.Clear();

            var filter = await context.Filters.FirstOrDefaultAsync(f => f.Id == invalidFilterId);
            filter.Should().BeNull();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddFilterApplicationTypes_ValidParameters_CatsAdded(
            [Frozen] BuyingCatalogueDbContext context,
            Organisation organisation,
            Filter filter,
            List<ApplicationType> cats,
            ManageFiltersService service)
        {
            filter.FilterApplicationTypes.Clear();
            context.Organisations.Add(organisation);
            context.Filters.Add(filter);
            await context.SaveChangesAsync();

            await service.AddFilterApplicationTypes(filter.Id, cats);
            context.ChangeTracker.Clear();

            var result = await context.Filters.Include(f => f.FilterApplicationTypes).FirstOrDefaultAsync(f => f.Id == filter.Id);
            result.Should().NotBeNull();

            result.FilterApplicationTypes.Should().NotBeNullOrEmpty();
            result.FilterApplicationTypes.Count.Should().Be(cats.Count);

            foreach (var x in result.FilterApplicationTypes)
            {
                x.FilterId.Should().Be(filter.Id);
                cats.Should().Contain(x.ApplicationTypeID);
            }
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddFilterHostingTypes_NullHostingTypeIds_NoHostingTypesAdded(
            [Frozen] BuyingCatalogueDbContext context,
            Organisation organisation,
            Filter filter,
            ManageFiltersService service)
        {
            filter.FilterHostingTypes.Clear();
            context.Organisations.Add(organisation);
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
            Organisation organisation,
            Filter filter,
            [Frozen] BuyingCatalogueDbContext context,
            ManageFiltersService service)
        {
            filter.FilterHostingTypes.Clear();
            context.Organisations.Add(organisation);
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
            List<HostingType> hostingTypes,
            Organisation organisation,
            Filter filter,
            [Frozen] BuyingCatalogueDbContext context,
            ManageFiltersService service)
        {
            filter.FilterHostingTypes.Clear();
            context.Organisations.Add(organisation);
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
            Organisation organisation,
            Filter filter,
            EntityFramework.Catalogue.Models.Framework framework,
            [Frozen] BuyingCatalogueDbContext context,
            ManageFiltersService service)
        {
            context.Organisations.Add(organisation);
            context.Filters.Add(filter);

            await context.SaveChangesAsync();

            context.Frameworks.Add(framework);
            await context.SaveChangesAsync();

            filter.Organisation = organisation;
            filter.OrganisationId = organisation.Id;
            filter.Framework = framework;
            filter.FrameworkId = framework.Id;

            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();

            var result = await service.GetFilters(organisation.Id);

            result.Should().NotBeNull();
            result.Count.Should().Be(1);
            result[0].OrganisationId.Should().Be(organisation.Id);
            result[0].FrameworkId.Should().BeEquivalentTo(framework.Id);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetFilterDetails_ReturnsExpected(
            EntityFramework.Catalogue.Models.Framework framework,
            Filter filter,
            [Frozen] BuyingCatalogueDbContext context,
            ManageFiltersService service)
        {
            filter.Framework = null;
            filter.FrameworkId = framework.Id;

            context.Frameworks.Add(framework);
            context.Filters.Add(filter);

            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var filterDetails = await service.GetFilterDetails(filter.OrganisationId, filter.Id);

            filterDetails.Name.Should().Be(filter.Name);
            filterDetails.Description.Should().Be(filter.Description);
            filterDetails.FrameworkName.Should().Be(framework.ShortName);
            filterDetails.Id.Should().Be(filter.Id);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetFilterIds_ReturnsExpected(
            EntityFramework.Catalogue.Models.Framework framework,
            Filter filter,
            [Frozen] BuyingCatalogueDbContext context,
            ManageFiltersService service)
        {
            var epic = filter.Capabilities.First().Epics.First();
            filter.FilterCapabilityEpics.Clear();
            filter.Capabilities.ForEach(c => filter.FilterCapabilityEpics.Add(new FilterCapabilityEpic()
            {
                CapabilityId = c.Id,
                EpicId = epic.Id,
            }));

            filter.Framework = null;
            filter.FrameworkId = framework.Id;

            context.Frameworks.Add(framework);
            context.Filters.Add(filter);

            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var filterIds = await service.GetFilterIds(filter.OrganisationId, filter.Id);

            var capabilityAndEpicIds = new Dictionary<int, string[]>(filter.Capabilities
                .Select(
                    y => new KeyValuePair<int, string[]>(
                        y.Id,
                        filter.FilterCapabilityEpics.Where(z => z.CapabilityId == y.Id)
                            .Select(z => z.EpicId)
                            .ToArray())));

            filterIds.CapabilityAndEpicIds.Should().BeEquivalentTo(capabilityAndEpicIds);
            filterIds.FrameworkId.Should().Be(filter.FrameworkId);
            filterIds.ApplicationTypeIds.Should().BeEquivalentTo(filter.FilterApplicationTypes.Select(fc => (int)fc.ApplicationTypeID));
            filterIds.HostingTypeIds.Should().BeEquivalentTo(filter.FilterHostingTypes.Select(fc => (int)fc.HostingType));
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task DeleteFilter_SoftDeleteFilter_ValidateDeletion(
            Organisation organisation,
            Filter filter,
            [Frozen] BuyingCatalogueDbContext context,
            ManageFiltersService service)
        {
            filter.IsDeleted = false;
            context.Organisations.Add(organisation);
            context.Filters.Add(filter);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            await service.DeleteFilter(filter.Id);

            var result = await context.Filters.FirstOrDefaultAsync(f => f.Id == filter.Id);

            result.Should().BeNull();
        }
    }
}
