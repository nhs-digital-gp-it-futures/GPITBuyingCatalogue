using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.Services.Solutions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.TestData;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Solutions
{
    public static class SolutionsServiceTests
    {
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
            newContacts.Should().BeEquivalentTo(supplierContactsModel.Contacts, config => config
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

            var marketingContacts = await context.MarketingContacts.AsAsyncEnumerable().Where(c => c.SolutionId == model.SolutionId).ToListAsync();

            marketingContacts.Should().BeEquivalentTo(model.ValidContacts());
        }

        [Theory]
        [MemberData(nameof(InvalidStringData.TestData), MemberType = typeof(InvalidStringData))]
        public static async Task SaveSolutionDescription_InvalidSummary_ThrowsException(string summary)
        {
            var service = new SolutionsService(
                Mock.Of<BuyingCatalogueDbContext>(),
                Mock.Of<IDbRepository<Solution, BuyingCatalogueDbContext>>(),
                Mock.Of<IDbRepository<Supplier, BuyingCatalogueDbContext>>(),
                Mock.Of<ICatalogueItemRepository>());

            var actual = await Assert.ThrowsAsync<ArgumentException>(() => service.SaveSolutionDescription(
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

            context.Solutions.Add(solution);
            await context.SaveChangesAsync();

            await service.SaveSolutionDetails(solution.CatalogueItemId, expectedSolutionName, expectedSupplierId, new List<FrameworkModel>());

            var dbSolution = await context.CatalogueItems
                .Include(s => s.Solution)
                .Include(s => s.Solution.FrameworkSolutions)
                .SingleAsync(s => s.Id == solution.CatalogueItemId);

            dbSolution.Name.Should().BeEquivalentTo(expectedSolutionName);
            dbSolution.SupplierId.Should().Be(expectedSupplierId);
        }

        [Theory]
        [CommonAutoData]
        public static async Task SaveSolutionDescription_CallsSaveChangesAsync_OnRepository(
            [Frozen] Mock<IDbRepository<Solution, BuyingCatalogueDbContext>> solutionRepositoryMock,
            SolutionsService service)
        {
            solutionRepositoryMock
                .Setup(r => r.SingleAsync(It.IsAny<Expression<Func<Solution, bool>>>()))
                .ReturnsAsync(new Solution());

            await service.SaveSolutionDescription(new CatalogueItemId(100000, "001"), "Summary", "Description", "Link");

            solutionRepositoryMock.Verify(r => r.SaveChangesAsync());
        }

        [Theory]
        [CommonAutoData]
        public static async Task SaveSolutionFeatures_CallsSaveChangesAsync_OnRepository(
            [Frozen] Mock<IDbRepository<Solution, BuyingCatalogueDbContext>> solutionRepositoryMock,
            SolutionsService service)
        {
            solutionRepositoryMock
                .Setup(r => r.SingleAsync(It.IsAny<Expression<Func<Solution, bool>>>()))
                .ReturnsAsync(new Solution());

            await service.SaveSolutionFeatures(new CatalogueItemId(100000, "001"), Array.Empty<string>());

            solutionRepositoryMock.Verify(r => r.SaveChangesAsync());
        }

        [Theory]
        [CommonAutoData]
        public static async Task SaveImplementationDetail_CallsSaveChangesAsync_OnRepository(
            [Frozen] Mock<IDbRepository<Solution, BuyingCatalogueDbContext>> solutionRepositoryMock,
            SolutionsService service)
        {
            solutionRepositoryMock
                .Setup(r => r.SingleAsync(It.IsAny<Expression<Func<Solution, bool>>>()))
                .ReturnsAsync(new Solution());

            await service.SaveImplementationDetail(new CatalogueItemId(100000, "001"), "123");

            solutionRepositoryMock.Verify(r => r.SaveChangesAsync());
        }

        [Theory]
        [CommonAutoData]
        public static async Task SaveRoadMap_CallsSaveChangesAsync_OnRepository(
            [Frozen] Mock<IDbRepository<Solution, BuyingCatalogueDbContext>> solutionRepositoryMock,
            SolutionsService service)
        {
            solutionRepositoryMock
                .Setup(r => r.SingleAsync(It.IsAny<Expression<Func<Solution, bool>>>()))
                .ReturnsAsync(new Solution());

            await service.SaveRoadMap(new CatalogueItemId(100000, "001"), "123");

            solutionRepositoryMock.Verify(r => r.SaveChangesAsync());
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
        [CommonAutoData]
        public static async Task SaveClientApplication_CallsSaveChangesAsync_OnRepository(
            [Frozen] Mock<IDbRepository<Solution, BuyingCatalogueDbContext>> solutionRepositoryMock,
            SolutionsService service)
        {
            solutionRepositoryMock
                .Setup(r => r.SingleAsync(It.IsAny<Expression<Func<Solution, bool>>>()))
                .ReturnsAsync(new Solution());

            await service.SaveClientApplication(new CatalogueItemId(100000, "001"), new ClientApplication());

            solutionRepositoryMock.Verify(r => r.SaveChangesAsync());
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
        [CommonAutoData]
        public static async Task SaveHosting_CallsSaveChangesAsync_OnRepository(
            [Frozen] Mock<IDbRepository<Solution, BuyingCatalogueDbContext>> solutionRepositoryMock,
            SolutionsService service)
        {
            solutionRepositoryMock
                .Setup(r => r.SingleAsync(It.IsAny<Expression<Func<Solution, bool>>>()))
                .ReturnsAsync(new Solution());

            await service.SaveHosting(new CatalogueItemId(100000, "001"), new Hosting());

            solutionRepositoryMock.Verify(r => r.SaveChangesAsync());
        }

        [Theory]
        [CommonAutoData]
        public static async Task SaveSupplier_CallsSaveChangesAsync_OnRepository(
            [Frozen] Mock<IDbRepository<Supplier, BuyingCatalogueDbContext>> supplierRepositoryMock,
            SolutionsService service)
        {
            supplierRepositoryMock
                .Setup(r => r.SingleAsync(It.IsAny<Expression<Func<Supplier, bool>>>()))
                .ReturnsAsync(new Supplier());

            await service.SaveSupplierDescriptionAndLink(100000, "Description", "Link");

            supplierRepositoryMock.Verify(r => r.SaveChangesAsync());
        }

        [Theory]
        [CommonAutoData]
        public static async Task AddCatalogueSolution_NullModel_ThrowsException(SolutionsService service)
        {
            (await Assert.ThrowsAsync<ArgumentNullException>(() => service.AddCatalogueSolution(null)))
                .ParamName.Should().Be("model");
        }

        [Theory]
        [CommonAutoData]
        public static async Task AddCatalogueSolution_NullListOfFrameWorkModels_ThrowsException(SolutionsService service)
        {
            (await Assert.ThrowsAsync<ArgumentNullException>(
                    () => service.AddCatalogueSolution(new CreateSolutionModel())))
                .ParamName.Should()
                .Be(nameof(CreateSolutionModel.Frameworks));
        }

        [Theory]
        [CommonAutoData]
        public static async Task AddCatalogueSolution_ModelValid_AddsCatalogueItemToRepository(
            CreateSolutionModel model,
            [Frozen] Mock<ICatalogueItemRepository> catalogueItemRepositoryMock,
            SolutionsService service)
        {
            var catalogueItemId = new CatalogueItemId(model.SupplierId, "045");

            await service.AddCatalogueSolution(model);

            catalogueItemRepositoryMock.Verify(
                repository => repository.Add(
                    It.Is<CatalogueItem>(
                        c =>
                            c.CatalogueItemType == CatalogueItemType.Solution &&
                            c.Solution.LastUpdated > DateTime.UtcNow.AddMinutes(-2) &&
                            c.Solution.LastUpdatedBy == model.UserId &&
                            c.Name == model.Name &&
                            c.PublishedStatus == PublicationStatus.Draft &&
                            c.SupplierId == model.SupplierId)));
        }

        [Theory]
        [CommonAutoData]
        public static async Task SupplierHasSolutionName_Returns_FromRepository(
            int supplierId,
            string solutionName,
            [Frozen] Mock<ICatalogueItemRepository> mockCatalogueItemRepository,
            SolutionsService service)
        {
            var expected = DateTime.Now.Ticks % 2 == 0;

            mockCatalogueItemRepository
                .Setup(c => c.SupplierHasSolutionName(supplierId, solutionName))
                .ReturnsAsync(expected);

            var actual = await service.SupplierHasSolutionName(supplierId, solutionName);

            mockCatalogueItemRepository.Verify(c => c.SupplierHasSolutionName(supplierId, solutionName));
            actual.Should().Be(expected);
        }

        [Theory]
        [CommonAutoData]
        public static async Task DeleteClientApplication_SavesChanges(
            [Frozen] CatalogueItemId catalogueItemId,
            Solution catalogueSolution,
            [Frozen] Mock<IDbRepository<Solution, BuyingCatalogueDbContext>> mockSolutionsRepository,
            SolutionsService service)
        {
            mockSolutionsRepository.Setup(r => r.SingleAsync(s => s.CatalogueItemId == catalogueItemId)).ReturnsAsync(catalogueSolution);

            await service.DeleteClientApplication(catalogueItemId, ClientApplicationType.BrowserBased);

            mockSolutionsRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Theory]
        [CommonAutoData]
        public static void Remove_BrowserBased_ClientApplication_RemovesBrowserBasedEntries(
            ClientApplication clientApplication)
        {
            clientApplication.ClientApplicationTypes = new HashSet<string>
            {
                ClientApplicationType.BrowserBased.AsString(EnumFormat.EnumMemberValue),
                ClientApplicationType.Desktop.AsString(EnumFormat.EnumMemberValue),
                ClientApplicationType.MobileTablet.AsString(EnumFormat.EnumMemberValue),
            };

            var updatedClientApplication = SolutionsService.RemoveClientApplicationType(clientApplication, ClientApplicationType.BrowserBased);

            updatedClientApplication.ClientApplicationTypes.Should().BeEquivalentTo(
                    new HashSet<string>
                    {
                        ClientApplicationType.Desktop.AsString(EnumFormat.EnumMemberValue),
                        ClientApplicationType.MobileTablet.AsString(EnumFormat.EnumMemberValue),
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
            updatedClientApplication.NativeDesktopAdditionalInformation.Should().Be(clientApplication.NativeDesktopAdditionalInformation);
            updatedClientApplication.NativeDesktopHardwareRequirements.Should().Be(clientApplication.NativeDesktopHardwareRequirements);
            updatedClientApplication.NativeDesktopMemoryAndStorage.Should().BeEquivalentTo(clientApplication.NativeDesktopMemoryAndStorage);
            updatedClientApplication.NativeDesktopMinimumConnectionSpeed.Should().Be(clientApplication.NativeDesktopMinimumConnectionSpeed);
            updatedClientApplication.NativeDesktopOperatingSystemsDescription.Should().Be(clientApplication.NativeDesktopOperatingSystemsDescription);
            updatedClientApplication.NativeDesktopThirdParty.Should().BeEquivalentTo(clientApplication.NativeDesktopThirdParty);

            // Mobile or Tablet
            updatedClientApplication.MobileConnectionDetails.Should().BeEquivalentTo(clientApplication.MobileConnectionDetails);
            updatedClientApplication.MobileMemoryAndStorage.Should().BeEquivalentTo(clientApplication.MobileMemoryAndStorage);
            updatedClientApplication.MobileOperatingSystems.Should().BeEquivalentTo(clientApplication.MobileOperatingSystems);
            updatedClientApplication.MobileThirdParty.Should().BeEquivalentTo(clientApplication.MobileThirdParty);
            updatedClientApplication.NativeMobileAdditionalInformation.Should().Be(clientApplication.NativeMobileAdditionalInformation);
            updatedClientApplication.NativeMobileFirstDesign.Should().Be(clientApplication.NativeMobileFirstDesign);
            updatedClientApplication.NativeMobileHardwareRequirements.Should().Be(clientApplication.NativeMobileHardwareRequirements);
        }

        [Theory]
        [CommonAutoData]
        public static void Remove_Desktop_ClientApplication_RemovesDesktopEntries(ClientApplication clientApplication)
        {
            clientApplication.ClientApplicationTypes = new HashSet<string>
            {
                ClientApplicationType.BrowserBased.AsString(EnumFormat.EnumMemberValue),
                ClientApplicationType.Desktop.AsString(EnumFormat.EnumMemberValue),
                ClientApplicationType.MobileTablet.AsString(EnumFormat.EnumMemberValue),
            };

            var updatedClientApplication = SolutionsService.RemoveClientApplicationType(clientApplication, ClientApplicationType.Desktop);

            updatedClientApplication.ClientApplicationTypes.Should().BeEquivalentTo(
                    new HashSet<string>
                    {
                        ClientApplicationType.BrowserBased.AsString(EnumFormat.EnumMemberValue),
                        ClientApplicationType.MobileTablet.AsString(EnumFormat.EnumMemberValue),
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
            updatedClientApplication.MobileConnectionDetails.Should().BeEquivalentTo(clientApplication.MobileConnectionDetails);
            updatedClientApplication.MobileMemoryAndStorage.Should().BeEquivalentTo(clientApplication.MobileMemoryAndStorage);
            updatedClientApplication.MobileOperatingSystems.Should().BeEquivalentTo(clientApplication.MobileOperatingSystems);
            updatedClientApplication.MobileThirdParty.Should().BeEquivalentTo(clientApplication.MobileThirdParty);
            updatedClientApplication.NativeMobileAdditionalInformation.Should().Be(clientApplication.NativeMobileAdditionalInformation);
            updatedClientApplication.NativeMobileFirstDesign.Should().Be(clientApplication.NativeMobileFirstDesign);
            updatedClientApplication.NativeMobileHardwareRequirements.Should().Be(clientApplication.NativeMobileHardwareRequirements);
        }

        [Theory]
        [CommonAutoData]
        public static void Remove_Mobile_ClientApplication_RemovesMobileEntries(ClientApplication clientApplication)
        {
            clientApplication.ClientApplicationTypes = new HashSet<string>
            {
                ClientApplicationType.BrowserBased.AsString(EnumFormat.EnumMemberValue),
                ClientApplicationType.Desktop.AsString(EnumFormat.EnumMemberValue),
                ClientApplicationType.MobileTablet.AsString(EnumFormat.EnumMemberValue),
            };

            var updatedClientApplication = SolutionsService.RemoveClientApplicationType(clientApplication, ClientApplicationType.MobileTablet);

            updatedClientApplication.ClientApplicationTypes.Should().BeEquivalentTo(
                    new HashSet<string>
                    {
                        ClientApplicationType.BrowserBased.AsString(EnumFormat.EnumMemberValue),
                        ClientApplicationType.Desktop.AsString(EnumFormat.EnumMemberValue),
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
            updatedClientApplication.NativeDesktopAdditionalInformation.Should().Be(clientApplication.NativeDesktopAdditionalInformation);
            updatedClientApplication.NativeDesktopHardwareRequirements.Should().Be(clientApplication.NativeDesktopHardwareRequirements);
            updatedClientApplication.NativeDesktopMemoryAndStorage.Should().BeEquivalentTo(clientApplication.NativeDesktopMemoryAndStorage);
            updatedClientApplication.NativeDesktopMinimumConnectionSpeed.Should().Be(clientApplication.NativeDesktopMinimumConnectionSpeed);
            updatedClientApplication.NativeDesktopOperatingSystemsDescription.Should().Be(clientApplication.NativeDesktopOperatingSystemsDescription);
            updatedClientApplication.NativeDesktopThirdParty.Should().BeEquivalentTo(clientApplication.NativeDesktopThirdParty);

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
        [InMemoryDbAutoData]
        public static async Task SavePublicationStatus_Updates_PublicationStatus(
            [Frozen] BuyingCatalogueDbContext context,
            CatalogueItem solution,
            SolutionsService service)
        {
            solution.PublishedStatus = PublicationStatus.Draft;
            context.CatalogueItems.Add(solution);
            await context.SaveChangesAsync();

            await service.SavePublicationStatus(solution.Id, PublicationStatus.Published);

            var updatedSolution = await context.CatalogueItems.AsQueryable().SingleAsync(c => c.Id == solution.Id);

            updatedSolution.PublishedStatus.Should().Be(PublicationStatus.Published);
        }
    }
}
