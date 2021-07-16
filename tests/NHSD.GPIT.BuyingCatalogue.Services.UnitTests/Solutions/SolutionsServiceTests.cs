using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Bogus;
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
                Mock.Of<ICatalogueItemRepository>());

            await service.SaveSupplierContacts(mockModel.Object);

            mockModel.Verify(x => x.SetSolutionId());
        }

        // TODO: fix
        [Fact(Skip = "Broken")]
        public static async Task SaveSupplierContacts_Retrieves_ContactsForSolutionId()
        {
            var mockModel = new Mock<SupplierContactsModel>();
            var solutionId = new CatalogueItemId(1, "123");
            mockModel.SetupGet(x => x.SolutionId)
                .Returns(solutionId);

            var mockMarketingContactRepository = new Mock<IDbRepository<MarketingContact, BuyingCatalogueDbContext>>();
            mockMarketingContactRepository.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<MarketingContact, bool>>>()))
                .Callback((Expression<Func<MarketingContact, bool>> predicate) => predicate.Compile()(new MarketingContact { SolutionId = solutionId }).Should().BeTrue());

            var service = new SolutionsService(
                Mock.Of<BuyingCatalogueDbContext>(),
                mockMarketingContactRepository.Object,
                Mock.Of<IDbRepository<Solution, BuyingCatalogueDbContext>>(),
                Mock.Of<IDbRepository<Supplier, BuyingCatalogueDbContext>>(),
                Mock.Of<ICatalogueItemRepository>());

            await service.SaveSupplierContacts(mockModel.Object);

            mockMarketingContactRepository.Verify(x => x.GetAllAsync(It.IsAny<Expression<Func<MarketingContact, bool>>>()));
        }

        // TODO: fix
        [Fact(Skip = "Broken")]
        public static async Task SaveSupplierContacts_NoContactsInDatabase_AddsValidContactsToRepository()
        {
            var validContacts = new Mock<IList<MarketingContact>>().Object;
            var mockModel = new Mock<SupplierContactsModel>();
            mockModel.Setup(x => x.ValidContacts())
                .Returns(validContacts);

            var mockMarketingContactRepository = new Mock<IDbRepository<MarketingContact, BuyingCatalogueDbContext>>();
            mockMarketingContactRepository.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<MarketingContact, bool>>>()))
                .ReturnsAsync(Array.Empty<MarketingContact>());

            var service = new SolutionsService(
                Mock.Of<BuyingCatalogueDbContext>(),
                mockMarketingContactRepository.Object,
                Mock.Of<IDbRepository<Solution, BuyingCatalogueDbContext>>(),
                Mock.Of<IDbRepository<Supplier, BuyingCatalogueDbContext>>(),
                Mock.Of<ICatalogueItemRepository>());

            await service.SaveSupplierContacts(mockModel.Object);

            mockModel.Verify(x => x.ValidContacts());
            mockMarketingContactRepository.Verify(x => x.AddAll(validContacts));
        }

        // TODO: fix
        [Fact(Skip = "Broken")]
        public static async Task SaveSupplierContacts_ContactsInDatabase_RemovesEmptyContactsFromDatabase()
        {
            var savedModels = new Faker<MarketingContact>()
                .RuleFor(x => x.Id, f => f.Random.Int(100, 500))
                .Generate(2);

            var mockModel = new Mock<SupplierContactsModel>();
            var mockNewContact = new Mock<MarketingContact>();
            mockNewContact.Setup(x => x.IsEmpty())
                .Returns(true);
            mockModel.Setup(x => x.ContactFor(savedModels[1].Id))
                .Returns(mockNewContact.Object);

            var mockMarketingContactRepository = new Mock<IDbRepository<MarketingContact, BuyingCatalogueDbContext>>();
            mockMarketingContactRepository.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<MarketingContact, bool>>>()))
                .ReturnsAsync(savedModels.ToArray);

            var service = new SolutionsService(
                Mock.Of<BuyingCatalogueDbContext>(),
                mockMarketingContactRepository.Object,
                Mock.Of<IDbRepository<Solution, BuyingCatalogueDbContext>>(),
                Mock.Of<IDbRepository<Supplier, BuyingCatalogueDbContext>>(),
                Mock.Of<ICatalogueItemRepository>());

            await service.SaveSupplierContacts(mockModel.Object);

            mockNewContact.Verify(x => x.IsEmpty());
            mockMarketingContactRepository.Verify(x => x.Remove(It.Is<MarketingContact>(y => y.Id == savedModels[1].Id)));
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
            mockNewContact.Setup(x => x.IsEmpty())
                .Returns(false);
            mockModel.Setup(x => x.ContactFor(savedModels[0].Id))
                .Returns(mockNewContact.Object);

            var mockMarketingContactRepository = new Mock<IDbRepository<MarketingContact, BuyingCatalogueDbContext>>();
            mockMarketingContactRepository.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<MarketingContact, bool>>>()))
                .ReturnsAsync(savedModels);

            var service = new SolutionsService(
                Mock.Of<BuyingCatalogueDbContext>(),
                mockMarketingContactRepository.Object,
                Mock.Of<IDbRepository<Solution, BuyingCatalogueDbContext>>(),
                Mock.Of<IDbRepository<Supplier, BuyingCatalogueDbContext>>(),
                Mock.Of<ICatalogueItemRepository>());

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
            mockModel.Setup(x => x.NewAndValidContacts())
                .Returns(newAndValidContacts);

            var mockMarketingContactRepository = new Mock<IDbRepository<MarketingContact, BuyingCatalogueDbContext>>();
            mockMarketingContactRepository.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<MarketingContact, bool>>>()))
                .ReturnsAsync(new[] { new MarketingContact() });

            var service = new SolutionsService(
                Mock.Of<BuyingCatalogueDbContext>(),
                mockMarketingContactRepository.Object,
                Mock.Of<IDbRepository<Solution, BuyingCatalogueDbContext>>(),
                Mock.Of<IDbRepository<Supplier, BuyingCatalogueDbContext>>(),
                Mock.Of<ICatalogueItemRepository>());

            await service.SaveSupplierContacts(mockModel.Object);

            mockModel.Verify(x => x.NewAndValidContacts());
            mockMarketingContactRepository.Verify(r => r.AddAll(newAndValidContacts));
        }

        // TODO: fix
        [Fact(Skip = "Broken")]
        public static async Task SaveSupplierContacts_CallsSaveChangesAsync_OnRepository()
        {
            var mockMarketingContactRepository = new Mock<IDbRepository<MarketingContact, BuyingCatalogueDbContext>>();

            var service = new SolutionsService(
                Mock.Of<BuyingCatalogueDbContext>(),
                mockMarketingContactRepository.Object,
                Mock.Of<IDbRepository<Solution, BuyingCatalogueDbContext>>(),
                Mock.Of<IDbRepository<Supplier, BuyingCatalogueDbContext>>(),
                Mock.Of<ICatalogueItemRepository>());

            await service.SaveSupplierContacts(Mock.Of<SupplierContactsModel>());

            mockMarketingContactRepository.Verify(r => r.SaveChangesAsync());
        }

        [Fact]
        public static async Task SaveIntegrationLink_CallsSaveChangesAsync_OnRepository()
        {
            var mockSolutionRepository = new Mock<IDbRepository<Solution, BuyingCatalogueDbContext>>();
            mockSolutionRepository.Setup(x => x.SingleAsync(It.IsAny<Expression<Func<Solution, bool>>>()))
                .ReturnsAsync(new Solution());

            var service = new SolutionsService(
                Mock.Of<BuyingCatalogueDbContext>(),
                Mock.Of<IDbRepository<MarketingContact, BuyingCatalogueDbContext>>(),
                mockSolutionRepository.Object,
                Mock.Of<IDbRepository<Supplier, BuyingCatalogueDbContext>>(),
                Mock.Of<ICatalogueItemRepository>());

            await service.SaveIntegrationLink(new CatalogueItemId(100000, "001"), "A link");

            mockSolutionRepository.Verify(r => r.SaveChangesAsync());
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
            mockSolutionRepository.Setup(x => x.SingleAsync(It.IsAny<Expression<Func<Solution, bool>>>()))
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
            mockSolutionRepository.Setup(x => x.SingleAsync(It.IsAny<Expression<Func<Solution, bool>>>()))
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
            mockSolutionRepository.Setup(x => x.SingleAsync(It.IsAny<Expression<Func<Solution, bool>>>()))
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
            mockSolutionRepository.Setup(x => x.SingleAsync(It.IsAny<Expression<Func<Solution, bool>>>()))
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
            mockSolutionRepository.Setup(x => x.SingleAsync(It.IsAny<Expression<Func<Solution, bool>>>()))
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
            mockSolutionRepository.Setup(x => x.SingleAsync(It.IsAny<Expression<Func<Solution, bool>>>()))
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

        [Theory]
        [MemberData(nameof(InvalidStringData.TestData), MemberType = typeof(InvalidStringData))]
        public static async Task GetSupplier_InvalidSupplierId_ThrowsException(string supplierId)
        {
            var service = new SolutionsService(
                Mock.Of<BuyingCatalogueDbContext>(),
                Mock.Of<IDbRepository<MarketingContact, BuyingCatalogueDbContext>>(),
                Mock.Of<IDbRepository<Solution, BuyingCatalogueDbContext>>(),
                Mock.Of<IDbRepository<Supplier, BuyingCatalogueDbContext>>(),
                Mock.Of<ICatalogueItemRepository>());

            var actual = await Assert.ThrowsAsync<ArgumentException>(() => service.GetSupplier(supplierId));

            actual.ParamName.Should().Be("supplierId");
        }

        [Fact]
        public static async Task SaveSupplier_CallsSaveChangesAsync_OnRepository()
        {
            var mockSupplierRepository = new Mock<IDbRepository<Supplier, BuyingCatalogueDbContext>>();
            mockSupplierRepository.Setup(x => x.SingleAsync(It.IsAny<Expression<Func<Supplier, bool>>>()))
                .ReturnsAsync(new Supplier());

            var service = new SolutionsService(
                Mock.Of<BuyingCatalogueDbContext>(),
                Mock.Of<IDbRepository<MarketingContact, BuyingCatalogueDbContext>>(),
                Mock.Of<IDbRepository<Solution, BuyingCatalogueDbContext>>(),
                mockSupplierRepository.Object,
                Mock.Of<ICatalogueItemRepository>());

            await service.SaveSupplierDescriptionAndLink("100000-001", "Description", "Link");

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
                .ReturnsAsync(new CatalogueItemId(int.Parse(model.SupplierId), "045"));

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
            var catalogueItemId = new CatalogueItemId(int.Parse(model.SupplierId), "045");
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
                            c.CatalogueItemId == catalogueItemId.NextSolutionId() &&
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
            string supplierId,
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
    }
}
