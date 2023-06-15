using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using EnumsNET;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Serialization;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.Services.Solutions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.TestData;
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
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(SolutionsService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static async Task SaveSupplierContacts_ModelNull_ThrowsException(SolutionsService service)
        {
            var actual = await Assert.ThrowsAsync<ArgumentNullException>(() => service.SaveSupplierContacts(default));

            actual.ParamName.Should().Be("model");
        }

        [Theory]
        [InMemoryDbAutoData]
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
        [InMemoryDbAutoData]
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
        [InMemoryDbAutoData]
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
        [InMemoryDbAutoData]
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
        [InMemoryDbAutoData]
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
            var service = new SolutionsService(Mock.Of<BuyingCatalogueDbContext>());

            var actual = await Assert.ThrowsAsync<ArgumentException>(
                () => service.SaveSolutionDescription(
                    new CatalogueItemId(100000, "001"),
                    summary,
                    "Description",
                    "Link"));

            actual.ParamName.Should().Be("summary");
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task SaveSolutionDetails_CallsSaveChangesAsync_OnRepository(
            [Frozen] BuyingCatalogueDbContext context,
            SolutionsService service,
            Solution solution)
        {
            const string expectedSolutionName = "Expected Name";
            const int expectedSupplierId = 247;
            const bool expectedPilotState = true;

            context.Solutions.Add(solution);
            await context.SaveChangesAsync();

            await service.SaveSolutionDetails(
                solution.CatalogueItemId,
                expectedSolutionName,
                expectedSupplierId,
                expectedPilotState,
                new List<FrameworkModel>());

            var dbSolution = await context.CatalogueItems
                .Include(s => s.Solution)
                .Include(s => s.Solution.FrameworkSolutions)
                .FirstAsync(s => s.Id == solution.CatalogueItemId);

            dbSolution.Name.Should().BeEquivalentTo(expectedSolutionName);
            dbSolution.SupplierId.Should().Be(expectedSupplierId);
            dbSolution.Solution.IsPilotSolution.Should().Be(expectedPilotState);
        }

        [Theory]
        [InMemoryDbAutoData]
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
        [InMemoryDbAutoData]
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
        [InMemoryDbAutoData]
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
        [CommonAutoData]
        public static async Task SaveClientApplication_InvalidModel_ThrowsException(SolutionsService service)
        {
            var actual = await Assert.ThrowsAsync<ArgumentNullException>(
                () => service.SaveClientApplication(new CatalogueItemId(100000, "001"), null));

            actual.ParamName.Should().Be("clientApplication");
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task SaveClientApplication_UpdatesDatabase(
            [Frozen] BuyingCatalogueDbContext context,
            Solution solution,
            ClientApplication clientApplication,
            SolutionsService service)
        {
            context.Solutions.Add(solution);
            await context.SaveChangesAsync();

            await service.SaveClientApplication(solution.CatalogueItemId, clientApplication);

            var actual = await context.Solutions.AsQueryable()
                .FirstAsync(s => s.CatalogueItemId == solution.CatalogueItemId);

            actual.ApplicationType.Should().Be(JsonSerializer.Serialize(clientApplication));
        }

        [Theory]
        [CommonAutoData]
        public static async Task SaveHosting_InvalidModel_ThrowsException(SolutionsService service)
        {
            var actual = await Assert.ThrowsAsync<ArgumentNullException>(
                () => service.SaveHosting(new CatalogueItemId(100000, "001"), null));

            actual.ParamName.Should().Be("hosting");
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task SaveHosting_UpdatesDatabase(
            [Frozen] BuyingCatalogueDbContext context,
            Solution solution,
            Hosting hosting,
            SolutionsService service)
        {
            context.Solutions.Add(solution);
            await context.SaveChangesAsync();

            await service.SaveHosting(solution.CatalogueItemId, hosting);

            var actual = await context.Solutions.AsQueryable()
                .FirstAsync(s => s.CatalogueItemId == solution.CatalogueItemId);

            actual.Hosting.Should().BeEquivalentTo(hosting);
        }

        [Theory]
        [InMemoryDbAutoData]
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
        [CommonAutoData]
        public static async Task AddCatalogueSolution_NullModel_ThrowsException(SolutionsService service)
        {
            (await Assert.ThrowsAsync<ArgumentNullException>(() => service.AddCatalogueSolution(null)))
                .ParamName.Should()
                .Be("model");
        }

        [Theory]
        [CommonAutoData]
        public static async Task AddCatalogueSolution_NullListOfFrameWorkModels_ThrowsException(
            SolutionsService service)
        {
            (await Assert.ThrowsAsync<ArgumentNullException>(
                    () => service.AddCatalogueSolution(new CreateSolutionModel())))
                .ParamName.Should()
                .Be(nameof(CreateSolutionModel.Frameworks));
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddCatalogueSolution_ModelValid_AddsCatalogueItemToDatabase(
            [Frozen] BuyingCatalogueDbContext context,
            CatalogueItem catalogueItem,
            CreateSolutionModel model,
            SolutionsService service)
        {
            model.SupplierId = catalogueItem.SupplierId;

            _ = await service.AddCatalogueSolution(model);

            var actual = await context.CatalogueItems.AsAsyncEnumerable().FirstAsync();

            actual.Name.Should().Be(model.Name);
        }

        [Theory]
        [InMemoryDbAutoData]
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
        [InMemoryDbAutoData]
        public static async Task DeleteClientApplication_UpdatesDatabase(
            Solution catalogueSolution,
            ClientApplication clientApplication,
            [Frozen] BuyingCatalogueDbContext context,
            SolutionsService service)
        {
            clientApplication.ClientApplicationTypes =
                new HashSet<string> { "browser-based", "native-mobile", "native-desktop" };
            catalogueSolution.ApplicationType = JsonSerializer.Serialize(clientApplication);
            context.Solutions.Add(catalogueSolution);
            await context.SaveChangesAsync();

            await service.DeleteClientApplication(
                catalogueSolution.CatalogueItemId,
                ApplicationType.BrowserBased);

            var actual = await context.Solutions.AsQueryable()
                .FirstAsync(s => s.CatalogueItemId == catalogueSolution.CatalogueItemId);

            var actualClientApplication = JsonDeserializer.Deserialize<ClientApplication>(actual.ApplicationType);

            actualClientApplication.ClientApplicationTypes.Any(c => c.Equals("browser-based")).Should().BeFalse();
            actualClientApplication.ClientApplicationTypes.Any(c => c.Equals("native-mobile")).Should().BeTrue();
            actualClientApplication.ClientApplicationTypes.Any(c => c.Equals("native-desktop")).Should().BeTrue();
        }

        [Theory]
        [CommonAutoData]
        public static void Remove_BrowserBased_ClientApplication_RemovesBrowserBasedEntries(
            ClientApplication clientApplication)
        {
            clientApplication.ClientApplicationTypes = new HashSet<string>
            {
                ApplicationType.BrowserBased.AsString(EnumFormat.EnumMemberValue),
                ApplicationType.Desktop.AsString(EnumFormat.EnumMemberValue),
                ApplicationType.MobileTablet.AsString(EnumFormat.EnumMemberValue),
            };

            var updatedClientApplication = SolutionsService.RemoveApplicationType(
                clientApplication,
                ApplicationType.BrowserBased);

            updatedClientApplication.ClientApplicationTypes.Should()
                .BeEquivalentTo(
                    new HashSet<string>
                    {
                        ApplicationType.Desktop.AsString(EnumFormat.EnumMemberValue),
                        ApplicationType.MobileTablet.AsString(EnumFormat.EnumMemberValue),
                    });

            // Browser Based
            updatedClientApplication.AdditionalInformation.Should().BeNull();
            updatedClientApplication.BrowsersSupported.Should().BeNull();
            updatedClientApplication.HardwareRequirements.Should().BeNull();
            updatedClientApplication.MinimumConnectionSpeed.Should().BeNull();
            updatedClientApplication.MinimumDesktopResolution.Should().BeNull();
            updatedClientApplication.MobileFirstDesign.Should().BeNull();
            updatedClientApplication.MobileResponsive.Should().BeNull();
            updatedClientApplication.Plugins.Should().BeNull();

            // Desktop
            updatedClientApplication.NativeDesktopAdditionalInformation.Should()
                .Be(clientApplication.NativeDesktopAdditionalInformation);
            updatedClientApplication.NativeDesktopHardwareRequirements.Should()
                .Be(clientApplication.NativeDesktopHardwareRequirements);
            updatedClientApplication.NativeDesktopMemoryAndStorage.Should()
                .BeEquivalentTo(clientApplication.NativeDesktopMemoryAndStorage);
            updatedClientApplication.NativeDesktopMinimumConnectionSpeed.Should()
                .Be(clientApplication.NativeDesktopMinimumConnectionSpeed);
            updatedClientApplication.NativeDesktopOperatingSystemsDescription.Should()
                .Be(clientApplication.NativeDesktopOperatingSystemsDescription);
            updatedClientApplication.NativeDesktopThirdParty.Should()
                .BeEquivalentTo(clientApplication.NativeDesktopThirdParty);

            // Mobile or Tablet
            updatedClientApplication.MobileConnectionDetails.Should()
                .BeEquivalentTo(clientApplication.MobileConnectionDetails);
            updatedClientApplication.MobileMemoryAndStorage.Should()
                .BeEquivalentTo(clientApplication.MobileMemoryAndStorage);
            updatedClientApplication.MobileOperatingSystems.Should()
                .BeEquivalentTo(clientApplication.MobileOperatingSystems);
            updatedClientApplication.MobileThirdParty.Should().BeEquivalentTo(clientApplication.MobileThirdParty);
            updatedClientApplication.NativeMobileAdditionalInformation.Should()
                .Be(clientApplication.NativeMobileAdditionalInformation);
            updatedClientApplication.NativeMobileFirstDesign.Should().Be(clientApplication.NativeMobileFirstDesign);
            updatedClientApplication.NativeMobileHardwareRequirements.Should()
                .Be(clientApplication.NativeMobileHardwareRequirements);
        }

        [Theory]
        [CommonAutoData]
        public static void Remove_Desktop_ClientApplication_RemovesDesktopEntries(ClientApplication clientApplication)
        {
            clientApplication.ClientApplicationTypes = new HashSet<string>
            {
                ApplicationType.BrowserBased.AsString(EnumFormat.EnumMemberValue),
                ApplicationType.Desktop.AsString(EnumFormat.EnumMemberValue),
                ApplicationType.MobileTablet.AsString(EnumFormat.EnumMemberValue),
            };

            var updatedClientApplication = SolutionsService.RemoveApplicationType(
                clientApplication,
                ApplicationType.Desktop);

            updatedClientApplication.ClientApplicationTypes.Should()
                .BeEquivalentTo(
                    new HashSet<string>
                    {
                        ApplicationType.BrowserBased.AsString(EnumFormat.EnumMemberValue),
                        ApplicationType.MobileTablet.AsString(EnumFormat.EnumMemberValue),
                    });

            // Browser Based
            updatedClientApplication.AdditionalInformation.Should().Be(clientApplication.AdditionalInformation);
            updatedClientApplication.BrowsersSupported.Should().BeEquivalentTo(clientApplication.BrowsersSupported);
            updatedClientApplication.HardwareRequirements.Should().Be(clientApplication.HardwareRequirements);
            updatedClientApplication.MinimumConnectionSpeed.Should().Be(clientApplication.MinimumConnectionSpeed);
            updatedClientApplication.MinimumDesktopResolution.Should().Be(clientApplication.MinimumDesktopResolution);
            updatedClientApplication.MobileFirstDesign.Should().Be(clientApplication.MobileFirstDesign);
            updatedClientApplication.MobileResponsive.Should().Be(clientApplication.MobileResponsive);
            updatedClientApplication.Plugins.Should().BeEquivalentTo(clientApplication.Plugins);

            // Desktop
            updatedClientApplication.NativeDesktopAdditionalInformation.Should().BeNull();
            updatedClientApplication.NativeDesktopHardwareRequirements.Should().BeNull();
            updatedClientApplication.NativeDesktopMemoryAndStorage.Should().BeNull();
            updatedClientApplication.NativeDesktopMinimumConnectionSpeed.Should().BeNull();
            updatedClientApplication.NativeDesktopOperatingSystemsDescription.Should().BeNull();
            updatedClientApplication.NativeDesktopThirdParty.Should().BeNull();

            // Mobile or Tablet
            updatedClientApplication.MobileConnectionDetails.Should()
                .BeEquivalentTo(clientApplication.MobileConnectionDetails);
            updatedClientApplication.MobileMemoryAndStorage.Should()
                .BeEquivalentTo(clientApplication.MobileMemoryAndStorage);
            updatedClientApplication.MobileOperatingSystems.Should()
                .BeEquivalentTo(clientApplication.MobileOperatingSystems);
            updatedClientApplication.MobileThirdParty.Should().BeEquivalentTo(clientApplication.MobileThirdParty);
            updatedClientApplication.NativeMobileAdditionalInformation.Should()
                .Be(clientApplication.NativeMobileAdditionalInformation);
            updatedClientApplication.NativeMobileFirstDesign.Should().Be(clientApplication.NativeMobileFirstDesign);
            updatedClientApplication.NativeMobileHardwareRequirements.Should()
                .Be(clientApplication.NativeMobileHardwareRequirements);
        }

        [Theory]
        [CommonAutoData]
        public static void Remove_Mobile_ClientApplication_RemovesMobileEntries(ClientApplication clientApplication)
        {
            clientApplication.ClientApplicationTypes = new HashSet<string>
            {
                ApplicationType.BrowserBased.AsString(EnumFormat.EnumMemberValue),
                ApplicationType.Desktop.AsString(EnumFormat.EnumMemberValue),
                ApplicationType.MobileTablet.AsString(EnumFormat.EnumMemberValue),
            };

            var updatedClientApplication = SolutionsService.RemoveApplicationType(
                clientApplication,
                ApplicationType.MobileTablet);

            updatedClientApplication.ClientApplicationTypes.Should()
                .BeEquivalentTo(
                    new HashSet<string>
                    {
                        ApplicationType.BrowserBased.AsString(EnumFormat.EnumMemberValue),
                        ApplicationType.Desktop.AsString(EnumFormat.EnumMemberValue),
                    });

            // Browser Based
            updatedClientApplication.AdditionalInformation.Should().Be(clientApplication.AdditionalInformation);
            updatedClientApplication.BrowsersSupported.Should().BeEquivalentTo(clientApplication.BrowsersSupported);
            updatedClientApplication.HardwareRequirements.Should().Be(clientApplication.HardwareRequirements);
            updatedClientApplication.MinimumConnectionSpeed.Should().Be(clientApplication.MinimumConnectionSpeed);
            updatedClientApplication.MinimumDesktopResolution.Should().Be(clientApplication.MinimumDesktopResolution);
            updatedClientApplication.MobileFirstDesign.Should().Be(clientApplication.MobileFirstDesign);
            updatedClientApplication.MobileResponsive.Should().Be(clientApplication.MobileResponsive);
            updatedClientApplication.Plugins.Should().BeEquivalentTo(clientApplication.Plugins);

            // Desktop
            updatedClientApplication.NativeDesktopAdditionalInformation.Should()
                .Be(clientApplication.NativeDesktopAdditionalInformation);
            updatedClientApplication.NativeDesktopHardwareRequirements.Should()
                .Be(clientApplication.NativeDesktopHardwareRequirements);
            updatedClientApplication.NativeDesktopMemoryAndStorage.Should()
                .BeEquivalentTo(clientApplication.NativeDesktopMemoryAndStorage);
            updatedClientApplication.NativeDesktopMinimumConnectionSpeed.Should()
                .Be(clientApplication.NativeDesktopMinimumConnectionSpeed);
            updatedClientApplication.NativeDesktopOperatingSystemsDescription.Should()
                .Be(clientApplication.NativeDesktopOperatingSystemsDescription);
            updatedClientApplication.NativeDesktopThirdParty.Should()
                .BeEquivalentTo(clientApplication.NativeDesktopThirdParty);

            // Mobile or Tablet
            updatedClientApplication.MobileConnectionDetails.Should().BeNull();
            updatedClientApplication.MobileMemoryAndStorage.Should().BeNull();
            updatedClientApplication.MobileOperatingSystems.Should().BeNull();
            updatedClientApplication.MobileThirdParty.Should().BeNull();
            updatedClientApplication.NativeMobileAdditionalInformation.Should().BeNull();
            updatedClientApplication.NativeMobileFirstDesign.Should().BeNull();
            updatedClientApplication.NativeMobileHardwareRequirements.Should().BeNull();
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData(" ")]
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
        [InMemoryDbAutoData]
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
        [InMemoryDbAutoData]
        public static async Task GetSupplierSolutions_ReturnsExpectedResults(
            Supplier supplier,
            List<Solution> solutions,
            [Frozen] BuyingCatalogueDbContext context,
            SolutionsService service)
        {
            var expectedSolutions = solutions.Where(x => x.FrameworkSolutions.Any(y => !y.Framework.IsExpired))
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

            var result = await service.GetSupplierSolutions(supplier.Id);

            result.Should().BeEquivalentTo(expectedSolutions);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetSupplierSolutionsWithAssociatedServices_ReturnsExpectedResults(
            Supplier supplier,
            AssociatedService associatedService,
            List<Solution> solutions,
            [Frozen] BuyingCatalogueDbContext context,
            SolutionsService service)
        {
            supplier.CatalogueItems.Clear();
            associatedService.CatalogueItem.Supplier = supplier;
            associatedService.CatalogueItem.PublishedStatus = PublicationStatus.Published;

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

            var expectedSolutions = solutions.Where(x => x.FrameworkSolutions.Any(y => !y.Framework.IsExpired))
                .Select(x => x.CatalogueItem)
                .ToList();

            context.AssociatedServices.Add(associatedService);
            context.Solutions.AddRange(solutions);
            context.Suppliers.Add(supplier);

            await context.SaveChangesAsync();

            var result = await service.GetSupplierSolutionsWithAssociatedServices(supplier.Id);

            result.Should().HaveCount(expectedSolutions.Count);
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
    }
}
