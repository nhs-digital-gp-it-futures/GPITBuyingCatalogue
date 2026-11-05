using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using EnumsNET;
using FluentAssertions;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Admin;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.Services.Solutions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.TestData;
using NSubstitute;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Solutions
{
    public static class SolutionsServiceTests
    {
        private const string SolutionName = "SolutionName";
        private const string SupplierName = "SupplierName";
        private const string Junk = "Junk";

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(SolutionsService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetSolutionWithBasicInformation_Valid_Returns(
            Solution solution,
            [Frozen] BuyingCatalogueDbContext dbContext,
            SolutionsService service)
        {
            dbContext.Solutions.Add(solution);
            await dbContext.SaveChangesAsync();
            dbContext.ChangeTracker.Clear();

            var result = await service.GetSolutionWithBasicInformation(solution.CatalogueItemId);

            result.Should().NotBeNull();
            result.Id.Should().Be(solution.CatalogueItemId);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetContentStatusForCatalogueItem_WithAllOptionals_ReturnsExpected(
            string features,
            string implementationDetail,
            string integrationsUrl,
            List<AdditionalService> additionalServices,
            List<SupplierServiceAssociation> serviceAssociations,
            List<SolutionIntegration> solutionIntegrations,
            Hosting hosting,
            Solution solution,
            [Frozen] BuyingCatalogueDbContext dbContext,
            SolutionsService service)
        {
            additionalServices.ForEach(x => x.CatalogueItem.PublishedStatus = PublicationStatus.Published);
            serviceAssociations.ForEach(x => x.CatalogueItem.PublishedStatus = PublicationStatus.Published);

            solution.Features = features;
            solution.ImplementationDetail = implementationDetail;
            solution.IntegrationsUrl = integrationsUrl;
            solution.AdditionalServices = additionalServices;
            solution.CatalogueItem.SupplierServiceAssociations = serviceAssociations;
            solution.Integrations = solutionIntegrations;
            solution.Hosting = hosting;

            var expected = new CatalogueItemContentStatus
            {
                ShowFeatures = true,
                ShowAdditionalServices = true,
                ShowAssociatedServices = true,
                ShowInteroperability = true,
                ShowImplementation = true,
                ShowHosting = true,
            };

            dbContext.Solutions.Add(solution);
            await dbContext.SaveChangesAsync();
            dbContext.ChangeTracker.Clear();

            var result = await service.GetContentStatusForCatalogueItem(solution.CatalogueItemId);

            result.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetContentStatusForCatalogueItem_WithNoOptionals_ReturnsExpected(
            Solution solution,
            [Frozen] BuyingCatalogueDbContext dbContext,
            SolutionsService service)
        {
            solution.Features = null;
            solution.ImplementationDetail = null;
            solution.IntegrationsUrl = null;
            solution.AdditionalServices = new List<AdditionalService>();
            solution.CatalogueItem.SupplierServiceAssociations = null;
            solution.Integrations = null;
            solution.Hosting = null;

            var expected = new CatalogueItemContentStatus
            {
                ShowFeatures = false,
                ShowAdditionalServices = false,
                ShowAssociatedServices = false,
                ShowInteroperability = false,
                ShowImplementation = false,
                ShowHosting = false,
            };

            dbContext.Solutions.Add(solution);
            await dbContext.SaveChangesAsync();
            dbContext.AdditionalServices.RemoveRange(dbContext.AdditionalServices);
            await dbContext.SaveChangesAsync();
            dbContext.ChangeTracker.Clear();

            var result = await service.GetContentStatusForCatalogueItem(solution.CatalogueItemId);

            result.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetContentStatusForCatalogueItem_WithUnpublishedAdditionalServices_ReturnsExpected(
            Solution solution,
            [Frozen] BuyingCatalogueDbContext dbContext,
            SolutionsService service)
        {
            solution.AdditionalServices.ForEach(x => x.CatalogueItem.PublishedStatus = PublicationStatus.Unpublished);

            dbContext.Solutions.Add(solution);
            await dbContext.SaveChangesAsync();
            dbContext.ChangeTracker.Clear();

            var result = await service.GetContentStatusForCatalogueItem(solution.CatalogueItemId);

            result.ShowAdditionalServices.Should().BeFalse();
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetContentStatusForCatalogueItem_WithIntegrationsUrlNoIntegrations_ReturnsExpected(
            string integrationsUrl,
            Solution solution,
            [Frozen] BuyingCatalogueDbContext dbContext,
            SolutionsService service)
        {
            solution.IntegrationsUrl = integrationsUrl;
            solution.Integrations = new List<SolutionIntegration>();

            dbContext.Solutions.Add(solution);
            await dbContext.SaveChangesAsync();
            dbContext.ChangeTracker.Clear();

            var result = await service.GetContentStatusForCatalogueItem(solution.CatalogueItemId);

            result.ShowInteroperability.Should().BeTrue();
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetSolutionWithDataProcessingInformation_Valid_Returns(
            Solution solution,
            [Frozen] BuyingCatalogueDbContext dbContext,
            SolutionsService service)
        {
            dbContext.CatalogueItems.Add(solution.CatalogueItem);
            dbContext.SaveChanges();

            var result = await service.GetSolutionWithDataProcessingInformation(solution.CatalogueItemId);

            result.Should().NotBeNull();
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetSolutionWithCapabilities_Valid_Returns(
            Solution solution,
            [Frozen] BuyingCatalogueDbContext dbContext,
            SolutionsService service)
        {
            dbContext.CatalogueItems.Add(solution.CatalogueItem);
            dbContext.SaveChanges();

            var result = await service.GetSolutionWithCapabilities(solution.CatalogueItemId);

            result.Should().NotBeNull();
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetSolutionWithCapabilities_Invalid_ReturnsNull(
            Solution solution,
            SolutionsService service)
        {
            var result = await service.GetSolutionWithCapabilities(solution.CatalogueItemId);

            result.Should().BeNull();
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetSolutionWithServiceLevelAgreements_Valid_Returns(
            Solution solution,
            [Frozen] BuyingCatalogueDbContext dbContext,
            SolutionsService service)
        {
            dbContext.CatalogueItems.Add(solution.CatalogueItem);
            dbContext.SaveChanges();

            var result = await service.GetSolutionWithServiceLevelAgreements(solution.CatalogueItemId);

            result.Should().NotBeNull();
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetSolutionWithServiceLevelAgreements_Invalid_ReturnsNull(
            Solution solution,
            SolutionsService service)
        {
            var result = await service.GetSolutionWithServiceLevelAgreements(solution.CatalogueItemId);

            result.Should().BeNull();
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetSolutionWithCataloguePrice_Valid_Returns(
            Solution solution,
            [Frozen] BuyingCatalogueDbContext dbContext,
            SolutionsService service)
        {
            dbContext.CatalogueItems.Add(solution.CatalogueItem);
            dbContext.SaveChanges();

            var result = await service.GetSolutionWithCataloguePrice(solution.CatalogueItemId);

            result.Should().NotBeNull();
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetSolutionWithCataloguePrice_Invalid_ReturnsNull(
            Solution solution,
            SolutionsService service)
        {
            var result = await service.GetSolutionWithCataloguePrice(solution.CatalogueItemId);

            result.Should().BeNull();
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetSolutionWithSupplierDetails_Valid_Returns(
            Solution solution,
            [Frozen] BuyingCatalogueDbContext dbContext,
            SolutionsService service)
        {
            dbContext.CatalogueItems.Add(solution.CatalogueItem);
            dbContext.SaveChanges();

            var result = await service.GetSolutionWithSupplierDetails(solution.CatalogueItemId);

            result.Should().NotBeNull();
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetSolutionWithSupplierDetails_Invalid_ReturnsNull(
            Solution solution,
            SolutionsService service)
        {
            var result = await service.GetSolutionWithSupplierDetails(solution.CatalogueItemId);

            result.Should().BeNull();
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetSolutionLoadingStatuses_With_Null_ApplicationType_Should_be_Status_NotStarted(
            [Frozen] BuyingCatalogueDbContext context,
            Solution solution,
            SolutionsService service)
        {
            solution.ApplicationTypeDetail = null;
            context.Solutions.Add(solution);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var actual = await service.GetSolutionLoadingStatuses(solution.CatalogueItemId, new SolutionSections { ApplicationType = SolutionSectionRequirement.Mandatory });
            actual.ApplicationType.Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetSolutionLoadingStatuses_With_ApplicationType_Should_be_Status_Completed(
            [Frozen] BuyingCatalogueDbContext context,
            Solution solution,
            SolutionsService service)
        {
            context.Solutions.Add(solution);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var actual = await service.GetSolutionLoadingStatuses(solution.CatalogueItemId, new SolutionSections { ApplicationType = SolutionSectionRequirement.Mandatory });
            actual.ApplicationType.Should().Be(TaskProgress.Completed);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetSolutionLoadingStatuses_With_InProgressStandards_Should_be_Status_InProgress(
            Solution solution,
            List<Standard> standards,
            [Frozen] BuyingCatalogueDbContext context,
            SolutionsService service)
        {
            solution.SolutionStandards = standards.Select(x => new SolutionStandard { Standard = x, Status = StandardCompliance.InProgress }).ToList();
            context.Solutions.Add(solution);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var actual = await service.GetSolutionLoadingStatuses(solution.CatalogueItemId, new SolutionSections { Standards = SolutionSectionRequirement.Mandatory });
            actual.Standards.Should().Be(TaskProgress.InProgress);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetSolutionLoadingStatuses_With_No_InProgressStandards_Should_be_Status_Completed(
            Solution solution,
            List<Standard> standards,
            [Frozen] BuyingCatalogueDbContext context,
            SolutionsService service)
        {
            solution.SolutionStandards = standards.Select(x => new SolutionStandard { Standard = x, Status = StandardCompliance.FullyMet }).ToList();
            context.Solutions.Add(solution);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var actual = await service.GetSolutionLoadingStatuses(solution.CatalogueItemId, new SolutionSections { Standards = SolutionSectionRequirement.Mandatory });
            actual.Standards.Should().Be(TaskProgress.Completed);
        }

        public static IEnumerable<object[]> GetSolutionLoadingStatusesTestData()
        {
            foreach (var section in new[] { "Description", "Features", "Implementation" })
            {
                yield return new object[] { section, SolutionSectionRequirement.NotApplicable, 0, TaskProgress.NotApplicable };
                yield return new object[] { section, SolutionSectionRequirement.Optional, 0, TaskProgress.Optional };
                yield return new object[] { section, SolutionSectionRequirement.Optional, 1, TaskProgress.Completed };
                yield return new object[] { section, SolutionSectionRequirement.Mandatory, 0, TaskProgress.NotStarted };
                yield return new object[] { section, SolutionSectionRequirement.Mandatory, 1, TaskProgress.Completed };
            }

            foreach (var section in new[] { "AssociatedServices", "Interoperability", "DevelopmentPlans", "CapabilitiesAndEpics", "SupplierDetails" })
            {
                yield return new object[] { section, SolutionSectionRequirement.NotApplicable, 0, TaskProgress.NotApplicable };
                yield return new object[] { section, SolutionSectionRequirement.Optional, 0, TaskProgress.Optional };
                yield return new object[] { section, SolutionSectionRequirement.Optional, 1, TaskProgress.Completed };
                yield return new object[] { section, SolutionSectionRequirement.Mandatory, 0, TaskProgress.NotStarted };
                yield return new object[] { section, SolutionSectionRequirement.Mandatory, 1, TaskProgress.Completed };
            }

            foreach (var section in new[] { "AdditionalServices", "ListPrice" })
            {
                yield return new object[] { section, SolutionSectionRequirement.NotApplicable, 0, TaskProgress.NotApplicable };
                yield return new object[] { section, SolutionSectionRequirement.Optional, 0, TaskProgress.Optional };
                yield return new object[] { section, SolutionSectionRequirement.Optional, 1, TaskProgress.InProgress };
                yield return new object[] { section, SolutionSectionRequirement.Optional, 2, TaskProgress.Completed };
                yield return new object[] { section, SolutionSectionRequirement.Mandatory, 0, TaskProgress.NotStarted };
                yield return new object[] { section, SolutionSectionRequirement.Mandatory, 1, TaskProgress.InProgress };
                yield return new object[] { section, SolutionSectionRequirement.Mandatory, 2, TaskProgress.Completed };
            }

            foreach (var section in new[] { "ApplicationType", "HostingType" })
            {
                yield return new object[] { section, SolutionSectionRequirement.NotApplicable, 0, TaskProgress.NotApplicable };
                yield return new object[] { section, SolutionSectionRequirement.Optional, 0, TaskProgress.Optional };
                yield return new object[] { section, SolutionSectionRequirement.Optional, 1, TaskProgress.Completed };
                yield return new object[] { section, SolutionSectionRequirement.Mandatory, 0, TaskProgress.NotStarted };
                yield return new object[] { section, SolutionSectionRequirement.Mandatory, 1, TaskProgress.Completed };
            }

            foreach (var section in new[] { "DataProcessing", "ServiceLevelAgreement" })
            {
                yield return new object[] { section, SolutionSectionRequirement.NotApplicable, 0, TaskProgress.NotApplicable };
                yield return new object[] { section, SolutionSectionRequirement.Optional, 0, TaskProgress.Optional };
                yield return new object[] { section, SolutionSectionRequirement.Optional, 1, TaskProgress.InProgress };
                yield return new object[] { section, SolutionSectionRequirement.Optional, 2, TaskProgress.Completed };
                yield return new object[] { section, SolutionSectionRequirement.Mandatory, 0, TaskProgress.NotStarted };
                yield return new object[] { section, SolutionSectionRequirement.Mandatory, 1, TaskProgress.InProgress };
                yield return new object[] { section, SolutionSectionRequirement.Mandatory, 2, TaskProgress.Completed };
            }

            yield return new object[] { "Standards", SolutionSectionRequirement.NotApplicable, 0, TaskProgress.NotApplicable };
            yield return new object[] { "Standards", SolutionSectionRequirement.Optional, 0, TaskProgress.Completed };
            yield return new object[] { "Standards", SolutionSectionRequirement.Optional, 1, TaskProgress.InProgress };
            yield return new object[] { "Standards", SolutionSectionRequirement.Optional, 2, TaskProgress.Completed };
            yield return new object[] { "Standards", SolutionSectionRequirement.Mandatory, 0, TaskProgress.Completed };
            yield return new object[] { "Standards", SolutionSectionRequirement.Mandatory, 1, TaskProgress.InProgress };
            yield return new object[] { "Standards", SolutionSectionRequirement.Mandatory, 2, TaskProgress.Completed };
        }

        [Theory]
        [MockInMemoryDbMemberAutoData(nameof(GetSolutionLoadingStatusesTestData))]
        public static async Task GetSolutionLoadingStatuses_ReturnsExpectedStatusForEverySectionPermutation(
            string section,
            SolutionSectionRequirement requirement,
            int state,
            TaskProgress expected,
            [Frozen] BuyingCatalogueDbContext context,
            Solution solution,
            List<Standard> standards,
            SolutionsService service)
        {
            context.ChangeTracker.Clear();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            var sections = new SolutionSections();
            SetRequirement(sections, section, requirement);
            solution = ConfigureSolution(solution, section, state, standards);

            context.Solutions.Add(solution);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var actual = await service.GetSolutionLoadingStatuses(solution.CatalogueItemId, sections);

            GetStatus(actual, section).Should().Be(expected, $"section={section}, requirement={requirement}, state={state}");
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetSolutionStandardsForEditing_ReturnsStandardsForSolution(
            Solution solution,
            [Frozen] BuyingCatalogueDbContext context,
            SolutionsService service)
        {
            context.Solutions.Add(solution);

            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var expected = context.CatalogueItems
                .Where(ci => ci.Id == solution.CatalogueItemId)
                .SelectMany(ci => ci.CatalogueItemCapabilities)
                .Select(cic => cic.Capability)
                .SelectMany(c => c.StandardCapabilities)
                .Select(sc => sc.Standard)
                .Distinct()
                .Union(context.Standards)
                .ToList();

            var result = await service.GetSolutionStandardsForEditing(solution.CatalogueItemId);

            result.Should().HaveCount(expected.Count);
        }

        [Theory]
        [MockAutoData]
        public static async Task SaveSupplierContacts_ModelNull_ThrowsException(SolutionsService service)
        {
            var actual = await Assert.ThrowsAsync<ArgumentNullException>(() => service.SaveSupplierContacts(default));

            actual.ParamName.Should().Be("model");
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task SaveSupplierContacts_ModelValid_CallsSetSolutionIdOnModel(
            [Frozen] BuyingCatalogueDbContext context,
            Solution solution,
            SolutionsService service,
            SupplierContactsModel model)
        {
            solution.MarketingContacts.Clear();
            context.Solutions.Add(solution);
            await context.SaveChangesAsync();

            model.SolutionId = solution.CatalogueItemId;

            await service.SaveSupplierContacts(model);

            model.Contacts.Select(c => c.SolutionId).Should().AllBeEquivalentTo(model.SolutionId);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task SaveSupplierContacts_NoContactsInDatabase_AddsValidContactsToRepository(
            [Frozen] BuyingCatalogueDbContext context,
            Solution solution,
            SupplierContactsModel supplierContactsModel,
            SolutionsService service)
        {
            solution.MarketingContacts.Clear();
            context.Solutions.Add(solution);
            await context.SaveChangesAsync();

            supplierContactsModel.SolutionId = solution.CatalogueItemId;

            await service.SaveSupplierContacts(supplierContactsModel);

            var newContacts = await context.MarketingContacts
                .AsAsyncEnumerable()
                .Where(mc => mc.SolutionId == solution.CatalogueItemId)
                .ToArrayAsync();

            newContacts.Length.Should().Be(supplierContactsModel.Contacts.Length);
            newContacts.Should()
                .BeEquivalentTo(
                    supplierContactsModel.Contacts,
                    config => config
                        .Excluding(mc => mc.SolutionId)
                        .Excluding(mc => mc.LastUpdated)
                        .Excluding(mc => mc.LastUpdatedBy)
                        .Excluding(mc => mc.LastUpdatedByUser));
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task SaveSupplierContacts_ContactsInDatabase_RemovesEmptyContactsFromDatabase(
            [Frozen] BuyingCatalogueDbContext context,
            Solution solution,
            SupplierContactsModel supplierContactsModel,
            SolutionsService service)
        {
            context.Solutions.Add(solution);
            await context.SaveChangesAsync();

            supplierContactsModel.Contacts = await context.MarketingContacts
                .AsAsyncEnumerable()
                .Where(mc => mc.SolutionId == solution.CatalogueItemId)
                .ToArrayAsync();

            supplierContactsModel.Contacts[0].FirstName = null;
            supplierContactsModel.Contacts[0].LastName = null;
            supplierContactsModel.Contacts[0].Department = null;
            supplierContactsModel.Contacts[0].PhoneNumber = null;
            supplierContactsModel.Contacts[0].Email = null;

            supplierContactsModel.SolutionId = solution.CatalogueItemId;

            await service.SaveSupplierContacts(supplierContactsModel);

            var updatedContacts = await context.MarketingContacts
                .AsAsyncEnumerable()
                .Where(mc => mc.SolutionId == solution.CatalogueItemId)
                .ToArrayAsync();

            updatedContacts.Length.Should().Be(2);
            updatedContacts[0].Id.Should().Be(supplierContactsModel.Contacts[1].Id);
            updatedContacts[1].Id.Should().Be(supplierContactsModel.Contacts[2].Id);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task SaveSupplierContacts_ContactsInDatabase_UpdatesNonEmptyContacts(
            [Frozen] BuyingCatalogueDbContext context,
            Solution solution,
            SupplierContactsModel model,
            string updatedFirstName,
            string updatedLastName,
            string updatedPhoneNumber,
            string updatedEmail,
            SolutionsService service)
        {
            context.Solutions.Add(solution);
            await context.SaveChangesAsync();

            model.Contacts = await context.MarketingContacts
                .AsAsyncEnumerable()
                .Where(mc => mc.SolutionId == solution.CatalogueItemId)
                .ToArrayAsync();

            model.Contacts[0].FirstName = updatedFirstName;
            model.Contacts[0].LastName = updatedLastName;
            model.Contacts[0].PhoneNumber = updatedPhoneNumber;
            model.Contacts[0].Email = updatedEmail;
            model.SolutionId = solution.CatalogueItemId;

            await service.SaveSupplierContacts(model);

            var updatedContacts = await context.MarketingContacts
                .AsAsyncEnumerable()
                .Where(mc => mc.SolutionId == solution.CatalogueItemId)
                .ToArrayAsync();

            updatedContacts[0].FirstName.Should().Be(updatedFirstName);
            updatedContacts[0].LastName.Should().Be(updatedLastName);
            updatedContacts[0].PhoneNumber.Should().Be(updatedPhoneNumber);
            updatedContacts[0].Email.Should().Be(updatedEmail);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task SaveSupplierContacts_AddsNewAndValidContacts_ToDatabase(
            [Frozen] BuyingCatalogueDbContext context,
            CatalogueItem solution,
            SupplierContactsModel model,
            SolutionsService service)
        {
            context.CatalogueItems.Add(solution);
            await context.SaveChangesAsync();

            await service.SaveSupplierContacts(model);

            var marketingContacts = await context.MarketingContacts.AsAsyncEnumerable()
                .Where(c => c.SolutionId == model.SolutionId)
                .ToListAsync();

            marketingContacts.Should().BeEquivalentTo(model.ValidContacts());
        }

        [Theory]
        [MemberData(nameof(InvalidStringData.TestData), MemberType = typeof(InvalidStringData))]
        public static async Task SaveSolutionDescription_InvalidSummary_ThrowsException(string summary)
        {
            var service = new SolutionsService(Substitute.For<BuyingCatalogueDbContext>());

            var actual = await Assert.ThrowsAsync<ArgumentException>(
                () => service.SaveSolutionDescription(
                    new CatalogueItemId(100000, "001"),
                    summary,
                    "Description",
                    "Link"));

            actual.ParamName.Should().Be("summary");
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task SaveSolutionDetails_CallsSaveChangesAsync_OnRepository(
            [Frozen] BuyingCatalogueDbContext context,
            SolutionsService service,
            Solution solution)
        {
            const string expectedSolutionName = "Expected Name";
            const int expectedSupplierId = 247;
            const bool expectedPilotState = true;
            const SolutionCategory expectedCategory = SolutionCategory.A;

            context.Solutions.Add(solution);
            await context.SaveChangesAsync();

            await service.SaveSolutionDetails(
                solution.CatalogueItemId,
                expectedSolutionName,
                expectedSupplierId,
                expectedPilotState,
                expectedCategory,
                new List<FrameworkModel>());

            var dbSolution = await context.CatalogueItems
                .Include(s => s.Solution)
                .Include(s => s.Solution.FrameworkSolutions)
                .FirstAsync(s => s.Id == solution.CatalogueItemId);

            dbSolution.Name.Should().BeEquivalentTo(expectedSolutionName);
            dbSolution.SupplierId.Should().Be(expectedSupplierId);
            dbSolution.Solution.IsPilotSolution.Should().Be(expectedPilotState);
            dbSolution.Solution.Category.Should().Be(expectedCategory);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task SaveSolutionDescription_UpdatesDatabase(
            [Frozen] BuyingCatalogueDbContext context,
            Solution solution,
            string summary,
            string description,
            string link,
            SolutionsService service)
        {
            context.Solutions.Add(solution);
            await context.SaveChangesAsync();

            await service.SaveSolutionDescription(solution.CatalogueItemId, summary, description, link);

            var actual = await context.Solutions.AsQueryable()
                .FirstAsync(s => s.CatalogueItemId == solution.CatalogueItemId);

            actual.Summary.Should().Be(summary);
            actual.FullDescription.Should().Be(description);
            actual.AboutUrl.Should().Be(link);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task SaveSolutionFeatures_UpdatesDatabase(
            [Frozen] BuyingCatalogueDbContext context,
            Solution solution,
            string[] features,
            SolutionsService service)
        {
            context.Solutions.Add(solution);
            await context.SaveChangesAsync();

            await service.SaveSolutionFeatures(solution.CatalogueItemId, features);

            var actual = await context.Solutions.AsQueryable()
                .FirstAsync(s => s.CatalogueItemId == solution.CatalogueItemId);

            actual.Features.Should().Be(JsonSerializer.Serialize(features));
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task SaveImplementationDetail_UpdatesDatabase(
            [Frozen] BuyingCatalogueDbContext context,
            Solution solution,
            string detail,
            SolutionsService service)
        {
            context.Solutions.Add(solution);
            await context.SaveChangesAsync();

            await service.SaveImplementationDetail(solution.CatalogueItemId, detail);

            var actual = await context.Solutions.AsQueryable()
                .FirstAsync(s => s.CatalogueItemId == solution.CatalogueItemId);

            actual.ImplementationDetail.Should().Be(detail);
        }

        [Theory]
        [MockAutoData]
        public static async Task SaveApplication_InvalidModel_ThrowsException(SolutionsService service)
        {
            var actual = await Assert.ThrowsAsync<ArgumentNullException>(
                () => service.SaveApplicationType(new CatalogueItemId(100000, "001"), null));

            actual.ParamName.Should().Be("applicationTypeDetail");
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task SaveApplication_UpdatesDatabase(
            [Frozen] BuyingCatalogueDbContext context,
            Solution solution,
            ApplicationTypeDetail applicationTypeDetail,
            SolutionsService service)
        {
            context.Solutions.Add(solution);
            await context.SaveChangesAsync();

            await service.SaveApplicationType(solution.CatalogueItemId, applicationTypeDetail);
            context.ChangeTracker.Clear();

            var actual = await context.Solutions.AsQueryable()
                .FirstAsync(s => s.CatalogueItemId == solution.CatalogueItemId);

            actual.ApplicationTypeDetail.Should().BeEquivalentTo(applicationTypeDetail);
        }

        [Theory]
        [MockAutoData]
        public static async Task SaveHosting_InvalidModel_ThrowsException(SolutionsService service)
        {
            var actual = await Assert.ThrowsAsync<ArgumentNullException>(
                () => service.SaveHosting(new CatalogueItemId(100000, "001"), null));

            actual.ParamName.Should().Be("hosting");
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task SaveHosting_UpdatesDatabase(
            [Frozen] BuyingCatalogueDbContext context,
            Solution solution,
            Hosting hosting,
            SolutionsService service)
        {
            context.Solutions.Add(solution);
            await context.SaveChangesAsync();

            await service.SaveHosting(solution.CatalogueItemId, hosting);
            context.ChangeTracker.Clear();

            var actual = await context.Solutions.AsQueryable()
                .FirstAsync(s => s.CatalogueItemId == solution.CatalogueItemId);

            actual.Hosting.Should().BeEquivalentTo(hosting);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task SaveSupplier_UpdatesDatabase(
            [Frozen] BuyingCatalogueDbContext context,
            Supplier supplier,
            string description,
            string link,
            SolutionsService service)
        {
            context.Suppliers.Add(supplier);
            await context.SaveChangesAsync();

            await service.SaveSupplierDescriptionAndLink(supplier.Id, description, link);

            var actual = await context.Suppliers.AsQueryable().FirstAsync(s => s.Id == supplier.Id);

            actual.Summary.Should().Be(description);
            actual.SupplierUrl.Should().Be(link);
        }

        [Theory]
        [MockAutoData]
        public static async Task AddCatalogueSolution_NullModel_ThrowsException(SolutionsService service)
        {
            (await Assert.ThrowsAsync<ArgumentNullException>(() => service.AddCatalogueSolution(null)))
                .ParamName.Should()
                .Be("model");
        }

        [Theory]
        [MockAutoData]
        public static async Task AddCatalogueSolution_NullListOfFrameWorkModels_ThrowsException(
            SolutionsService service)
        {
            (await Assert.ThrowsAsync<ArgumentNullException>(
                    () => service.AddCatalogueSolution(new CreateSolutionModel())))
                .ParamName.Should()
                .Be(nameof(CreateSolutionModel.Frameworks));
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task AddCatalogueSolution_ModelValid_AddsCatalogueItemToDatabase(
            [Frozen] BuyingCatalogueDbContext context,
            CatalogueItem catalogueItem,
            CreateSolutionModel model,
            SolutionsService service)
        {
            model.SupplierId = catalogueItem.SupplierId;

            _ = await service.AddCatalogueSolution(model);

            var actual = await context.CatalogueItems.FirstAsync();

            actual.Name.Should().Be(model.Name);
        }

        [Theory]
        [MockInMemoryDbInlineAutoData(SolutionType.GPIT)]
        [MockInMemoryDbInlineAutoData(SolutionType.CommunityPharmacy)]
        public static async Task AddCatalogueSolution_SetsSolutionTypeFromSelectedFramework(
            SolutionType solutionType,
            [Frozen] BuyingCatalogueDbContext context,
            EntityFramework.Catalogue.Models.Framework framework,
            CreateSolutionModel model,
            SolutionsService service)
        {
            framework.SolutionType = solutionType;
            context.Frameworks.Add(framework);
            model.Frameworks = new List<FrameworkModel>
            {
                new() { FrameworkId = framework.Id, Selected = true },
            };
            await context.SaveChangesAsync();

            var catalogueItemId = await service.AddCatalogueSolution(model);
            context.ChangeTracker.Clear();

            var solution = await context.Solutions.SingleAsync(x => x.CatalogueItemId == catalogueItemId);
            solution.SolutionType.Should().Be(solutionType);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task AddCatalogueSolution_StandardsDefaultedAsMet_SetsInProgressSolutionStandards(
            CreateSolutionModel model,
            List<Standard> standards,
            [Frozen] BuyingCatalogueDbContext context,
            SolutionsService service)
        {
            standards.ForEach(x =>
            {
                x.StandardType = StandardType.Overarching;
                x.StandardCapabilities = new List<StandardCapability>();
                x.LastUpdatedByUser = null;
                x.LastUpdatedBy = null;
                x.IsMetByDefault = false;
            });

            context.Standards.RemoveRange(context.Standards);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var fullyMetStandard = standards.First();
            var mappedStandards = standards.Skip(1).ToList();

            fullyMetStandard.IsMetByDefault = true;

            context.Standards.AddRange(standards);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var catalogueItemId = await service.AddCatalogueSolution(model);

            var solution = await context.Solutions.Include(x => x.SolutionStandards)
                .FirstAsync(x => x.CatalogueItemId == catalogueItemId);

            solution.Should().NotBeNull();
            solution.SolutionStandards.Should().NotBeEmpty();
            mappedStandards.ForEach(x => solution.SolutionStandards.Should().Contain(y => y.StandardId == x.Id));
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task AddCatalogueSolution_NoStandardsDefaultedAsMet_EmptyInProgressSolutionStandards(
            CreateSolutionModel model,
            List<Standard> standards,
            [Frozen] BuyingCatalogueDbContext context,
            SolutionsService service)
        {
            standards.ForEach(x =>
            {
                x.StandardCapabilities = new List<StandardCapability>();
                x.LastUpdatedByUser = null;
                x.LastUpdatedBy = null;
                x.IsMetByDefault = false;
            });

            context.Standards.RemoveRange(context.Standards);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            context.Standards.AddRange(standards);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var catalogueItemId = await service.AddCatalogueSolution(model);

            var solution = await context.Solutions.Include(x => x.SolutionStandards)
                .FirstAsync(x => x.CatalogueItemId == catalogueItemId);

            solution.Should().NotBeNull();
            solution.SolutionStandards.Should().NotContain(x => x.Status != StandardCompliance.NotYetSelected);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task SupplierHasSolutionName_Returns_FromDatabase(
            [Frozen] BuyingCatalogueDbContext context,
            CatalogueItem catalogueItem,
            SolutionsService service)
        {
            context.CatalogueItems.Add(catalogueItem);
            await context.SaveChangesAsync();

            var actual = await service.SupplierHasSolutionName(catalogueItem.SupplierId, catalogueItem.Name);

            actual.Should().BeTrue();
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task DeleteApplicationType_UpdatesDatabase(
            Solution catalogueSolution,
            ApplicationTypeDetail applicationTypeDetail,
            [Frozen] BuyingCatalogueDbContext context,
            SolutionsService service)
        {
            applicationTypeDetail.ApplicationTypes =
                new HashSet<string> { "browser-based", "native-mobile", "native-desktop" };
            catalogueSolution.ApplicationTypeDetail = applicationTypeDetail;
            context.Solutions.Add(catalogueSolution);
            await context.SaveChangesAsync();

            await service.DeleteApplicationType(
                catalogueSolution.CatalogueItemId,
                ApplicationType.BrowserBased);
            context.ChangeTracker.Clear();

            var actual = await context.Solutions.AsQueryable()
                .FirstAsync(s => s.CatalogueItemId == catalogueSolution.CatalogueItemId);

            var actualApplicationTypeDetail = actual.ApplicationTypeDetail;

            actualApplicationTypeDetail.ApplicationTypes.Any(c => c.Equals("browser-based")).Should().BeFalse();
            actualApplicationTypeDetail.ApplicationTypes.Any(c => c.Equals("native-mobile")).Should().BeTrue();
            actualApplicationTypeDetail.ApplicationTypes.Any(c => c.Equals("native-desktop")).Should().BeTrue();
        }

        [Theory]
        [MockAutoData]
        public static void Remove_BrowserBased_ApplicationType_RemovesBrowserBasedEntries(
            ApplicationTypeDetail applicationTypeDetail)
        {
            applicationTypeDetail.ApplicationTypes = new HashSet<string>
            {
                ApplicationType.BrowserBased.AsString(EnumFormat.EnumMemberValue),
                ApplicationType.Desktop.AsString(EnumFormat.EnumMemberValue),
                ApplicationType.MobileTablet.AsString(EnumFormat.EnumMemberValue),
            };

            var updatedApplicationType = SolutionsService.RemoveApplicationType(
                applicationTypeDetail,
                ApplicationType.BrowserBased);

            updatedApplicationType.ApplicationTypes.Should()
                .BeEquivalentTo(
                    new HashSet<string>
                    {
                        ApplicationType.Desktop.AsString(EnumFormat.EnumMemberValue),
                        ApplicationType.MobileTablet.AsString(EnumFormat.EnumMemberValue),
                    });

            // Browser Based
            updatedApplicationType.AdditionalInformation.Should().BeNull();
            updatedApplicationType.BrowsersSupported.Should().BeNull();
            updatedApplicationType.HardwareRequirements.Should().BeNull();
            updatedApplicationType.MinimumConnectionSpeed.Should().BeNull();
            updatedApplicationType.MinimumDesktopResolution.Should().BeNull();
            updatedApplicationType.MobileFirstDesign.Should().BeNull();
            updatedApplicationType.MobileResponsive.Should().BeNull();
            updatedApplicationType.Plugins.Should().BeNull();

            // Desktop
            updatedApplicationType.NativeDesktopAdditionalInformation.Should()
                .Be(applicationTypeDetail.NativeDesktopAdditionalInformation);
            updatedApplicationType.NativeDesktopHardwareRequirements.Should()
                .Be(applicationTypeDetail.NativeDesktopHardwareRequirements);
            updatedApplicationType.NativeDesktopMemoryAndStorage.Should()
                .BeEquivalentTo(applicationTypeDetail.NativeDesktopMemoryAndStorage);
            updatedApplicationType.NativeDesktopMinimumConnectionSpeed.Should()
                .Be(applicationTypeDetail.NativeDesktopMinimumConnectionSpeed);
            updatedApplicationType.NativeDesktopOperatingSystemsDescription.Should()
                .Be(applicationTypeDetail.NativeDesktopOperatingSystemsDescription);
            updatedApplicationType.NativeDesktopThirdParty.Should()
                .BeEquivalentTo(applicationTypeDetail.NativeDesktopThirdParty);

            // Mobile or Tablet
            updatedApplicationType.MobileConnectionDetails.Should()
                .BeEquivalentTo(applicationTypeDetail.MobileConnectionDetails);
            updatedApplicationType.MobileMemoryAndStorage.Should()
                .BeEquivalentTo(applicationTypeDetail.MobileMemoryAndStorage);
            updatedApplicationType.MobileOperatingSystems.Should()
                .BeEquivalentTo(applicationTypeDetail.MobileOperatingSystems);
            updatedApplicationType.MobileThirdParty.Should().BeEquivalentTo(applicationTypeDetail.MobileThirdParty);
            updatedApplicationType.NativeMobileAdditionalInformation.Should()
                .Be(applicationTypeDetail.NativeMobileAdditionalInformation);
            updatedApplicationType.NativeMobileFirstDesign.Should().Be(applicationTypeDetail.NativeMobileFirstDesign);
            updatedApplicationType.NativeMobileHardwareRequirements.Should()
                .Be(applicationTypeDetail.NativeMobileHardwareRequirements);
        }

        [Theory]
        [MockAutoData]
        public static void Remove_Desktop_ApplicationType_RemovesDesktopEntries(ApplicationTypeDetail applicationTypeDetail)
        {
            applicationTypeDetail.ApplicationTypes = new HashSet<string>
            {
                ApplicationType.BrowserBased.AsString(EnumFormat.EnumMemberValue),
                ApplicationType.Desktop.AsString(EnumFormat.EnumMemberValue),
                ApplicationType.MobileTablet.AsString(EnumFormat.EnumMemberValue),
            };

            var updatedApplicationType = SolutionsService.RemoveApplicationType(
                applicationTypeDetail,
                ApplicationType.Desktop);

            updatedApplicationType.ApplicationTypes.Should()
                .BeEquivalentTo(
                    new HashSet<string>
                    {
                        ApplicationType.BrowserBased.AsString(EnumFormat.EnumMemberValue),
                        ApplicationType.MobileTablet.AsString(EnumFormat.EnumMemberValue),
                    });

            // Browser Based
            updatedApplicationType.AdditionalInformation.Should().Be(applicationTypeDetail.AdditionalInformation);
            updatedApplicationType.BrowsersSupported.Should().BeEquivalentTo(applicationTypeDetail.BrowsersSupported);
            updatedApplicationType.HardwareRequirements.Should().Be(applicationTypeDetail.HardwareRequirements);
            updatedApplicationType.MinimumConnectionSpeed.Should().Be(applicationTypeDetail.MinimumConnectionSpeed);
            updatedApplicationType.MinimumDesktopResolution.Should().Be(applicationTypeDetail.MinimumDesktopResolution);
            updatedApplicationType.MobileFirstDesign.Should().Be(applicationTypeDetail.MobileFirstDesign);
            updatedApplicationType.MobileResponsive.Should().Be(applicationTypeDetail.MobileResponsive);
            updatedApplicationType.Plugins.Should().BeEquivalentTo(applicationTypeDetail.Plugins);

            // Desktop
            updatedApplicationType.NativeDesktopAdditionalInformation.Should().BeNull();
            updatedApplicationType.NativeDesktopHardwareRequirements.Should().BeNull();
            updatedApplicationType.NativeDesktopMemoryAndStorage.Should().BeNull();
            updatedApplicationType.NativeDesktopMinimumConnectionSpeed.Should().BeNull();
            updatedApplicationType.NativeDesktopOperatingSystemsDescription.Should().BeNull();
            updatedApplicationType.NativeDesktopThirdParty.Should().BeNull();

            // Mobile or Tablet
            updatedApplicationType.MobileConnectionDetails.Should()
                .BeEquivalentTo(applicationTypeDetail.MobileConnectionDetails);
            updatedApplicationType.MobileMemoryAndStorage.Should()
                .BeEquivalentTo(applicationTypeDetail.MobileMemoryAndStorage);
            updatedApplicationType.MobileOperatingSystems.Should()
                .BeEquivalentTo(applicationTypeDetail.MobileOperatingSystems);
            updatedApplicationType.MobileThirdParty.Should().BeEquivalentTo(applicationTypeDetail.MobileThirdParty);
            updatedApplicationType.NativeMobileAdditionalInformation.Should()
                .Be(applicationTypeDetail.NativeMobileAdditionalInformation);
            updatedApplicationType.NativeMobileFirstDesign.Should().Be(applicationTypeDetail.NativeMobileFirstDesign);
            updatedApplicationType.NativeMobileHardwareRequirements.Should()
                .Be(applicationTypeDetail.NativeMobileHardwareRequirements);
        }

        [Theory]
        [MockAutoData]
        public static void Remove_Mobile_ApplicationType_RemovesMobileEntries(ApplicationTypeDetail applicationTypeDetail)
        {
            applicationTypeDetail.ApplicationTypes = new HashSet<string>
            {
                ApplicationType.BrowserBased.AsString(EnumFormat.EnumMemberValue),
                ApplicationType.Desktop.AsString(EnumFormat.EnumMemberValue),
                ApplicationType.MobileTablet.AsString(EnumFormat.EnumMemberValue),
            };

            var updatedApplicationType = SolutionsService.RemoveApplicationType(
                applicationTypeDetail,
                ApplicationType.MobileTablet);

            updatedApplicationType.ApplicationTypes.Should()
                .BeEquivalentTo(
                    new HashSet<string>
                    {
                        ApplicationType.BrowserBased.AsString(EnumFormat.EnumMemberValue),
                        ApplicationType.Desktop.AsString(EnumFormat.EnumMemberValue),
                    });

            // Browser Based
            updatedApplicationType.AdditionalInformation.Should().Be(applicationTypeDetail.AdditionalInformation);
            updatedApplicationType.BrowsersSupported.Should().BeEquivalentTo(applicationTypeDetail.BrowsersSupported);
            updatedApplicationType.HardwareRequirements.Should().Be(applicationTypeDetail.HardwareRequirements);
            updatedApplicationType.MinimumConnectionSpeed.Should().Be(applicationTypeDetail.MinimumConnectionSpeed);
            updatedApplicationType.MinimumDesktopResolution.Should().Be(applicationTypeDetail.MinimumDesktopResolution);
            updatedApplicationType.MobileFirstDesign.Should().Be(applicationTypeDetail.MobileFirstDesign);
            updatedApplicationType.MobileResponsive.Should().Be(applicationTypeDetail.MobileResponsive);
            updatedApplicationType.Plugins.Should().BeEquivalentTo(applicationTypeDetail.Plugins);

            // Desktop
            updatedApplicationType.NativeDesktopAdditionalInformation.Should()
                .Be(applicationTypeDetail.NativeDesktopAdditionalInformation);
            updatedApplicationType.NativeDesktopHardwareRequirements.Should()
                .Be(applicationTypeDetail.NativeDesktopHardwareRequirements);
            updatedApplicationType.NativeDesktopMemoryAndStorage.Should()
                .BeEquivalentTo(applicationTypeDetail.NativeDesktopMemoryAndStorage);
            updatedApplicationType.NativeDesktopMinimumConnectionSpeed.Should()
                .Be(applicationTypeDetail.NativeDesktopMinimumConnectionSpeed);
            updatedApplicationType.NativeDesktopOperatingSystemsDescription.Should()
                .Be(applicationTypeDetail.NativeDesktopOperatingSystemsDescription);
            updatedApplicationType.NativeDesktopThirdParty.Should()
                .BeEquivalentTo(applicationTypeDetail.NativeDesktopThirdParty);

            // Mobile or Tablet
            updatedApplicationType.MobileConnectionDetails.Should().BeNull();
            updatedApplicationType.MobileMemoryAndStorage.Should().BeNull();
            updatedApplicationType.MobileOperatingSystems.Should().BeNull();
            updatedApplicationType.MobileThirdParty.Should().BeNull();
            updatedApplicationType.NativeMobileAdditionalInformation.Should().BeNull();
            updatedApplicationType.NativeMobileFirstDesign.Should().BeNull();
            updatedApplicationType.NativeMobileHardwareRequirements.Should().BeNull();
        }

        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        [MockInlineAutoData(" ")]
        public static void GetAllSolutionsForSearchTerm_NullSearchTerm_ThrowsException(
            string searchTerm,
            SolutionsService systemUnderTest)
        {
            FluentActions
                .Awaiting(() => systemUnderTest.GetAllSolutionsForSearchTerm(searchTerm))
                .Should()
                .ThrowAsync<ArgumentNullException>();
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetAllSolutionsForSearchTerm_ReturnsExpectedResults(
            string searchTerm,
            [Frozen] BuyingCatalogueDbContext context,
            SolutionsService systemUnderTest)
        {
            var solutions = GetSolutionsForSearchTerm(searchTerm);
            var noMatch = solutions.First(x => x.Name == SolutionName && x.Supplier.Name == SupplierName);

            for (var i = 0; i < solutions.Count; i++)
            {
                var supplier = solutions[i].Supplier;

                supplier.Id = (i + 1) * 10;
                supplier.LegalName = $"{i}";

                context.Suppliers.Add(supplier);

                solutions[i].CatalogueItemType = CatalogueItemType.Solution;
                solutions[i].Id = new CatalogueItemId(supplier.Id, $"{i}");
                solutions[i].SupplierId = supplier.Id;
            }

            context.CatalogueItems.AddRange(solutions);

            await context.SaveChangesAsync();

            var results = await systemUnderTest.GetAllSolutionsForSearchTerm(searchTerm);

            results.Should().BeEquivalentTo(solutions.Except(new[] { noMatch }));
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetSupplierSolutions_ReturnsExpectedResults(
            Supplier supplier,
            List<Solution> solutions,
            [Frozen] BuyingCatalogueDbContext context,
            SolutionsService service)
        {
            solutions.ForEach(s => s.FrameworkSolutions.ForEach(f => f.Framework.IsExpired = false));
            var expectedSolutions = solutions
                .Select(x => x.CatalogueItem)
                .ToList();

            solutions.ForEach(
                x =>
                {
                    x.CatalogueItem.Supplier = supplier;
                    x.CatalogueItem.PublishedStatus = PublicationStatus.Published;
                });

            context.Suppliers.RemoveRange(context.Suppliers);
            context.Solutions.AddRange(solutions);
            context.Suppliers.Add(supplier);

            await context.SaveChangesAsync();

            var result = await service.GetSupplierSolutions(supplier.Id, null);

            result.Should().BeEquivalentTo(expectedSolutions);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetSupplierSolutions_With_Framework_ReturnsNoResults(
            Supplier supplier,
            string nonMatchingFrameworkId,
            List<Solution> solutions,
            [Frozen] BuyingCatalogueDbContext context,
            SolutionsService service)
        {
            solutions.ForEach(
                x =>
                {
                    x.FrameworkSolutions.ForEach(f => f.Framework.IsExpired = false);
                    x.CatalogueItem.Supplier = supplier;
                    x.CatalogueItem.PublishedStatus = PublicationStatus.Published;
                });

            context.Solutions.AddRange(solutions);
            context.Suppliers.Add(supplier);

            await context.SaveChangesAsync();

            var result = await service.GetSupplierSolutions(supplier.Id, nonMatchingFrameworkId);

            result.Should().BeEmpty();
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetSupplierSolutions_With_Framework_ReturnsExpectedResults(
            Supplier supplier,
            List<Solution> solutions,
            [Frozen] BuyingCatalogueDbContext context,
            SolutionsService service)
        {
            solutions.ForEach(s => s.FrameworkSolutions.ForEach(f => f.Framework.IsExpired = false));
            var matchingFrameworkId = solutions.First().FrameworkSolutions.First().Framework.Id;
            var expectedSolutions = solutions
                .Take(1)
                .Select(x => x.CatalogueItem)
                .ToList();

            solutions.ForEach(
                x =>
                {
                    x.CatalogueItem.Supplier = supplier;
                    x.CatalogueItem.PublishedStatus = PublicationStatus.Published;
                });

            context.Solutions.AddRange(solutions);
            context.Suppliers.Add(supplier);

            await context.SaveChangesAsync();

            var result = await service.GetSupplierSolutions(supplier.Id, matchingFrameworkId);

            result.Should().BeEquivalentTo(expectedSolutions);
        }

        [Theory]
        [MockInMemoryDbInlineAutoData(PracticeReorganisationTypeEnum.Split)]
        [MockInMemoryDbInlineAutoData(PracticeReorganisationTypeEnum.Merger)]
        [MockInMemoryDbInlineAutoData(PracticeReorganisationTypeEnum.None)]
        public static async Task GetSupplierSolutionsWithAssociatedServices_ReturnsMatchingResults(
            PracticeReorganisationTypeEnum practiceReorganisationType,
            Supplier supplier,
            AssociatedService associatedService,
            List<Solution> solutions,
            [Frozen] BuyingCatalogueDbContext context,
            SolutionsService service)
        {
            supplier.CatalogueItems.Clear();
            associatedService.CatalogueItem.Supplier = supplier;
            associatedService.CatalogueItem.PublishedStatus = PublicationStatus.Published;
            associatedService.PracticeReorganisationType = practiceReorganisationType;

            solutions.ForEach(
                x =>
                {
                    x.FrameworkSolutions.ForEach(f => f.Framework.IsExpired = false);
                    x.CatalogueItem.Supplier = supplier;
                    x.CatalogueItem.PublishedStatus = PublicationStatus.Published;
                    x.CatalogueItem.SupplierServiceAssociations = new List<SupplierServiceAssociation>
                    {
                        new(x.CatalogueItemId, associatedService.CatalogueItemId),
                    };
                });

            var expectedSolutions = solutions
                .Select(x => x.CatalogueItem)
                .ToList();

            context.AssociatedServices.Add(associatedService);
            context.Solutions.AddRange(solutions);
            context.Suppliers.Add(supplier);

            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var result = await service.GetSupplierSolutionsWithAssociatedServices(supplier.Id, practiceReorganisationType, null);

            result.Should().NotBeEmpty();
            result.Should().HaveCount(expectedSolutions.Count);
        }

        [Theory]
        [MockInMemoryDbInlineAutoData(PracticeReorganisationTypeEnum.Split)]
        [MockInMemoryDbInlineAutoData(PracticeReorganisationTypeEnum.Merger)]
        [MockInMemoryDbInlineAutoData(PracticeReorganisationTypeEnum.None)]
        public static async Task GetSupplierSolutionsWithAssociatedServices_With_NonMatchingFramework_ReturnsNoResults(
            PracticeReorganisationTypeEnum practiceReorganisationType,
            string nonMatchingFramework,
            Supplier supplier,
            AssociatedService associatedService,
            List<Solution> solutions,
            [Frozen] BuyingCatalogueDbContext context,
            SolutionsService service)
        {
            supplier.CatalogueItems.Clear();
            associatedService.CatalogueItem.Supplier = supplier;
            associatedService.CatalogueItem.PublishedStatus = PublicationStatus.Published;
            associatedService.PracticeReorganisationType = practiceReorganisationType;

            solutions.ForEach(
                x =>
                {
                    x.FrameworkSolutions.ForEach(f => f.Framework.IsExpired = false);
                    x.CatalogueItem.Supplier = supplier;
                    x.CatalogueItem.PublishedStatus = PublicationStatus.Published;
                    x.CatalogueItem.SupplierServiceAssociations = new List<SupplierServiceAssociation>
                    {
                        new(x.CatalogueItemId, associatedService.CatalogueItemId),
                    };
                });

            context.AssociatedServices.Add(associatedService);
            context.Solutions.AddRange(solutions);
            context.Suppliers.Add(supplier);

            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var result = await service.GetSupplierSolutionsWithAssociatedServices(supplier.Id, practiceReorganisationType, nonMatchingFramework);

            result.Should().BeEmpty();
        }

        [Theory]
        [MockInMemoryDbInlineAutoData(PracticeReorganisationTypeEnum.Split)]
        [MockInMemoryDbInlineAutoData(PracticeReorganisationTypeEnum.Merger)]
        [MockInMemoryDbInlineAutoData(PracticeReorganisationTypeEnum.None)]
        public static async Task GetSupplierSolutionsWithAssociatedServices_With_MatchingFramework_ReturnsMatchingResults(
            PracticeReorganisationTypeEnum practiceReorganisationType,
            Supplier supplier,
            AssociatedService associatedService,
            List<Solution> solutions,
            [Frozen] BuyingCatalogueDbContext context,
            SolutionsService service)
        {
            supplier.CatalogueItems.Clear();
            associatedService.CatalogueItem.Supplier = supplier;
            associatedService.CatalogueItem.PublishedStatus = PublicationStatus.Published;
            associatedService.PracticeReorganisationType = practiceReorganisationType;

            solutions.ForEach(
                x =>
                {
                    x.FrameworkSolutions.ForEach(f => f.Framework.IsExpired = false);
                    x.CatalogueItem.Supplier = supplier;
                    x.CatalogueItem.PublishedStatus = PublicationStatus.Published;
                    x.CatalogueItem.SupplierServiceAssociations = new List<SupplierServiceAssociation>
                    {
                        new(x.CatalogueItemId, associatedService.CatalogueItemId),
                    };
                });

            var matchingFrameworkId = solutions.First().FrameworkSolutions.First().Framework.Id;

            var expectedSolutions = solutions
                .Take(1)
                .Select(x => x.CatalogueItem)
                .ToList();

            context.AssociatedServices.Add(associatedService);
            context.Solutions.AddRange(solutions);
            context.Suppliers.Add(supplier);

            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var result = await service.GetSupplierSolutionsWithAssociatedServices(supplier.Id, practiceReorganisationType, matchingFrameworkId);

            result.Should().NotBeEmpty();
            result.Should().HaveCount(expectedSolutions.Count);
        }

        [Theory]
        [MockInMemoryDbInlineAutoData(PracticeReorganisationTypeEnum.Split)]
        [MockInMemoryDbInlineAutoData(PracticeReorganisationTypeEnum.Merger)]
        public static async Task GetSupplierSolutionsWithAssociatedServices_WithMergerOrSplit_OnlyReturnsMergersOrSplits(
            PracticeReorganisationTypeEnum practiceReorganisationType,
            Supplier supplier,
            AssociatedService associatedService,
            List<Solution> solutions,
            [Frozen] BuyingCatalogueDbContext context,
            SolutionsService service)
        {
            supplier.CatalogueItems.Clear();
            associatedService.CatalogueItem.Supplier = supplier;
            associatedService.CatalogueItem.PublishedStatus = PublicationStatus.Published;
            associatedService.PracticeReorganisationType = PracticeReorganisationTypeEnum.None;

            solutions.ForEach(
                x =>
                {
                    x.CatalogueItem.Supplier = supplier;
                    x.CatalogueItem.PublishedStatus = PublicationStatus.Published;
                    x.CatalogueItem.SupplierServiceAssociations = new List<SupplierServiceAssociation>
                    {
                        new(x.CatalogueItemId, associatedService.CatalogueItemId),
                    };
                });

            context.AssociatedServices.Add(associatedService);
            context.Solutions.AddRange(solutions);
            context.Suppliers.Add(supplier);

            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var result = await service.GetSupplierSolutionsWithAssociatedServices(supplier.Id, practiceReorganisationType, null);

            result.Should().BeEmpty();
        }

        private static List<CatalogueItem> GetSolutionsForSearchTerm(string searchTerm)
        {
            return new List<CatalogueItem>
            {
                new() { Name = SolutionName, Supplier = new Supplier { Name = SupplierName } },
                new() { Name = $"{searchTerm}", Supplier = new Supplier { Name = SupplierName } },
                new() { Name = $"{searchTerm}{Junk}", Supplier = new Supplier { Name = SupplierName } },
                new() { Name = $"{Junk}{searchTerm}", Supplier = new Supplier { Name = SupplierName } },
                new() { Name = $"{Junk}{searchTerm}{Junk}", Supplier = new Supplier { Name = SupplierName } },
                new() { Name = SolutionName, Supplier = new Supplier { Name = $"{searchTerm}" } },
                new() { Name = SolutionName, Supplier = new Supplier { Name = $"{searchTerm}{Junk}" } },
                new() { Name = SolutionName, Supplier = new Supplier { Name = $"{Junk}{searchTerm}" } },
                new() { Name = SolutionName, Supplier = new Supplier { Name = $"{Junk}{searchTerm}{Junk}" } },
            };
        }

        private static void SetRequirement(SolutionSections sections, string section, SolutionSectionRequirement requirement)
        {
            var property = section == "DataProcessing"
                ? nameof(SolutionSections.DataProcessingInformation)
                : section == "Interoperability"
                    ? nameof(SolutionSections.Integrations)
                : section;

            typeof(SolutionSections).GetProperty(property).SetValue(sections, requirement);
        }

        private static TaskProgress GetStatus(SolutionLoadingStatusesModel statuses, string section)
        {
            var property = section == "Interoperability"
                ? nameof(SolutionLoadingStatusesModel.Interoperability)
                : section;

            return (TaskProgress)typeof(SolutionLoadingStatusesModel).GetProperty(property).GetValue(statuses);
        }

        private static Solution ConfigureSolution(Solution source, string section, int state, List<Standard> standards)
        {
            var solution = new Solution
            {
                CatalogueItemId = source.CatalogueItemId,
                ServiceLevelAgreement = new NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models.ServiceLevelAgreements
                {
                    SolutionId = source.CatalogueItemId,
                    Contacts = new HashSet<SlaContact>(),
                    ServiceHours = new HashSet<ServiceAvailabilityTimes>(),
                    ServiceLevels = new HashSet<SlaServiceLevel>(),
                },
            };

            solution.ServiceLevelAgreement.Solution = solution;

            solution.CatalogueItem = new CatalogueItem
            {
                Id = solution.CatalogueItemId,
                Name = "Solution",
                CatalogueItemType = CatalogueItemType.Solution,
                Solution = solution,
            };

            solution.Summary = state == 1 ? "Summary" : null;
            solution.Features = state == 1 ? "Features" : null;
            solution.ImplementationDetail = state == 1 ? "Implementation" : null;
            solution.ApplicationTypeDetail = section == "ApplicationType" && state == 1 ? new ApplicationTypeDetail() : null;
            solution.Hosting = section == "HostingType" && state == 1 ? new Hosting { PublicCloud = new PublicCloud { Summary = "Hosting" } } : null;

            solution.AdditionalServices = [];
            solution.CatalogueItem.SupplierServiceAssociations = [];
            solution.WorkOffPlans = [];
            solution.CatalogueItem.CatalogueItemCapabilities = [];
            solution.CatalogueItem.CatalogueItemContacts = [];
            solution.CatalogueItem.CataloguePrices = [];
            solution.SolutionStandards = [];
            solution.Integrations = [];

            if (state > 0)
            {
                switch (section)
                {
                    case "AssociatedServices":
                        solution.CatalogueItem.SupplierServiceAssociations.Add(new SupplierServiceAssociation
                        {
                            CatalogueItemId = solution.CatalogueItemId,
                            AssociatedServiceId = new CatalogueItemId(999997, "997"),
                        }); break;
                    case "Interoperability": solution.Integrations.Add(new SolutionIntegration { CatalogueItemId = solution.CatalogueItemId }); break;
                    case "DevelopmentPlans": solution.WorkOffPlans.Add(new WorkOffPlan { SolutionId = solution.CatalogueItemId, StandardId = standards[0].Id, Standard = standards[0], Details = "Details" }); break;
                    case "CapabilitiesAndEpics":
                        solution.CatalogueItem.CatalogueItemCapabilities.Add(new CatalogueItemCapability
                        {
                            CatalogueItemId = solution.CatalogueItemId,
                            CapabilityId = 1,
                        }); break;
                    case "SupplierDetails": solution.CatalogueItem.CatalogueItemContacts.Add(new SupplierContact()); break;
                    case "AdditionalServices":
                        solution.AdditionalServices.Add(new AdditionalService
                        {
                            SolutionId = solution.CatalogueItemId,
                            CatalogueItemId = new CatalogueItemId(999998, "998"),
                            CatalogueItem = new CatalogueItem
                            {
                                Id = new CatalogueItemId(999998, "998"),
                                Name = "Additional service",
                                CatalogueItemType = CatalogueItemType.AdditionalService,
                                PublishedStatus = state == 2 ? PublicationStatus.Published : PublicationStatus.Unpublished,
                            },
                        }); break;
                    case "ListPrice":
                        solution.CatalogueItem.CataloguePrices.Add(new CataloguePrice
                        {
                            CatalogueItemId = solution.CatalogueItemId,
                            CurrencyCode = "GBP",
                            PublishedStatus = state == 2 ? PublicationStatus.Published : PublicationStatus.Unpublished,
                        }); break;
                    case "Standards": solution.SolutionStandards.Add(new SolutionStandard { Standard = standards[0], Status = state == 1 ? StandardCompliance.InProgress : StandardCompliance.FullyMet }); break;
                    case "DataProcessing":
                        solution.DataProcessingInformation = new DataProcessingInformation
                        {
                            SolutionId = solution.CatalogueItemId,
                            DataProcessingDetailsId = 1,
                            Details = new DataProcessingDetails
                            {
                                Subject = "Subject",
                                Duration = "Duration",
                                ProcessingNature = "Processing nature",
                                PersonalDataTypes = "Personal data types",
                                DataSubjectCategories = "Data subject categories",
                            },
                            Location = state == 2 ? new DataProcessingLocation { DataProcessingInfoId = 1, ProcessingLocation = "Location" } : null,
                            Officer = state == 2 ? new DataProtectionOfficer { DataProcessingInfoId = 1, Name = "Name", EmailAddress = "email@example.com", PhoneNumber = "0123456789" } : null,
                        }; break;
                }
            }

            if (section == "AdditionalServices" && state > 0)
            {
                var additionalService = solution.AdditionalServices.Single();
                additionalService.Solution = solution;
                additionalService.CatalogueItem.AdditionalService = additionalService;
            }

            if (section == "ServiceLevelAgreement" && state > 0)
            {
                solution.ServiceLevelAgreement.Contacts.Add(new SlaContact
                {
                    SolutionId = solution.CatalogueItemId,
                    ServiceLevelAgreement = solution.ServiceLevelAgreement,
                });
            }

            if (section == "ServiceLevelAgreement" && state == 2)
            {
                solution.ServiceLevelAgreement.ServiceHours.Add(new ServiceAvailabilityTimes
                {
                    SolutionId = solution.CatalogueItemId,
                    ServiceLevelAgreement = solution.ServiceLevelAgreement,
                });
                solution.ServiceLevelAgreement.ServiceLevels.Add(new SlaServiceLevel
                {
                    SolutionId = solution.CatalogueItemId,
                    ServiceLevelAgreement = solution.ServiceLevelAgreement,
                });
            }

            return solution;
        }
    }
}
