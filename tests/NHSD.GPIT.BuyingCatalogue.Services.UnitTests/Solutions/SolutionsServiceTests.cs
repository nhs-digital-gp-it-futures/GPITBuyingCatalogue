using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using EnumsNET;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
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
        public static async Task SaveSupplierContacts_ModelNull_ThrowsException()
        {
            var service = new SolutionsService(
                Mock.Of<BuyingCatalogueDbContext>(),
                Mock.Of<IDbRepository<MarketingContact, BuyingCatalogueDbContext>>(),
                Mock.Of<IDbRepository<Solution, BuyingCatalogueDbContext>>(),
                Mock.Of<IDbRepository<Supplier, BuyingCatalogueDbContext>>(),
                Mock.Of<ICatalogueItemRepository>(),
                Mock.Of<IMemoryCache>());

            var actual = await Assert.ThrowsAsync<ArgumentNullException>(() => service.SaveSupplierContacts(default));

            actual.ParamName.Should().Be("model");
        }

        // TODO: fix
        [Fact(Skip = "Broken")]
        public static async Task SaveSupplierContacts_ModelValid_CallsSetSolutionIdOnModel()
        {
            var mockModel = new Mock<SupplierContactsModel>();
            var service = new SolutionsService(
                Mock.Of<BuyingCatalogueDbContext>(),
                Mock.Of<IDbRepository<MarketingContact, BuyingCatalogueDbContext>>(),
                Mock.Of<IDbRepository<Solution, BuyingCatalogueDbContext>>(),
                Mock.Of<IDbRepository<Supplier, BuyingCatalogueDbContext>>(),
                Mock.Of<ICatalogueItemRepository>(),
                Mock.Of<IMemoryCache>());

            await service.SaveSupplierContacts(mockModel.Object);

            mockModel.Verify(m => m.SetSolutionId());
        }

        [Theory]
        [CommonAutoData]
        public static async Task SaveSupplierContacts_Retrieves_ContactsForSolutionId(
            [Frozen] CatalogueItemId catalogueItemId,
            MarketingContact contact,
            SupplierContactsModel supplierContactsModel,
            [Frozen] Mock<IDbRepository<MarketingContact, BuyingCatalogueDbContext>> mockMarketingContactRepository,
            SolutionsService service)
        {
            // Parameter included to freeze value for marketing contact and supplier contacts model
            _ = catalogueItemId;

            void TestPredicate(Expression<Func<MarketingContact, bool>> expression)
            {
                var predicate = expression.Compile();
                var result = predicate(contact);

                result.Should().BeTrue();
            }

            mockMarketingContactRepository.Setup(r => r.GetAllAsync(It.IsAny<Expression<Func<MarketingContact, bool>>>()))
                .Callback<Expression<Func<MarketingContact, bool>>>(TestPredicate);

            await service.SaveSupplierContacts(supplierContactsModel);

            mockMarketingContactRepository.Verify(r => r.GetAllAsync(It.IsAny<Expression<Func<MarketingContact, bool>>>()));
        }

        [Theory]
        [CommonAutoData]
        public static async Task SaveSupplierContacts_NoContactsInDatabase_AddsValidContactsToRepository(
            [Frozen] MarketingContact[] validContacts,
            SupplierContactsModel supplierContactsModel,
            [Frozen] Mock<IDbRepository<MarketingContact, BuyingCatalogueDbContext>> mockMarketingContactRepository,
            SolutionsService service)
        {
            mockMarketingContactRepository.Setup(r => r.GetAllAsync(It.IsAny<Expression<Func<MarketingContact, bool>>>()))
                .ReturnsAsync(Array.Empty<MarketingContact>());

            await service.SaveSupplierContacts(supplierContactsModel);

            mockMarketingContactRepository.Verify(r => r.AddAll(validContacts));
        }

        [Theory]
        [CommonAutoData]
        public static async Task SaveSupplierContacts_ContactsInDatabase_RemovesEmptyContactsFromDatabase(
            MarketingContact[] savedModels,
            SupplierContactsModel supplierContactsModel,
            [Frozen] Mock<IDbRepository<MarketingContact, BuyingCatalogueDbContext>> mockMarketingContactRepository,
            SolutionsService service)
        {
            supplierContactsModel.Contacts = new[]
            {
                new MarketingContact
                {
                    Id = savedModels[0].Id,
                    Department = "Department",
                },
                new MarketingContact
                {
                    Id = savedModels[1].Id,
                },
            };

            mockMarketingContactRepository.Setup(r => r.GetAllAsync(It.IsAny<Expression<Func<MarketingContact, bool>>>()))
                .ReturnsAsync(savedModels);

            await service.SaveSupplierContacts(supplierContactsModel);

            mockMarketingContactRepository.Verify(r => r.Remove(It.Is<MarketingContact>(c => c.Id == savedModels[1].Id)));
        }

        // TODO: fix
        [Fact(Skip = "Broken")]
        public static async Task SaveSupplierContacts_ContactsInDatabase_UpdatesNonEmptyContacts()
        {
            var savedModel = new Mock<MarketingContact> { CallBase = true, };
            savedModel.Object.Id = 42;
            var savedModels = new[] { savedModel.Object, };

            var mockModel = new Mock<SupplierContactsModel>();
            var mockNewContact = new Mock<MarketingContact>();
            mockNewContact.Setup(c => c.IsEmpty())
                .Returns(false);
            mockModel.Setup(m => m.ContactFor(savedModels[0].Id))
                .Returns(mockNewContact.Object);

            var mockMarketingContactRepository = new Mock<IDbRepository<MarketingContact, BuyingCatalogueDbContext>>();
            mockMarketingContactRepository.Setup(r => r.GetAllAsync(It.IsAny<Expression<Func<MarketingContact, bool>>>()))
                .ReturnsAsync(savedModels);

            var service = new SolutionsService(
                Mock.Of<BuyingCatalogueDbContext>(),
                mockMarketingContactRepository.Object,
                Mock.Of<IDbRepository<Solution, BuyingCatalogueDbContext>>(),
                Mock.Of<IDbRepository<Supplier, BuyingCatalogueDbContext>>(),
                Mock.Of<ICatalogueItemRepository>(),
                Mock.Of<IMemoryCache>());

            await service.SaveSupplierContacts(mockModel.Object);

            mockNewContact.Verify(c => c.IsEmpty());
            savedModel.Verify(r => r.UpdateFrom(mockNewContact.Object));
        }

        // TODO: fix
        [Fact(Skip = "Broken")]
        public static async Task SaveSupplierContacts_AddsNewAndValidContacts_ToRepository()
        {
            var mockModel = new Mock<SupplierContactsModel>();
            var newAndValidContacts = new Mock<IList<MarketingContact>>().Object;
            mockModel.Setup(m => m.NewAndValidContacts())
                .Returns(newAndValidContacts);

            var mockMarketingContactRepository = new Mock<IDbRepository<MarketingContact, BuyingCatalogueDbContext>>();
            mockMarketingContactRepository.Setup(r => r.GetAllAsync(It.IsAny<Expression<Func<MarketingContact, bool>>>()))
                .ReturnsAsync(new[] { new MarketingContact() });

            var service = new SolutionsService(
                Mock.Of<BuyingCatalogueDbContext>(),
                mockMarketingContactRepository.Object,
                Mock.Of<IDbRepository<Solution, BuyingCatalogueDbContext>>(),
                Mock.Of<IDbRepository<Supplier, BuyingCatalogueDbContext>>(),
                Mock.Of<ICatalogueItemRepository>(),
                Mock.Of<IMemoryCache>());

            await service.SaveSupplierContacts(mockModel.Object);

            mockModel.Verify(m => m.NewAndValidContacts());
            mockMarketingContactRepository.Verify(r => r.AddAll(newAndValidContacts));
        }

        [Theory]
        [CommonAutoData]
        public static async Task SaveSupplierContacts_CallsSaveChangesAsync_OnRepository(
            SupplierContactsModel supplierContactsModel,
            [Frozen] Mock<IDbRepository<MarketingContact, BuyingCatalogueDbContext>> mockMarketingContactRepository,
            SolutionsService service)
        {
            await service.SaveSupplierContacts(supplierContactsModel);

            mockMarketingContactRepository.Verify(r => r.SaveChangesAsync());
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
                Mock.Of<ICatalogueItemRepository>(),
                Mock.Of<IMemoryCache>());

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
                Mock.Of<ICatalogueItemRepository>(),
                Mock.Of<IMemoryCache>());

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
                Mock.Of<ICatalogueItemRepository>(),
                Mock.Of<IMemoryCache>());

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
                Mock.Of<ICatalogueItemRepository>(),
                Mock.Of<IMemoryCache>());

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
                Mock.Of<ICatalogueItemRepository>(),
                Mock.Of<IMemoryCache>());

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
                Mock.Of<ICatalogueItemRepository>(),
                Mock.Of<IMemoryCache>());

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
                Mock.Of<ICatalogueItemRepository>(),
                Mock.Of<IMemoryCache>());

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
                Mock.Of<ICatalogueItemRepository>(),
                Mock.Of<IMemoryCache>());

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
                Mock.Of<ICatalogueItemRepository>(),
                Mock.Of<IMemoryCache>());

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
                Mock.Of<ICatalogueItemRepository>(),
                Mock.Of<IMemoryCache>());

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
                Mock.Of<ICatalogueItemRepository>(),
                Mock.Of<IMemoryCache>());

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
                Mock.Of<ICatalogueItemRepository>(),
                Mock.Of<IMemoryCache>());

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
                mockCatalogueItemRepository.Object,
                Mock.Of<IMemoryCache>());

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
                mockCatalogueItemRepository.Object,
                Mock.Of<IMemoryCache>());

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
                mockCatalogueItemRepository.Object,
                Mock.Of<IMemoryCache>());

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
    }
}
