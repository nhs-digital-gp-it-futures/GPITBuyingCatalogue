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

        [Fact]
        public static async Task SaveSupplierContacts_ModelNull_ThrowsException()
        {
            var service = new SolutionsService(
                Mock.Of<BuyingCatalogueDbContext>(),
                Mock.Of<IDbRepository<MarketingContact, BuyingCatalogueDbContext>>(),
                Mock.Of<IDbRepository<Solution, BuyingCatalogueDbContext>>(),
                Mock.Of<IDbRepository<Supplier, BuyingCatalogueDbContext>>(),
                Mock.Of<ICatalogueItemRepository>());

            var actual = await Assert.ThrowsAsync<ArgumentNullException>(() => service.SaveSupplierContacts(default));

            actual.ParamName.Should().Be("model");
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task SaveSupplierContacts_ModelValid_CallsSetSolutionIdOnModel(
            [Frozen] BuyingCatalogueDbContext context,
            CatalogueItem solution,
            SolutionsService service,
            SupplierContactsModel model)
        {
            solution.Solution.MarketingContacts.Clear();
            context.CatalogueItems.Add(solution);
            await context.SaveChangesAsync();

            model.SolutionId = solution.Id;

            await service.SaveSupplierContacts(model);

            model.Contacts.Select(c => c.SolutionId).Should().AllBeEquivalentTo(model.SolutionId);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task SaveSupplierContacts_NoContactsInDatabase_AddsValidContactsToRepository(
            [Frozen] BuyingCatalogueDbContext context,
            CatalogueItem solution,
            SupplierContactsModel supplierContactsModel,
            SolutionsService service)
        {
            solution.Solution.MarketingContacts.Clear();
            context.CatalogueItems.Add(solution);
            await context.SaveChangesAsync();

            supplierContactsModel.SolutionId = solution.Id;

            await service.SaveSupplierContacts(supplierContactsModel);

            var newContacts = await context.MarketingContacts.AsAsyncEnumerable().Where(mc => mc.SolutionId == solution.Id).ToArrayAsync();

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
            CatalogueItem solution,
            SupplierContactsModel supplierContactsModel,
            SolutionsService service)
        {
            context.CatalogueItems.Add(solution);
            await context.SaveChangesAsync();

            supplierContactsModel.Contacts = await context.MarketingContacts.AsAsyncEnumerable().Where(mc => mc.SolutionId == solution.Id).ToArrayAsync();

            supplierContactsModel.Contacts[0].FirstName = null;
            supplierContactsModel.Contacts[0].LastName = null;
            supplierContactsModel.Contacts[0].Department = null;
            supplierContactsModel.Contacts[0].PhoneNumber = null;
            supplierContactsModel.Contacts[0].Email = null;

            supplierContactsModel.SolutionId = solution.Id;

            await service.SaveSupplierContacts(supplierContactsModel);

            var updatedContacts = await context.MarketingContacts.AsAsyncEnumerable().Where(mc => mc.SolutionId == solution.Id).ToArrayAsync();

            updatedContacts.Length.Should().Be(2);
            updatedContacts[0].Id.Should().Be(supplierContactsModel.Contacts[1].Id);
            updatedContacts[1].Id.Should().Be(supplierContactsModel.Contacts[2].Id);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task SaveSupplierContacts_ContactsInDatabase_UpdatesNonEmptyContacts(
            [Frozen] BuyingCatalogueDbContext context,
            CatalogueItem solution,
            SupplierContactsModel model,
            string updatedFirstName,
            string updatedLastName,
            string updatedPhoneNumber,
            string updatedEmail,
            SolutionsService service)
        {
            context.CatalogueItems.Add(solution);
            await context.SaveChangesAsync();

            model.Contacts = await context.MarketingContacts.AsAsyncEnumerable().Where(mc => mc.SolutionId == solution.Id).ToArrayAsync();

            model.Contacts[0].FirstName = updatedFirstName;
            model.Contacts[0].LastName = updatedLastName;
            model.Contacts[0].PhoneNumber = updatedPhoneNumber;
            model.Contacts[0].Email = updatedEmail;
            model.SolutionId = solution.Id;

            await service.SaveSupplierContacts(model);

            var updatedContacts = await context.MarketingContacts.AsAsyncEnumerable().Where(mc => mc.SolutionId == solution.Id).ToArrayAsync();

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
                Mock.Of<IDbRepository<MarketingContact, BuyingCatalogueDbContext>>(),
                Mock.Of<IDbRepository<Solution, BuyingCatalogueDbContext>>(),
                Mock.Of<IDbRepository<Supplier, BuyingCatalogueDbContext>>(),
                Mock.Of<ICatalogueItemRepository>());

            var actual = await Assert.ThrowsAsync<ArgumentException>(() => service.SaveSolutionDescription(new CatalogueItemId(100000, "001"), summary, "Description", "Link"));

            actual.ParamName.Should().Be("summary");
        }

        [Fact]
        public static async Task SaveSolutionDescription_CallsSaveChangesAsync_OnRepository()
        {
            var mockSolutionRepository = new Mock<IDbRepository<Solution, BuyingCatalogueDbContext>>();
            mockSolutionRepository.Setup(r => r.SingleAsync(It.IsAny<Expression<Func<Solution, bool>>>()))
                .ReturnsAsync(new Solution());

            var service = new SolutionsService(
                Mock.Of<BuyingCatalogueDbContext>(),
                Mock.Of<IDbRepository<MarketingContact, BuyingCatalogueDbContext>>(),
                mockSolutionRepository.Object,
                Mock.Of<IDbRepository<Supplier, BuyingCatalogueDbContext>>(),
                Mock.Of<ICatalogueItemRepository>());

            await service.SaveSolutionDescription(new CatalogueItemId(100000, "001"), "Summary", "Description", "Link");

            mockSolutionRepository.Verify(r => r.SaveChangesAsync());
        }

        [Fact]
        public static async Task SaveSolutionFeatures_CallsSaveChangesAsync_OnRepository()
        {
            var mockSolutionRepository = new Mock<IDbRepository<Solution, BuyingCatalogueDbContext>>();
            mockSolutionRepository.Setup(r => r.SingleAsync(It.IsAny<Expression<Func<Solution, bool>>>()))
                .ReturnsAsync(new Solution());

            var service = new SolutionsService(
                Mock.Of<BuyingCatalogueDbContext>(),
                Mock.Of<IDbRepository<MarketingContact, BuyingCatalogueDbContext>>(),
                mockSolutionRepository.Object,
                Mock.Of<IDbRepository<Supplier, BuyingCatalogueDbContext>>(),
                Mock.Of<ICatalogueItemRepository>());

            await service.SaveSolutionFeatures(new CatalogueItemId(100000, "001"), Array.Empty<string>());

            mockSolutionRepository.Verify(r => r.SaveChangesAsync());
        }

        [Fact]
        public static async Task SaveImplementationDetail_CallsSaveChangesAsync_OnRepository()
        {
            var mockSolutionRepository = new Mock<IDbRepository<Solution, BuyingCatalogueDbContext>>();
            mockSolutionRepository.Setup(r => r.SingleAsync(It.IsAny<Expression<Func<Solution, bool>>>()))
                .ReturnsAsync(new Solution());

            var service = new SolutionsService(
                Mock.Of<BuyingCatalogueDbContext>(),
                Mock.Of<IDbRepository<MarketingContact, BuyingCatalogueDbContext>>(),
                mockSolutionRepository.Object,
                Mock.Of<IDbRepository<Supplier, BuyingCatalogueDbContext>>(),
                Mock.Of<ICatalogueItemRepository>());

            await service.SaveImplementationDetail(new CatalogueItemId(100000, "001"), "123");

            mockSolutionRepository.Verify(r => r.SaveChangesAsync());
        }

        [Fact]
        public static async Task SaveRoadMap_CallsSaveChangesAsync_OnRepository()
        {
            var mockSolutionRepository = new Mock<IDbRepository<Solution, BuyingCatalogueDbContext>>();
            mockSolutionRepository.Setup(r => r.SingleAsync(It.IsAny<Expression<Func<Solution, bool>>>()))
                .ReturnsAsync(new Solution());

            var service = new SolutionsService(
                Mock.Of<BuyingCatalogueDbContext>(),
                Mock.Of<IDbRepository<MarketingContact, BuyingCatalogueDbContext>>(),
                mockSolutionRepository.Object,
                Mock.Of<IDbRepository<Supplier, BuyingCatalogueDbContext>>(),
                Mock.Of<ICatalogueItemRepository>());

            await service.SaveRoadMap(new CatalogueItemId(100000, "001"), "123");

            mockSolutionRepository.Verify(r => r.SaveChangesAsync());
        }

        [Fact]
        public static async Task SaveClientApplication_InvalidModel_ThrowsException()
        {
            var service = new SolutionsService(
                Mock.Of<BuyingCatalogueDbContext>(),
                Mock.Of<IDbRepository<MarketingContact, BuyingCatalogueDbContext>>(),
                Mock.Of<IDbRepository<Solution, BuyingCatalogueDbContext>>(),
                Mock.Of<IDbRepository<Supplier, BuyingCatalogueDbContext>>(),
                Mock.Of<ICatalogueItemRepository>());

            var actual = await Assert.ThrowsAsync<ArgumentNullException>(() => service.SaveClientApplication(new CatalogueItemId(100000, "001"), null));

            actual.ParamName.Should().Be("clientApplication");
        }

        [Fact]
        public static async Task SaveClientApplication_CallsSaveChangesAsync_OnRepository()
        {
            var mockSolutionRepository = new Mock<IDbRepository<Solution, BuyingCatalogueDbContext>>();
            mockSolutionRepository.Setup(r => r.SingleAsync(It.IsAny<Expression<Func<Solution, bool>>>()))
                .ReturnsAsync(new Solution());

            var service = new SolutionsService(
                Mock.Of<BuyingCatalogueDbContext>(),
                Mock.Of<IDbRepository<MarketingContact, BuyingCatalogueDbContext>>(),
                mockSolutionRepository.Object,
                Mock.Of<IDbRepository<Supplier, BuyingCatalogueDbContext>>(),
                Mock.Of<ICatalogueItemRepository>());

            await service.SaveClientApplication(new CatalogueItemId(100000, "001"), new ClientApplication());

            mockSolutionRepository.Verify(r => r.SaveChangesAsync());
        }

        [Fact]
        public static async Task SaveHosting_InvalidModel_ThrowsException()
        {
            var service = new SolutionsService(
                Mock.Of<BuyingCatalogueDbContext>(),
                Mock.Of<IDbRepository<MarketingContact, BuyingCatalogueDbContext>>(),
                Mock.Of<IDbRepository<Solution, BuyingCatalogueDbContext>>(),
                Mock.Of<IDbRepository<Supplier, BuyingCatalogueDbContext>>(),
                Mock.Of<ICatalogueItemRepository>());

            var actual = await Assert.ThrowsAsync<ArgumentNullException>(() => service.SaveHosting(new CatalogueItemId(100000, "001"), null));

            actual.ParamName.Should().Be("hosting");
        }

        [Fact]
        public static async Task SaveHosting_CallsSaveChangesAsync_OnRepository()
        {
            var mockSolutionRepository = new Mock<IDbRepository<Solution, BuyingCatalogueDbContext>>();
            mockSolutionRepository.Setup(r => r.SingleAsync(It.IsAny<Expression<Func<Solution, bool>>>()))
                .ReturnsAsync(new Solution());

            var service = new SolutionsService(
                Mock.Of<BuyingCatalogueDbContext>(),
                Mock.Of<IDbRepository<MarketingContact, BuyingCatalogueDbContext>>(),
                mockSolutionRepository.Object,
                Mock.Of<IDbRepository<Supplier, BuyingCatalogueDbContext>>(),
                Mock.Of<ICatalogueItemRepository>());

            await service.SaveHosting(new CatalogueItemId(100000, "001"), new Hosting());

            mockSolutionRepository.Verify(r => r.SaveChangesAsync());
        }

        [Fact]
        public static async Task SaveSupplier_CallsSaveChangesAsync_OnRepository()
        {
            var mockSupplierRepository = new Mock<IDbRepository<Supplier, BuyingCatalogueDbContext>>();
            mockSupplierRepository.Setup(r => r.SingleAsync(It.IsAny<Expression<Func<Supplier, bool>>>()))
                .ReturnsAsync(new Supplier());

            var service = new SolutionsService(
                Mock.Of<BuyingCatalogueDbContext>(),
                Mock.Of<IDbRepository<MarketingContact, BuyingCatalogueDbContext>>(),
                Mock.Of<IDbRepository<Solution, BuyingCatalogueDbContext>>(),
                mockSupplierRepository.Object,
                Mock.Of<ICatalogueItemRepository>());

            await service.SaveSupplierDescriptionAndLink(100000, "Description", "Link");

            mockSupplierRepository.Verify(r => r.SaveChangesAsync());
        }

        [Fact]
        public static async Task AddCatalogueSolution_NullModel_ThrowsException()
        {
            var service = new SolutionsService(
                Mock.Of<BuyingCatalogueDbContext>(),
                Mock.Of<IDbRepository<MarketingContact, BuyingCatalogueDbContext>>(),
                Mock.Of<IDbRepository<Solution, BuyingCatalogueDbContext>>(),
                Mock.Of<IDbRepository<Supplier, BuyingCatalogueDbContext>>(),
                Mock.Of<ICatalogueItemRepository>());

            (await Assert.ThrowsAsync<ArgumentNullException>(() => service.AddCatalogueSolution(null)))
                .ParamName.Should().Be(nameof(CreateSolutionModel));
        }

        [Fact]
        public static async Task AddCatalogueSolution_NullListOfFrameWorkModels_ThrowsException()
        {
            var service = new SolutionsService(
                Mock.Of<BuyingCatalogueDbContext>(),
                Mock.Of<IDbRepository<MarketingContact, BuyingCatalogueDbContext>>(),
                Mock.Of<IDbRepository<Solution, BuyingCatalogueDbContext>>(),
                Mock.Of<IDbRepository<Supplier, BuyingCatalogueDbContext>>(),
                Mock.Of<ICatalogueItemRepository>());

            (await Assert.ThrowsAsync<ArgumentNullException>(
                    () => service.AddCatalogueSolution(new CreateSolutionModel())))
                .ParamName.Should()
                .Be(nameof(CreateSolutionModel.Frameworks));
        }

        [Theory]
        [CommonAutoData]
        public static async Task AddCatalogueSolution_ModelValid_GetsLatestCatalogueItemId(
            CreateSolutionModel model)
        {
            var mockCatalogueItemRepository = new Mock<ICatalogueItemRepository>();
            mockCatalogueItemRepository.Setup(c => c.GetLatestCatalogueItemIdFor(model.SupplierId))
                .ReturnsAsync(new CatalogueItemId(model.SupplierId, "045"));

            var service = new SolutionsService(
                Mock.Of<BuyingCatalogueDbContext>(),
                Mock.Of<IDbRepository<MarketingContact, BuyingCatalogueDbContext>>(),
                Mock.Of<IDbRepository<Solution, BuyingCatalogueDbContext>>(),
                Mock.Of<IDbRepository<Supplier, BuyingCatalogueDbContext>>(),
                mockCatalogueItemRepository.Object);

            await service.AddCatalogueSolution(model);

            mockCatalogueItemRepository.Verify(c => c.GetLatestCatalogueItemIdFor(model.SupplierId));
        }

        [Theory]
        [CommonAutoData]
        public static async Task AddCatalogueSolution_ModelValid_AddsCatalogueItemToRepository(
            CreateSolutionModel model)
        {
            var catalogueItemId = new CatalogueItemId(model.SupplierId, "045");
            var mockCatalogueItemRepository = new Mock<ICatalogueItemRepository>();
            mockCatalogueItemRepository.Setup(c => c.GetLatestCatalogueItemIdFor(model.SupplierId))
                .ReturnsAsync(catalogueItemId);

            var service = new SolutionsService(
                Mock.Of<BuyingCatalogueDbContext>(),
                Mock.Of<IDbRepository<MarketingContact, BuyingCatalogueDbContext>>(),
                Mock.Of<IDbRepository<Solution, BuyingCatalogueDbContext>>(),
                Mock.Of<IDbRepository<Supplier, BuyingCatalogueDbContext>>(),
                mockCatalogueItemRepository.Object);

            await service.AddCatalogueSolution(model);

            mockCatalogueItemRepository.Verify(
                repository => repository.Add(
                    It.Is<CatalogueItem>(
                        c =>
                            c.Id == catalogueItemId.NextSolutionId() &&
                            c.CatalogueItemType == CatalogueItemType.Solution &&
                            c.Solution.LastUpdated > DateTime.UtcNow.AddMinutes(-2) &&
                            c.Solution.LastUpdatedBy == model.UserId &&
                            c.Name == model.Name &&
                            c.PublishedStatus == PublicationStatus.Draft &&
                            c.SupplierId == model.SupplierId)));
        }

        [Theory]
        [AutoData]
        public static async Task SupplierHasSolutionName_Returns_FromRepository(
            int supplierId,
            string solutionName,
            Mock<ICatalogueItemRepository> mockCatalogueItemRepository)
        {
            var expected = DateTime.Now.Ticks % 2 == 0;
            mockCatalogueItemRepository.Setup(c => c.SupplierHasSolutionName(supplierId, solutionName))
                .ReturnsAsync(expected);
            var service = new SolutionsService(
                Mock.Of<BuyingCatalogueDbContext>(),
                Mock.Of<IDbRepository<MarketingContact, BuyingCatalogueDbContext>>(),
                Mock.Of<IDbRepository<Solution, BuyingCatalogueDbContext>>(),
                Mock.Of<IDbRepository<Supplier, BuyingCatalogueDbContext>>(),
                mockCatalogueItemRepository.Object);

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
            ClientApplication clientApplication,
            SolutionsService service)
        {
            clientApplication.ClientApplicationTypes = new HashSet<string>
            {
                ClientApplicationType.BrowserBased.AsString(EnumFormat.EnumMemberValue),
                ClientApplicationType.Desktop.AsString(EnumFormat.EnumMemberValue),
                ClientApplicationType.MobileTablet.AsString(EnumFormat.EnumMemberValue),
            };

            var updatedClientApplication = service.RemoveClientApplicationType(clientApplication, ClientApplicationType.BrowserBased);

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
            updatedClientApplication.NativeDesktopAdditionalInformation.Should().Equals(clientApplication.NativeDesktopAdditionalInformation);
            updatedClientApplication.NativeDesktopHardwareRequirements.Should().Equals(clientApplication.NativeDesktopHardwareRequirements);
            updatedClientApplication.NativeDesktopMemoryAndStorage.Should().BeEquivalentTo(clientApplication.NativeDesktopMemoryAndStorage);
            updatedClientApplication.NativeDesktopMinimumConnectionSpeed.Should().Equals(clientApplication.NativeDesktopMinimumConnectionSpeed);
            updatedClientApplication.NativeDesktopOperatingSystemsDescription.Should().Equals(clientApplication.NativeDesktopOperatingSystemsDescription);
            updatedClientApplication.NativeDesktopThirdParty.Should().BeEquivalentTo(clientApplication.NativeDesktopThirdParty);

            // Mobile or Tablet
            updatedClientApplication.MobileConnectionDetails.Should().BeEquivalentTo(clientApplication.MobileConnectionDetails);
            updatedClientApplication.MobileMemoryAndStorage.Should().BeEquivalentTo(clientApplication.MobileMemoryAndStorage);
            updatedClientApplication.MobileOperatingSystems.Should().BeEquivalentTo(clientApplication.MobileOperatingSystems);
            updatedClientApplication.MobileThirdParty.Should().BeEquivalentTo(clientApplication.MobileThirdParty);
            updatedClientApplication.NativeMobileAdditionalInformation.Should().Equals(clientApplication.NativeMobileAdditionalInformation);
            updatedClientApplication.NativeMobileFirstDesign.Should().Equals(clientApplication.NativeMobileFirstDesign);
            updatedClientApplication.NativeMobileHardwareRequirements.Should().Equals(clientApplication.NativeMobileHardwareRequirements);
        }

        [Theory]
        [CommonAutoData]
        public static void Remove_Desktop_ClientApplication_RemovesDesktopEntries(
                  ClientApplication clientApplication,
                  SolutionsService service)
        {
            clientApplication.ClientApplicationTypes = new HashSet<string>
            {
                ClientApplicationType.BrowserBased.AsString(EnumFormat.EnumMemberValue),
                ClientApplicationType.Desktop.AsString(EnumFormat.EnumMemberValue),
                ClientApplicationType.MobileTablet.AsString(EnumFormat.EnumMemberValue),
            };

            var updatedClientApplication = service.RemoveClientApplicationType(clientApplication, ClientApplicationType.Desktop);

            updatedClientApplication.ClientApplicationTypes.Should().BeEquivalentTo(
                    new HashSet<string>
                    {
                        ClientApplicationType.BrowserBased.AsString(EnumFormat.EnumMemberValue),
                        ClientApplicationType.MobileTablet.AsString(EnumFormat.EnumMemberValue),
                    });

            // Browser Based
            updatedClientApplication.AdditionalInformation.Should().Equals(clientApplication.AdditionalInformation);
            updatedClientApplication.BrowsersSupported.Should().BeEquivalentTo(clientApplication.BrowsersSupported);
            updatedClientApplication.HardwareRequirements.Should().Equals(clientApplication.HardwareRequirements);
            updatedClientApplication.MinimumConnectionSpeed.Should().Equals(clientApplication.MinimumConnectionSpeed);
            updatedClientApplication.MinimumDesktopResolution.Should().Equals(clientApplication.MinimumDesktopResolution);
            updatedClientApplication.MobileFirstDesign.Should().Equals(clientApplication.MobileFirstDesign);
            updatedClientApplication.MobileResponsive.Should().Equals(clientApplication.MobileResponsive);
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
            updatedClientApplication.NativeMobileAdditionalInformation.Should().Equals(clientApplication.NativeMobileAdditionalInformation);
            updatedClientApplication.NativeMobileFirstDesign.Should().Equals(clientApplication.NativeMobileFirstDesign);
            updatedClientApplication.NativeMobileHardwareRequirements.Should().Equals(clientApplication.NativeMobileHardwareRequirements);
        }

        [Theory]
        [CommonAutoData]
        public static void Remove_Mobile_ClientApplication_RemovesMobileEntries(
                         ClientApplication clientApplication,
                         SolutionsService service)
        {
            clientApplication.ClientApplicationTypes = new HashSet<string>
            {
                ClientApplicationType.BrowserBased.AsString(EnumFormat.EnumMemberValue),
                ClientApplicationType.Desktop.AsString(EnumFormat.EnumMemberValue),
                ClientApplicationType.MobileTablet.AsString(EnumFormat.EnumMemberValue),
            };

            var updatedClientApplication = service.RemoveClientApplicationType(clientApplication, ClientApplicationType.MobileTablet);

            updatedClientApplication.ClientApplicationTypes.Should().BeEquivalentTo(
                    new HashSet<string>
                    {
                        ClientApplicationType.BrowserBased.AsString(EnumFormat.EnumMemberValue),
                        ClientApplicationType.Desktop.AsString(EnumFormat.EnumMemberValue),
                    });

            // Browser Based
            updatedClientApplication.AdditionalInformation.Should().Equals(clientApplication.AdditionalInformation);
            updatedClientApplication.BrowsersSupported.Should().BeEquivalentTo(clientApplication.BrowsersSupported);
            updatedClientApplication.HardwareRequirements.Should().Equals(clientApplication.HardwareRequirements);
            updatedClientApplication.MinimumConnectionSpeed.Should().Equals(clientApplication.MinimumConnectionSpeed);
            updatedClientApplication.MinimumDesktopResolution.Should().Equals(clientApplication.MinimumDesktopResolution);
            updatedClientApplication.MobileFirstDesign.Should().Equals(clientApplication.MobileFirstDesign);
            updatedClientApplication.MobileResponsive.Should().Equals(clientApplication.MobileResponsive);
            updatedClientApplication.Plugins.Should().BeEquivalentTo(clientApplication.Plugins);

            // Desktop
            updatedClientApplication.NativeDesktopAdditionalInformation.Should().Equals(clientApplication.NativeDesktopAdditionalInformation);
            updatedClientApplication.NativeDesktopHardwareRequirements.Should().Equals(clientApplication.NativeDesktopHardwareRequirements);
            updatedClientApplication.NativeDesktopMemoryAndStorage.Should().BeEquivalentTo(clientApplication.NativeDesktopMemoryAndStorage);
            updatedClientApplication.NativeDesktopMinimumConnectionSpeed.Should().Equals(clientApplication.NativeDesktopMinimumConnectionSpeed);
            updatedClientApplication.NativeDesktopOperatingSystemsDescription.Should().Equals(clientApplication.NativeDesktopOperatingSystemsDescription);
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

            var updatedSolution = await context.CatalogueItems.SingleAsync(c => c.Id == solution.Id);

            updatedSolution.PublishedStatus.Should().Be(PublicationStatus.Published);
        }
    }
}
