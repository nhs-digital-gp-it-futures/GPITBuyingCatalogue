using System.Linq;
using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.AboutOrganisation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.AboutSolution;
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
        public static void Map_CatalogueItemToFeaturesModel_ResultAsExpected(CatalogueItem catalogueItem,
            ClientApplication clientApplication)
        {
            var features = new Fixture().CreateMany<string>(10).ToList();
            catalogueItem.Solution.ClientApplication = JsonConvert.SerializeObject(clientApplication);
            catalogueItem.Solution.Features = JsonConvert.SerializeObject(features);
            var mapper = new MapperConfiguration(cfg => cfg.AddProfile<OrganisationProfile>())
                .CreateMapper();

            var actual = mapper.Map<CatalogueItem, FeaturesModel>(catalogueItem);
            
            actual.BackLink.Should().Be($"/marketing/supplier/solution/{catalogueItem.CatalogueItemId}");
            actual.BackLinkText.Should().Be("Return to all sections");
            actual.ClientApplication.Should().BeEquivalentTo(clientApplication);
            actual.Listing1.Should().Be(features[0]);
            actual.Listing2.Should().Be(features[1]);
            actual.Listing3.Should().Be(features[2]);
            actual.Listing4.Should().Be(features[3]);
            actual.Listing5.Should().Be(features[4]);
            actual.Listing6.Should().Be(features[5]);
            actual.Listing7.Should().Be(features[6]);
            actual.Listing8.Should().Be(features[7]);
            actual.Listing9.Should().Be(features[8]);
            actual.Listing10.Should().Be(features[9]);
            actual.SolutionId.Should().Be(catalogueItem.CatalogueItemId);
            actual.SupplierId.Should().Be(catalogueItem.Supplier.Id);
        }

        [Test, IgnoreCircularReferenceAutoData]
        public static void Map_CatalogueItemToFeaturesModel_NoFeaturesString_ListingsNotSet(CatalogueItem catalogueItem)
        {
            catalogueItem.Solution.ClientApplication = null;
            catalogueItem.Solution.Features = string.Empty;
            var mapper = new MapperConfiguration(cfg => cfg.AddProfile<OrganisationProfile>())
                .CreateMapper();

            var actual = mapper.Map<CatalogueItem, FeaturesModel>(catalogueItem);

            actual.ClientApplication.Should().BeEquivalentTo(new ClientApplication());
            actual.Listing1.Should().Be(string.Empty);
            actual.Listing2.Should().Be(string.Empty);
            actual.Listing3.Should().Be(string.Empty);
            actual.Listing4.Should().Be(string.Empty);
            actual.Listing5.Should().Be(string.Empty);
            actual.Listing6.Should().Be(string.Empty);
            actual.Listing7.Should().Be(string.Empty);
            actual.Listing8.Should().Be(string.Empty);
            actual.Listing9.Should().Be(string.Empty);
            actual.Listing10.Should().Be(string.Empty);
        }

        [Test, IgnoreCircularReferenceAutoData]
        public static void Map_CatalogueItemToImplementationTimescalesModel_ResultAsExpected(
            CatalogueItem catalogueItem, ClientApplication clientApplication)
        {
            catalogueItem.Solution.ClientApplication = JsonConvert.SerializeObject(clientApplication);
            var mapper = new MapperConfiguration(cfg => cfg.AddProfile<OrganisationProfile>())
                .CreateMapper();
            
            var actual = mapper.Map<CatalogueItem, ImplementationTimescalesModel>(catalogueItem);

            actual.BackLink.Should().Be($"/marketing/supplier/solution/{catalogueItem.CatalogueItemId}");
            actual.BackLinkText.Should().Be("Return to all sections");
            actual.ClientApplication.Should().BeEquivalentTo(clientApplication);
            actual.Description.Should().Be(catalogueItem.Solution.ImplementationDetail);
            actual.SolutionId.Should().Be(catalogueItem.CatalogueItemId);
            actual.SupplierId.Should().Be(catalogueItem.Supplier.Id);
        }
        
        [Test, IgnoreCircularReferenceAutoData]
        public static void Map_CatalogueItemToIntegrationsModel_ResultAsExpected(
            CatalogueItem catalogueItem, ClientApplication clientApplication)
        {
            catalogueItem.Solution.ClientApplication = JsonConvert.SerializeObject(clientApplication);
            var mapper = new MapperConfiguration(cfg => cfg.AddProfile<OrganisationProfile>())
                .CreateMapper();
            
            var actual = mapper.Map<CatalogueItem, IntegrationsModel>(catalogueItem);

            actual.BackLink.Should().Be($"/marketing/supplier/solution/{catalogueItem.CatalogueItemId}");
            actual.BackLinkText.Should().Be("Return to all sections");
            actual.ClientApplication.Should().BeEquivalentTo(clientApplication);
            actual.Link.Should().Be(catalogueItem.Solution.IntegrationsUrl);
            actual.SolutionId.Should().Be(catalogueItem.CatalogueItemId);
            actual.SupplierId.Should().Be(catalogueItem.Supplier.Id);
        }
        
        [Test, IgnoreCircularReferenceAutoData]
        public static void Map_CatalogueItemToRoadMapModel_ResultAsExpected(
            CatalogueItem catalogueItem, ClientApplication clientApplication)
        {
            catalogueItem.Solution.ClientApplication = JsonConvert.SerializeObject(clientApplication);
            var mapper = new MapperConfiguration(cfg => cfg.AddProfile<OrganisationProfile>())
                .CreateMapper();
            
            var actual = mapper.Map<CatalogueItem, RoadmapModel>(catalogueItem);

            actual.BackLink.Should().Be($"/marketing/supplier/solution/{catalogueItem.CatalogueItemId}");
            actual.BackLinkText.Should().Be("Return to all sections");
            actual.ClientApplication.Should().BeEquivalentTo(clientApplication);
            actual.SolutionId.Should().Be(catalogueItem.CatalogueItemId);
            actual.Summary.Should().Be(catalogueItem.Solution.RoadMap);
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

        [Test, IgnoreCircularReferenceAutoData]
        public static void Map_CatalogueItemToSolutionDescriptionModel_ResultAsExpected(CatalogueItem catalogueItem,
            ClientApplication clientApplication)
        {
            catalogueItem.Solution.ClientApplication = JsonConvert.SerializeObject(clientApplication);
            var mapper = new MapperConfiguration(cfg => cfg.AddProfile<OrganisationProfile>())
                .CreateMapper();

            var actual = mapper.Map<CatalogueItem, SolutionDescriptionModel>(catalogueItem);
            
            actual.BackLink.Should().Be($"/marketing/supplier/solution/{catalogueItem.CatalogueItemId}");
            actual.BackLinkText.Should().Be("Return to all sections");
            actual.ClientApplication.Should().BeEquivalentTo(clientApplication);
            actual.Description.Should().Be(catalogueItem.Solution.FullDescription);
            actual.Link.Should().Be(catalogueItem.Solution.AboutUrl);
            actual.SolutionId.Should().Be(catalogueItem.CatalogueItemId);
            actual.Summary.Should().Be(catalogueItem.Solution.Summary);
            actual.SupplierId.Should().Be(catalogueItem.Supplier.Id);
        }
    }
}