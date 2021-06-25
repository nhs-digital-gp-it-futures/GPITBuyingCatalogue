using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.Services.Solutions;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Solutions
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class SolutionsServiceTests
    {
        private static readonly string[] InvalidStrings = { null, string.Empty, "    " };

        [Test]
        public static void SaveSupplierContacts_ModelNull_ThrowsException()
        {
            var service = new SolutionsService(
                Mock.Of<GPITBuyingCatalogueDbContext>(), Mock.Of<IDbRepository<MarketingContact, GPITBuyingCatalogueDbContext>>(),
                Mock.Of<IDbRepository<Solution, GPITBuyingCatalogueDbContext>>(), Mock.Of<IDbRepository<Supplier, GPITBuyingCatalogueDbContext>>());

            var actual = Assert.ThrowsAsync<ArgumentNullException>(() => service.SaveSupplierContacts(default));

            actual.ParamName.Should().Be("model");
        }

        // TODO: fix
        [Ignore("Broken")]
        [Test]
        public static async Task SaveSupplierContacts_ModelValid_CallsSetSolutionIdOnModel()
        {
            var mockModel = new Mock<SupplierContactsModel>();
            var service = new SolutionsService(
                Mock.Of<GPITBuyingCatalogueDbContext>(), Mock.Of<IDbRepository<MarketingContact, GPITBuyingCatalogueDbContext>>(),
                Mock.Of<IDbRepository<Solution, GPITBuyingCatalogueDbContext>>(), Mock.Of<IDbRepository<Supplier, GPITBuyingCatalogueDbContext>>());

            await service.SaveSupplierContacts(mockModel.Object);

            mockModel.Verify(x => x.SetSolutionId());
        }

        // TODO: fix
        [Ignore("Broken")]
        [Test]
        public static async Task SaveSupplierContacts_Retrieves_ContactsForSolutionId()
        {
            var mockModel = new Mock<SupplierContactsModel>();
            var solutionId = new CatalogueItemId(1, "123");
            mockModel.SetupGet(x => x.SolutionId)
                .Returns(solutionId);

            var mockMarketingContactRepository = new Mock<IDbRepository<MarketingContact, GPITBuyingCatalogueDbContext>>();
            mockMarketingContactRepository.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<MarketingContact, bool>>>()))
                .Callback((Expression<Func<MarketingContact, bool>> predicate) => predicate.Compile()(new MarketingContact { SolutionId = solutionId }).Should().BeTrue());

            var service = new SolutionsService(
                Mock.Of<GPITBuyingCatalogueDbContext>(), mockMarketingContactRepository.Object,
                Mock.Of<IDbRepository<Solution, GPITBuyingCatalogueDbContext>>(), Mock.Of<IDbRepository<Supplier, GPITBuyingCatalogueDbContext>>());

            await service.SaveSupplierContacts(mockModel.Object);

            mockMarketingContactRepository.Verify(x => x.GetAllAsync(It.IsAny<Expression<Func<MarketingContact, bool>>>()));
        }

        // TODO: fix
        [Ignore("Broken")]
        [Test]
        public static async Task SaveSupplierContacts_NoContactsInDatabase_AddsValidContactsToRepository()
        {
            var validContacts = new Mock<IList<MarketingContact>>().Object;
            var mockModel = new Mock<SupplierContactsModel>();
            mockModel.Setup(x => x.ValidContacts())
                .Returns(validContacts);

            var mockMarketingContactRepository = new Mock<IDbRepository<MarketingContact, GPITBuyingCatalogueDbContext>>();
            mockMarketingContactRepository.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<MarketingContact, bool>>>()))
                .ReturnsAsync(Array.Empty<MarketingContact>());

            var service = new SolutionsService(
                Mock.Of<GPITBuyingCatalogueDbContext>(), mockMarketingContactRepository.Object,
                Mock.Of<IDbRepository<Solution, GPITBuyingCatalogueDbContext>>(), Mock.Of<IDbRepository<Supplier, GPITBuyingCatalogueDbContext>>());

            await service.SaveSupplierContacts(mockModel.Object);

            mockModel.Verify(x => x.ValidContacts());
            mockMarketingContactRepository.Verify(x => x.AddAll(validContacts));
        }

        // TODO: fix
        [Ignore("Broken")]
        [Test]
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

            var mockMarketingContactRepository = new Mock<IDbRepository<MarketingContact, GPITBuyingCatalogueDbContext>>();
            mockMarketingContactRepository.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<MarketingContact, bool>>>()))
                .ReturnsAsync(savedModels.ToArray);

            var service = new SolutionsService(
                Mock.Of<GPITBuyingCatalogueDbContext>(), mockMarketingContactRepository.Object,
                Mock.Of<IDbRepository<Solution, GPITBuyingCatalogueDbContext>>(), Mock.Of<IDbRepository<Supplier, GPITBuyingCatalogueDbContext>>());

            await service.SaveSupplierContacts(mockModel.Object);

            mockNewContact.Verify(x => x.IsEmpty());
            mockMarketingContactRepository.Verify(x => x.Remove(It.Is<MarketingContact>(y => y.Id == savedModels[1].Id)));
        }

        // TODO: fix
        [Ignore("Broken")]
        [Test]
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

            var mockMarketingContactRepository = new Mock<IDbRepository<MarketingContact, GPITBuyingCatalogueDbContext>>();
            mockMarketingContactRepository.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<MarketingContact, bool>>>()))
                .ReturnsAsync(savedModels);

            var service = new SolutionsService(
                Mock.Of<GPITBuyingCatalogueDbContext>(), mockMarketingContactRepository.Object,
                Mock.Of<IDbRepository<Solution, GPITBuyingCatalogueDbContext>>(), Mock.Of<IDbRepository<Supplier, GPITBuyingCatalogueDbContext>>());

            await service.SaveSupplierContacts(mockModel.Object);

            mockNewContact.Verify(c => c.IsEmpty());
            savedModel.Verify(r => r.UpdateFrom(mockNewContact.Object));
        }

        // TODO: fix
        [Ignore("Broken")]
        [Test]
        public static async Task SaveSupplierContacts_AddsNewAndValidContacts_ToRepository()
        {
            var mockModel = new Mock<SupplierContactsModel>();
            var newAndValidContacts = new Mock<IList<MarketingContact>>().Object;
            mockModel.Setup(x => x.NewAndValidContacts())
                .Returns(newAndValidContacts);

            var mockMarketingContactRepository = new Mock<IDbRepository<MarketingContact, GPITBuyingCatalogueDbContext>>();
            mockMarketingContactRepository.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<MarketingContact, bool>>>()))
                .ReturnsAsync(new[] { new MarketingContact() });

            var service = new SolutionsService(
                Mock.Of<GPITBuyingCatalogueDbContext>(), mockMarketingContactRepository.Object,
                Mock.Of<IDbRepository<Solution, GPITBuyingCatalogueDbContext>>(), Mock.Of<IDbRepository<Supplier, GPITBuyingCatalogueDbContext>>());

            await service.SaveSupplierContacts(mockModel.Object);

            mockModel.Verify(x => x.NewAndValidContacts());
            mockMarketingContactRepository.Verify(r => r.AddAll(newAndValidContacts));
        }

        // TODO: fix
        [Ignore("Broken")]
        [Test]
        public static async Task SaveSupplierContacts_CallsSaveChangesAsync_OnRepository()
        {
            var mockMarketingContactRepository = new Mock<IDbRepository<MarketingContact, GPITBuyingCatalogueDbContext>>();

            var service = new SolutionsService(
                Mock.Of<GPITBuyingCatalogueDbContext>(), mockMarketingContactRepository.Object,
                Mock.Of<IDbRepository<Solution, GPITBuyingCatalogueDbContext>>(), Mock.Of<IDbRepository<Supplier, GPITBuyingCatalogueDbContext>>());

            await service.SaveSupplierContacts(Mock.Of<SupplierContactsModel>());

            mockMarketingContactRepository.Verify(r => r.SaveChangesAsync());
        }

        [Test]
        public static async Task SaveIntegrationLink_CallsSaveChangesAsync_OnRepository()
        {
            var mockSolutionRepository = new Mock<IDbRepository<Solution, GPITBuyingCatalogueDbContext>>();
            mockSolutionRepository.Setup(x => x.SingleAsync(It.IsAny<Expression<Func<Solution, bool>>>()))
                .ReturnsAsync(new Solution());

            var service = new SolutionsService(
                Mock.Of<GPITBuyingCatalogueDbContext>(), Mock.Of<IDbRepository<MarketingContact, GPITBuyingCatalogueDbContext>>(),
                mockSolutionRepository.Object, Mock.Of<IDbRepository<Supplier, GPITBuyingCatalogueDbContext>>());

            await service.SaveIntegrationLink(new CatalogueItemId(100000, "001"), "A link");

            mockSolutionRepository.Verify(r => r.SaveChangesAsync());
        }

        [Test]
        [TestCaseSource(nameof(InvalidStrings))]
        public static void SaveSolutionDescription_InvalidSummary_ThrowsException(string summary)
        {
            var service = new SolutionsService(
                Mock.Of<GPITBuyingCatalogueDbContext>(), Mock.Of<IDbRepository<MarketingContact, GPITBuyingCatalogueDbContext>>(),
                Mock.Of<IDbRepository<Solution, GPITBuyingCatalogueDbContext>>(), Mock.Of<IDbRepository<Supplier, GPITBuyingCatalogueDbContext>>());

            var actual = Assert.ThrowsAsync<ArgumentException>(() => service.SaveSolutionDescription(new CatalogueItemId(100000, "001"), summary, "Description", "Link"));

            actual.ParamName.Should().Be("summary");
        }

        [Test]
        public static async Task SaveSolutionDescription_CallsSaveChangesAsync_OnRepository()
        {
            var mockSolutionRepository = new Mock<IDbRepository<Solution, GPITBuyingCatalogueDbContext>>();
            mockSolutionRepository.Setup(x => x.SingleAsync(It.IsAny<Expression<Func<Solution, bool>>>()))
                .ReturnsAsync(new Solution());

            var service = new SolutionsService(
                Mock.Of<GPITBuyingCatalogueDbContext>(), Mock.Of<IDbRepository<MarketingContact, GPITBuyingCatalogueDbContext>>(),
                mockSolutionRepository.Object, Mock.Of<IDbRepository<Supplier, GPITBuyingCatalogueDbContext>>());

            await service.SaveSolutionDescription(new CatalogueItemId(100000, "001"), "Summary", "Description", "Link");

            mockSolutionRepository.Verify(r => r.SaveChangesAsync());
        }

        [Test]
        public static async Task SaveSolutionFeatures_CallsSaveChangesAsync_OnRepository()
        {
            var mockSolutionRepository = new Mock<IDbRepository<Solution, GPITBuyingCatalogueDbContext>>();
            mockSolutionRepository.Setup(x => x.SingleAsync(It.IsAny<Expression<Func<Solution, bool>>>()))
                .ReturnsAsync(new Solution());

            var service = new SolutionsService(
                Mock.Of<GPITBuyingCatalogueDbContext>(), Mock.Of<IDbRepository<MarketingContact, GPITBuyingCatalogueDbContext>>(),
                mockSolutionRepository.Object, Mock.Of<IDbRepository<Supplier, GPITBuyingCatalogueDbContext>>());

            await service.SaveSolutionFeatures(new CatalogueItemId(100000, "001"), Array.Empty<string>());

            mockSolutionRepository.Verify(r => r.SaveChangesAsync());
        }

        [Test]
        public static async Task SaveImplementationDetail_CallsSaveChangesAsync_OnRepository()
        {
            var mockSolutionRepository = new Mock<IDbRepository<Solution, GPITBuyingCatalogueDbContext>>();
            mockSolutionRepository.Setup(x => x.SingleAsync(It.IsAny<Expression<Func<Solution, bool>>>()))
                .ReturnsAsync(new Solution());

            var service = new SolutionsService(
                Mock.Of<GPITBuyingCatalogueDbContext>(), Mock.Of<IDbRepository<MarketingContact, GPITBuyingCatalogueDbContext>>(),
                mockSolutionRepository.Object, Mock.Of<IDbRepository<Supplier, GPITBuyingCatalogueDbContext>>());

            await service.SaveImplementationDetail(new CatalogueItemId(100000, "001"), "123");

            mockSolutionRepository.Verify(r => r.SaveChangesAsync());
        }

        [Test]
        public static async Task SaveRoadMap_CallsSaveChangesAsync_OnRepository()
        {
            var mockSolutionRepository = new Mock<IDbRepository<Solution, GPITBuyingCatalogueDbContext>>();
            mockSolutionRepository.Setup(x => x.SingleAsync(It.IsAny<Expression<Func<Solution, bool>>>()))
                .ReturnsAsync(new Solution());

            var service = new SolutionsService(
                Mock.Of<GPITBuyingCatalogueDbContext>(), Mock.Of<IDbRepository<MarketingContact, GPITBuyingCatalogueDbContext>>(),
                mockSolutionRepository.Object, Mock.Of<IDbRepository<Supplier, GPITBuyingCatalogueDbContext>>());

            await service.SaveRoadMap(new CatalogueItemId(100000, "001"), "123");

            mockSolutionRepository.Verify(r => r.SaveChangesAsync());
        }

        [Test]
        public static void SaveClientApplication_InvalidModel_ThrowsException()
        {
            var service = new SolutionsService(
                Mock.Of<GPITBuyingCatalogueDbContext>(), Mock.Of<IDbRepository<MarketingContact, GPITBuyingCatalogueDbContext>>(),
                Mock.Of<IDbRepository<Solution, GPITBuyingCatalogueDbContext>>(), Mock.Of<IDbRepository<Supplier, GPITBuyingCatalogueDbContext>>());

            var actual = Assert.ThrowsAsync<ArgumentNullException>(() => service.SaveClientApplication(new CatalogueItemId(100000, "001"), null));

            actual.ParamName.Should().Be("clientApplication");
        }

        [Test]
        public static async Task SaveClientApplication_CallsSaveChangesAsync_OnRepository()
        {
            var mockSolutionRepository = new Mock<IDbRepository<Solution, GPITBuyingCatalogueDbContext>>();
            mockSolutionRepository.Setup(x => x.SingleAsync(It.IsAny<Expression<Func<Solution, bool>>>()))
                .ReturnsAsync(new Solution());

            var service = new SolutionsService(
                Mock.Of<GPITBuyingCatalogueDbContext>(), Mock.Of<IDbRepository<MarketingContact, GPITBuyingCatalogueDbContext>>(),
                mockSolutionRepository.Object, Mock.Of<IDbRepository<Supplier, GPITBuyingCatalogueDbContext>>());

            await service.SaveClientApplication(new CatalogueItemId(100000, "001"), new ClientApplication());

            mockSolutionRepository.Verify(r => r.SaveChangesAsync());
        }

        [Test]
        public static void SaveHosting_InvalidModel_ThrowsException()
        {
            var service = new SolutionsService(
                Mock.Of<GPITBuyingCatalogueDbContext>(), Mock.Of<IDbRepository<MarketingContact, GPITBuyingCatalogueDbContext>>(),
                Mock.Of<IDbRepository<Solution, GPITBuyingCatalogueDbContext>>(), Mock.Of<IDbRepository<Supplier, GPITBuyingCatalogueDbContext>>());

            var actual = Assert.ThrowsAsync<ArgumentNullException>(() => service.SaveHosting(new CatalogueItemId(100000, "001"), null));

            actual.ParamName.Should().Be("hosting");
        }

        [Test]
        public static async Task SaveHosting_CallsSaveChangesAsync_OnRepository()
        {
            var mockSolutionRepository = new Mock<IDbRepository<Solution, GPITBuyingCatalogueDbContext>>();
            mockSolutionRepository.Setup(x => x.SingleAsync(It.IsAny<Expression<Func<Solution, bool>>>()))
                .ReturnsAsync(new Solution());

            var service = new SolutionsService(
                Mock.Of<GPITBuyingCatalogueDbContext>(), Mock.Of<IDbRepository<MarketingContact, GPITBuyingCatalogueDbContext>>(),
                mockSolutionRepository.Object, Mock.Of<IDbRepository<Supplier, GPITBuyingCatalogueDbContext>>());

            await service.SaveHosting(new CatalogueItemId(100000, "001"), new Hosting());

            mockSolutionRepository.Verify(r => r.SaveChangesAsync());
        }

        [Test]
        [TestCaseSource(nameof(InvalidStrings))]
        public static void GetSupplier_InvalidSupplierId_ThrowsException(string supplierId)
        {
            var service = new SolutionsService(
                Mock.Of<GPITBuyingCatalogueDbContext>(), Mock.Of<IDbRepository<MarketingContact, GPITBuyingCatalogueDbContext>>(),
                Mock.Of<IDbRepository<Solution, GPITBuyingCatalogueDbContext>>(), Mock.Of<IDbRepository<Supplier, GPITBuyingCatalogueDbContext>>());

            var actual = Assert.ThrowsAsync<ArgumentException>(() => service.GetSupplier(supplierId));

            actual.ParamName.Should().Be("supplierId");
        }

        [Test]
        public static async Task SaveSupplier_CallsSaveChangesAsync_OnRepository()
        {
            var mockSupplierRepository = new Mock<IDbRepository<Supplier, GPITBuyingCatalogueDbContext>>();
            mockSupplierRepository.Setup(x => x.SingleAsync(It.IsAny<Expression<Func<Supplier, bool>>>()))
                .ReturnsAsync(new Supplier());

            var service = new SolutionsService(
                Mock.Of<GPITBuyingCatalogueDbContext>(), Mock.Of<IDbRepository<MarketingContact, GPITBuyingCatalogueDbContext>>(),
                Mock.Of<IDbRepository<Solution, GPITBuyingCatalogueDbContext>>(), mockSupplierRepository.Object);

            await service.SaveSupplierDescriptionAndLink("100000-001", "Description", "Link");

            mockSupplierRepository.Verify(r => r.SaveChangesAsync());
        }
    }
}
