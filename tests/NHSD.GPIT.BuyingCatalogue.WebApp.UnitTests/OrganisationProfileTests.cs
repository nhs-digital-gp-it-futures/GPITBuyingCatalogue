using AutoFixture.NUnit3;
using AutoMapper;
using FluentAssertions;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.AboutOrganisation;
using NHSD.GPIT.BuyingCatalogue.WebApp.MappingProfiles;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class OrganisationProfileTests
    {
        [Test]
        public static void Mappings_Configuration_Valid()
        {
            var mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile<OrganisationProfile>());
            
            mapperConfiguration.AssertConfigurationIsValid();
        }

        [Test, IgnoreCircularReferenceAutoData]
        public static void Map_CatalogueItemToAboutSupplierModel_ResultAsExpected(CatalogueItem catalogueItem,
            ClientApplication clientApplication)
        {
            catalogueItem.Solution.ClientApplication = JsonConvert.SerializeObject(clientApplication);
            var mapper = new MapperConfiguration(cfg => cfg.AddProfile<OrganisationProfile>())
                .CreateMapper();

            var actual = mapper.Map<CatalogueItem, AboutSupplierModel>(catalogueItem);

            actual.BackLink.Should().Be($"/marketing/supplier/solution/{catalogueItem.CatalogueItemId}");
            actual.BackLinkText.Should().Be("Return to all sections");
            actual.ClientApplication.Should().BeEquivalentTo(clientApplication);
            actual.Description.Should().Be(catalogueItem.Supplier.Summary);
            actual.Link.Should().Be(catalogueItem.Supplier.SupplierUrl);
            actual.SolutionId.Should().Be(catalogueItem.CatalogueItemId);
            actual.SupplierId.Should().Be(catalogueItem.Supplier.Id);
        }

        [Test, IgnoreCircularReferenceAutoData]
        public static void Map_CatalogueItemToContactDetailsModel_ResultAsExpected(CatalogueItem catalogueItem,
            ClientApplication clientApplication)
        {
            catalogueItem.Solution.ClientApplication = JsonConvert.SerializeObject(clientApplication);
            var mapper = new MapperConfiguration(cfg => cfg.AddProfile<OrganisationProfile>())
                .CreateMapper();

            var actual = mapper.Map<CatalogueItem, ContactDetailsModel>(catalogueItem);

            actual.BackLink.Should().Be($"/marketing/supplier/solution/{catalogueItem.CatalogueItemId}");
            actual.BackLinkText.Should().Be("Return to all sections");
            actual.Contact1.Should().BeEquivalentTo(catalogueItem.FirstContact());
            actual.Contact2.Should().BeEquivalentTo(catalogueItem.SecondContact());
            actual.ClientApplication.Should().BeEquivalentTo(clientApplication);
            actual.SolutionId.Should().Be(catalogueItem.CatalogueItemId);
            actual.SupplierId.Should().Be(catalogueItem.Supplier.Id);
        }

        [Test, IgnoreCircularReferenceAutoData]
        public static void Map_ContactDetailsModelToSupplierContactsModel_ResultAsExpected(
            ContactDetailsModel contactDetailsModel)
        {
            var mapper = new MapperConfiguration(cfg => cfg.AddProfile<OrganisationProfile>())
                .CreateMapper();

            var actual = mapper.Map<ContactDetailsModel, SupplierContactsModel>(contactDetailsModel);

            actual.Contacts.Should()
                .BeEquivalentTo(contactDetailsModel.Contact1, contactDetailsModel.Contact2);
            actual.SolutionId.Should().Be(contactDetailsModel.SolutionId);
        }
    }
}