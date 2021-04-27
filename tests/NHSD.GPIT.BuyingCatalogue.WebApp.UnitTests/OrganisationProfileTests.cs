using AutoFixture.NUnit3;
using AutoMapper;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
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