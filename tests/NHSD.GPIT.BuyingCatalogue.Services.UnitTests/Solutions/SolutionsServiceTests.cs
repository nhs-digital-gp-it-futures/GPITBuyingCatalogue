using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.Services.Solutions;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Solutions
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class SolutionsServiceTests
    {
        [Test]
        public static void SaveSupplierContacts_ModelNull_ThrowsException()
        {
            var service = new SolutionsService(Mock.Of<ILogWrapper<SolutionsService>>(),
                Mock.Of<BuyingCatalogueDbContext>(), Mock.Of<IRepository<MarketingContact>>());

            var actual = Assert.ThrowsAsync<ArgumentNullException>(() => service.SaveSupplierContacts(default));

            actual.ParamName.Should().Be("model");
        }

        [Test]
        public static async Task SaveSupplierContacts_ModelValid_CallsSetSolutionIdOnModel()
        {
            var mockModel = new Mock<SupplierContactsModel>();
            var service = new SolutionsService(Mock.Of<ILogWrapper<SolutionsService>>(),
                Mock.Of<BuyingCatalogueDbContext>(), Mock.Of<IRepository<MarketingContact>>());

            await service.SaveSupplierContacts(mockModel.Object);
            
            mockModel.Verify(x => x.SetSolutionId());
        }

        [Test]
        public static async Task SaveSupplierContacts_Retrieves_ContactsForSolutionId()
        {
            var mockModel = new Mock<SupplierContactsModel>();
            var solutionId = "Some-Solution-Id";
            mockModel.SetupGet(x => x.SolutionId)
                .Returns(solutionId);
            var mockRepository = new Mock<IRepository<MarketingContact>>();
            mockRepository.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<MarketingContact, bool>>>()))
                .Callback((Expression<Func<MarketingContact, bool>> predicate) => predicate.Compile()
                    .Invoke(new MarketingContact {SolutionId = solutionId}).Should().BeTrue());

            var service = new SolutionsService(Mock.Of<ILogWrapper<SolutionsService>>(),
                Mock.Of<BuyingCatalogueDbContext>(), mockRepository.Object);

            await service.SaveSupplierContacts(mockModel.Object);
            
            mockRepository.Verify(x => x.GetAllAsync(It.IsAny<Expression<Func<MarketingContact, bool>>>()));
        }

        [Test]
        public static async Task SaveSupplierContacts_NoContactsInDatabase_AddsValidContactsToRepository()
        {
            var validContacts = new Mock<IList<MarketingContact>>().Object;
            var mockModel = new Mock<SupplierContactsModel>();
            mockModel.Setup(x => x.ValidContacts())
                .Returns(validContacts);
            
            var mockRepository = new Mock<IRepository<MarketingContact>>();
            mockRepository.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<MarketingContact, bool>>>()))
                .ReturnsAsync(Array.Empty<MarketingContact>());

            var service = new SolutionsService(Mock.Of<ILogWrapper<SolutionsService>>(),
                Mock.Of<BuyingCatalogueDbContext>(), mockRepository.Object);

            await service.SaveSupplierContacts(mockModel.Object);
            
            mockModel.Verify(x => x.ValidContacts());
            mockRepository.Verify(x => x.AddAll(validContacts));
        }

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
            
            var mockRepository = new Mock<IRepository<MarketingContact>>();
            mockRepository.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<MarketingContact, bool>>>()))
                .ReturnsAsync(savedModels.ToArray);

            var service = new SolutionsService(Mock.Of<ILogWrapper<SolutionsService>>(),
                Mock.Of<BuyingCatalogueDbContext>(), mockRepository.Object);

            await service.SaveSupplierContacts(mockModel.Object);
            
            mockNewContact.Verify(x => x.IsEmpty());
            mockRepository.Verify(x => x.Remove(It.Is<MarketingContact>(y => y.Id == savedModels[1].Id)));
        }

        [Test]
        public static async Task SaveSupplierContacts_ContactsInDatabase_UpdatesNonEmptyContacts()
        {
            var savedModel = new Mock<MarketingContact> {CallBase = true,};
            savedModel.Object.Id = 42;
            var savedModels = new [] {savedModel.Object,};
            
            var mockModel = new Mock<SupplierContactsModel>();
            var mockNewContact = new Mock<MarketingContact>();
            mockNewContact.Setup(x => x.IsEmpty())
                .Returns(false);
            mockModel.Setup(x => x.ContactFor(savedModels[0].Id))
                .Returns(mockNewContact.Object);
            
            var mockRepository = new Mock<IRepository<MarketingContact>>();
            mockRepository.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<MarketingContact, bool>>>()))
                .ReturnsAsync(savedModels);

            var service = new SolutionsService(Mock.Of<ILogWrapper<SolutionsService>>(),
                Mock.Of<BuyingCatalogueDbContext>(), mockRepository.Object);

            await service.SaveSupplierContacts(mockModel.Object);
            
            mockNewContact.Verify(x => x.IsEmpty());
            savedModel.Verify(x => x.UpdateFrom(mockNewContact.Object));
        }

        [Test]
        public static async Task SaveSupplierContacts_AddsNewAndValidContacts_ToRepository()
        {
            var mockModel = new Mock<SupplierContactsModel>();
            var newAndValidContacts = new Mock<IList<MarketingContact>>().Object;
            mockModel.Setup(x => x.NewAndValidContacts())
                .Returns(newAndValidContacts);
            
            var mockRepository = new Mock<IRepository<MarketingContact>>();
            mockRepository.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<MarketingContact, bool>>>()))
                .ReturnsAsync(new []{new MarketingContact()});
            var service = new SolutionsService(Mock.Of<ILogWrapper<SolutionsService>>(),
                Mock.Of<BuyingCatalogueDbContext>(), mockRepository.Object);

            await service.SaveSupplierContacts(mockModel.Object);
            
            mockModel.Verify(x => x.NewAndValidContacts());
            mockRepository.Verify(x => x.AddAll(newAndValidContacts));
        }

        [Test]
        public static async Task SaveSupplierContacts_CallsSaveChangesAsync_OnRepository()
        {
            var mockRepository = new Mock<IRepository<MarketingContact>>();
            var service = new SolutionsService(Mock.Of<ILogWrapper<SolutionsService>>(),
                Mock.Of<BuyingCatalogueDbContext>(), mockRepository.Object);

            await service.SaveSupplierContacts(Mock.Of<SupplierContactsModel>());
            
            mockRepository.Verify(x => x.SaveChangesAsync());
        }
    }
}
