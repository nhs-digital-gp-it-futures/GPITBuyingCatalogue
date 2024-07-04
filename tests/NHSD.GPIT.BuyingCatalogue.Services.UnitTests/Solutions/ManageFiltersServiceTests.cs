using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
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
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Solutions
{
    public static class ManageFiltersServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(ManageFiltersService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        [MockInlineAutoData(" ")]
        public static Task FilterExists_NullOrEmptyFilterName_ThrowsException(
            string name,
            ManageFiltersService service,
            int organisationId)
        {
            return Assert.ThrowsAsync<ArgumentNullException>(() => service.FilterExists(name, organisationId));
        }

        [Theory]
        [MockInMemoryDbAutoData]
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
        [MockInMemoryDbAutoData]
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
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        [MockInlineAutoData(" ")]
        public static void AddFilter_NullOrEmptyName_ThrowsException(
            string name,
            string description,
            int organisationId,
            Dictionary<int, string[]> capabilityAndEpics,
            string frameworkId,
            List<ApplicationType> applicationTypes,
            List<HostingType> hostingTypes,
            Dictionary<SupportedIntegrations, int[]> integrations,
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
                        hostingTypes,
                        integrations))
                .Should()
                .ThrowAsync<ArgumentNullException>(nameof(name));
        }

        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        [MockInlineAutoData(" ")]
        public static void AddFilter_NullOrEmptyDescription_ThrowsException(
            string description,
            string name,
            int organisationId,
            Dictionary<int, string[]> capabilityAndEpics,
            string frameworkId,
            List<ApplicationType> applicationTypes,
            List<HostingType> hostingTypes,
            Dictionary<SupportedIntegrations, int[]> integrations,
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
                        hostingTypes,
                        integrations))
                .Should()
                .ThrowAsync<ArgumentNullException>(nameof(description));
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static void AddFilter_NullOrganisation_ThrowsException(
            string description,
            string name,
            int organisationId,
            Dictionary<int, string[]> capabilityAndEpics,
            string frameworkId,
            List<ApplicationType> applicationTypes,
            List<HostingType> hostingTypes,
            Dictionary<SupportedIntegrations, int[]> integrations,
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
                        hostingTypes,
                        integrations))
                .Should()
                .ThrowAsync<ArgumentException>("organisationId");
        }

        [Theory]
        [MockInMemoryDbAutoData]
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
                Enumerable.Empty<ApplicationType>(),
                Enumerable.Empty<HostingType>(),
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
        [MockInMemoryDbAutoData]
        public static async Task AddFilter_WithIntegrations_AddsIntegrationsToFilter(
            string description,
            string name,
            Organisation organisation,
            Integration integration,
            List<IntegrationType> integrationTypes,
            EntityFramework.Catalogue.Models.Framework framework,
            [Frozen] BuyingCatalogueDbContext context,
            ManageFiltersService service)
        {
            integrationTypes.ForEach(x =>
            {
                x.Integration = integration;
                x.IntegrationId = integration.Id;
            });

            integration.IntegrationTypes = integrationTypes;

            context.Integrations.Add(integration);
            context.Organisations.Add(organisation);
            context.Frameworks.Add(framework);
            await context.SaveChangesAsync();

            var result = await service.AddFilter(
                name,
                description,
                organisation.Id,
                null,
                framework.Id,
                Enumerable.Empty<ApplicationType>(),
                Enumerable.Empty<HostingType>(),
                new Dictionary<SupportedIntegrations, int[]>
                {
                    { integration.Id, integrationTypes.Select(x => x.Id).ToArray() },
                });

            result.Should().NotBe(0);
            context.ChangeTracker.Clear();

            var newFilter = await context.Filters
                .Include(f => f.Integrations)
                .ThenInclude(x => x.IntegrationTypes)
                .FirstAsync(f => f.Id == result);

            newFilter.Should().NotBeNull();
            newFilter.Integrations.Should().ContainSingle();
            newFilter.Integrations.Should().Contain(x => x.IntegrationId == integration.Id);
            newFilter.Integrations.Should()
                .Contain(x => x.IntegrationTypes.All(y => integrationTypes.Any(z => z.Id == y.IntegrationTypeId)));
        }

        [Theory]
        [MockInMemoryDbAutoData]
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
                Enumerable.Empty<ApplicationType>(),
                Enumerable.Empty<HostingType>(),
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
        [MockInMemoryDbAutoData]
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
                Enumerable.Empty<ApplicationType>(),
                Enumerable.Empty<HostingType>(),
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
        [MockInMemoryDbAutoData]
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

            await service.AddFilterCapabilities(filter, null);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var result = await context.Filters.Include(x => x.Capabilities).FirstAsync(f => f.Id == filter.Id);
            result.Should().NotBeNull();

            result.Capabilities.Should().BeEmpty();
        }

        [Theory]
        [MockInMemoryDbAutoData]
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

            await service.AddFilterCapabilities(filter, new Dictionary<int, string[]>());
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var result = await context.Filters.Include(x => x.Capabilities).FirstAsync(f => f.Id == filter.Id);
            result.Should().NotBeNull();

            result.Capabilities.Should().BeEmpty();
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static Task AddFilterCapabilities_NullFilter_ThrowsArgumentNullException(
            Dictionary<int, string[]> capabilityEpicIds,
            ManageFiltersService service) => FluentActions
            .Awaiting(() => service.AddFilterCapabilities(null, capabilityEpicIds))
            .Should()
            .ThrowAsync<ArgumentNullException>();

        [Theory]
        [MockInMemoryDbAutoData]
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
            await service.AddFilterCapabilities(filter, new Dictionary<int, string[]> { { capability.Id, null } });
            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();

            var result = await context.Filters.Include(f => f.Capabilities).FirstAsync(f => f.Id == filter.Id);
            result.Should().NotBeNull();

            result.Capabilities.Should().NotBeNullOrEmpty();
            result.Capabilities.Count.Should().Be(1);
        }

        [Theory]
        [MockInMemoryDbAutoData]
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

            service.AddFilterCapabilityEpics(filter, null);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var result = await context.Filters.Include(x => x.FilterCapabilityEpics).FirstAsync(f => f.Id == filter.Id);
            result.Should().NotBeNull();

            result.FilterCapabilityEpics.Should().BeEmpty();
        }

        [Theory]
        [MockInMemoryDbAutoData]
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

            service.AddFilterCapabilityEpics(filter, new Dictionary<int, string[]>());
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var result = await context.Filters.Include(x => x.FilterCapabilityEpics).FirstAsync(f => f.Id == filter.Id);
            result.Should().NotBeNull();

            result.FilterCapabilityEpics.Should().BeEmpty();
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static void AddFilterCapabilityEpics_NullFilter_ThrowsArgumentNullException(
            Dictionary<int, string[]> capabilityEpicIds,
            ManageFiltersService service) => FluentActions
            .Invoking(() => service.AddFilterCapabilityEpics(null, capabilityEpicIds))
            .Should()
            .Throw<ArgumentNullException>();

        [Theory]
        [MockInMemoryDbAutoData]
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

            service.AddFilterCapabilityEpics(filter, new Dictionary<int, string[]>() { { 1, null } });
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var result = await context.Filters
                .Include(f => f.FilterCapabilityEpics)
                .FirstOrDefaultAsync(f => f.Id == filter.Id);

            result.Should().NotBeNull();
            result.FilterCapabilityEpics.Should().BeEmpty();
        }

        [Theory]
        [MockInMemoryDbAutoData]
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

            var epicIds = epics.Select(c => c.Id).ToList();

            service.AddFilterCapabilityEpics(filter, new Dictionary<int, string[]> { { 1, epicIds.ToArray() } });
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var result = await context.Filters
                .Include(f => f.FilterCapabilityEpics)
                .FirstOrDefaultAsync(f => f.Id == filter.Id);

            result.Should().NotBeNull();
            result.FilterCapabilityEpics.Should().NotBeNullOrEmpty();
            result.FilterCapabilityEpics.Count.Should().Be(epics.Count);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task AddFilterCapabilityEpics_ValidParameters_SameEpics_DifferentCapabilities_Added(
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

            var epicIds = epics.Select(c => c.Id).ToList();

            service.AddFilterCapabilityEpics(filter, new Dictionary<int, string[]>() { { 1, epicIds.ToArray() }, { 2, epicIds.ToArray() } });
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var result = await context.Filters
                .Include(f => f.FilterCapabilityEpics)
                .FirstOrDefaultAsync(f => f.Id == filter.Id);

            result.Should().NotBeNull();
            result.FilterCapabilityEpics.Should().NotBeNullOrEmpty();
            result.FilterCapabilityEpics.Count.Should().Be(epics.Count * 2);
        }

        [Theory]
        [MockInMemoryDbAutoData]
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
        [MockInMemoryDbAutoData]
        public static async Task GetFilterDetails_ReturnsExpected(
            EntityFramework.Catalogue.Models.Framework framework,
            Filter filter,
            [Frozen] BuyingCatalogueDbContext context,
            ManageFiltersService service)
        {
            filter.IsDeleted = false;
            filter.Framework = null;
            filter.FrameworkId = framework.Id;
            filter.Capabilities.ForEach(c => c.Status = CapabilityStatus.Effective);
            filter.FilterCapabilityEpics.ForEach(c => c.Epic.IsActive = true);
            filter.FilterCapabilityEpics.ForEach(c =>
            {
                c.CapabilityId = c.Capability.Id;
                c.EpicId = c.Epic.Id;
                c.Capability.Epics.Clear();
                c.Capability.CapabilityEpics.Clear();
                c.Capability.CapabilityEpics.Add(new CapabilityEpic()
                {
                    CapabilityId = c.CapabilityId.Value,
                    EpicId = c.EpicId,
                    CompliancyLevel = CompliancyLevel.Must,
                });
            });

            context.Frameworks.Add(framework);
            context.Filters.Add(filter);

            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var filterDetails = await service.GetFilterDetails(filter.OrganisationId, filter.Id);

            filterDetails.Name.Should().Be(filter.Name);
            filterDetails.Description.Should().Be(filter.Description);
            filterDetails.FrameworkName.Should().Be(framework.ShortName);
            filterDetails.Id.Should().Be(filter.Id);
            filterDetails.Invalid.Should().BeFalse();
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetFilterDetails_With_Epics_With_No_Capability_Context_Is_Invalid(
            EntityFramework.Catalogue.Models.Framework framework,
            Filter filter,
            [Frozen] BuyingCatalogueDbContext context,
            ManageFiltersService service)
        {
            filter.IsDeleted = false;
            filter.Framework = null;
            filter.FrameworkId = framework.Id;
            filter.Capabilities.ForEach(c => c.Status = CapabilityStatus.Effective);
            filter.FilterCapabilityEpics.ForEach(c => c.Epic.IsActive = true);
            filter.FilterCapabilityEpics.ForEach(c =>
            {
                c.Capability = null;
                c.CapabilityId = null;
            });

            context.Frameworks.Add(framework);
            context.Filters.Add(filter);

            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var filterDetails = await service.GetFilterDetails(filter.OrganisationId, filter.Id);

            filterDetails.Invalid.Should().BeTrue();
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetFilterDetails_With_Expired_Capabilities_Is_Invalid(
            EntityFramework.Catalogue.Models.Framework framework,
            Filter filter,
            [Frozen] BuyingCatalogueDbContext context,
            ManageFiltersService service)
        {
            filter.IsDeleted = false;
            filter.Framework = null;
            filter.FrameworkId = framework.Id;
            filter.Capabilities.ForEach(c => c.Status = CapabilityStatus.Expired);
            filter.FilterCapabilityEpics.ForEach(c => c.Epic.IsActive = true);

            context.Frameworks.Add(framework);
            context.Filters.Add(filter);

            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var filterDetails = await service.GetFilterDetails(filter.OrganisationId, filter.Id);

            filterDetails.Invalid.Should().BeTrue();
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetFilterDetails_With_IsActive_False_Epics_Is_Invalid(
            EntityFramework.Catalogue.Models.Framework framework,
            Filter filter,
            [Frozen] BuyingCatalogueDbContext context,
            ManageFiltersService service)
        {
            filter.IsDeleted = false;
            filter.Framework = null;
            filter.FrameworkId = framework.Id;
            filter.Capabilities.ForEach(c => c.Status = CapabilityStatus.Effective);
            filter.FilterCapabilityEpics.ForEach(c => c.Epic.IsActive = false);

            context.Frameworks.Add(framework);
            context.Filters.Add(filter);

            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var filterDetails = await service.GetFilterDetails(filter.OrganisationId, filter.Id);

            filterDetails.Invalid.Should().BeTrue();
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetFilterDetails_With_Epics_No_Longer_Linked_To_Capability_Is_Invalid(
            EntityFramework.Catalogue.Models.Framework framework,
            Filter filter,
            [Frozen] BuyingCatalogueDbContext context,
            ManageFiltersService service)
        {
            filter.IsDeleted = false;
            filter.Framework = null;
            filter.FrameworkId = framework.Id;
            filter.Capabilities.ForEach(c => c.Status = CapabilityStatus.Effective);
            filter.FilterCapabilityEpics.ForEach(c => c.Epic.IsActive = true);
            filter.FilterCapabilityEpics.ForEach(c =>
            {
                c.CapabilityId = c.Capability.Id;
                c.EpicId = c.Epic.Id;
                c.Capability.Epics.Clear();
                c.Capability.CapabilityEpics.Clear();
            });

            context.Frameworks.Add(framework);
            context.Filters.Add(filter);

            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var filterDetails = await service.GetFilterDetails(filter.OrganisationId, filter.Id);

            filterDetails.Name.Should().Be(filter.Name);
            filterDetails.Description.Should().Be(filter.Description);
            filterDetails.FrameworkName.Should().Be(framework.ShortName);
            filterDetails.Id.Should().Be(filter.Id);
            filterDetails.Invalid.Should().BeTrue();
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetFilterIds_ReturnsExpected(
            EntityFramework.Catalogue.Models.Framework framework,
            Filter filter,
            [Frozen] BuyingCatalogueDbContext context,
            ManageFiltersService service)
        {
            var epic = filter.Capabilities.First().Epics.First();
            filter.IsDeleted = false;
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

            var integrations = new Dictionary<SupportedIntegrations, int[]>(
                filter.Integrations.Select(
                    y =>
                        new KeyValuePair<SupportedIntegrations, int[]>(
                            y.IntegrationId,
                            y.IntegrationTypes.Select(
                                    z => z.IntegrationTypeId)
                                .ToArray())));

            filterIds.CapabilityAndEpicIds.Should().BeEquivalentTo(capabilityAndEpicIds);
            filterIds.FrameworkId.Should().Be(filter.FrameworkId);
            filterIds.ApplicationTypeIds.Should().BeEquivalentTo(filter.FilterApplicationTypes.Select(fc => (int)fc.ApplicationTypeID));
            filterIds.HostingTypeIds.Should().BeEquivalentTo(filter.FilterHostingTypes.Select(fc => (int)fc.HostingType));
            filterIds.IntegrationsIds.Should().BeEquivalentTo(integrations);
        }

        [Theory]
        [MockInMemoryDbAutoData]
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
