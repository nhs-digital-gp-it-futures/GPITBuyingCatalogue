using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Models
{
    public static class ServiceLevelAgreementDetailsModelTests
    {
        [Fact]
        public static void Constructing_NullCatalogueItem_ThrowsArgumentNullException()
            => Assert.Throws<ArgumentNullException>(() => new ServiceLevelAgreementDetailsModel(
                null,
                new CatalogueItemContentStatus()));

        [Fact]
        public static void Constructing_CatalogueItemWithoutSolution_ThrowsArgumentException()
        {
            var catalogueItem = new CatalogueItem
            {
                Solution = null,
            };

            Assert.Throws<ArgumentException>(() => new ServiceLevelAgreementDetailsModel(
                catalogueItem,
                new CatalogueItemContentStatus()));
        }

        [Fact]
        public static void Constructing_WithServiceLevelAgreement_SetsProperties()
        {
            var serviceHours = new[] { new ServiceAvailabilityTimes() };
            var contacts = new[] { new SlaContact() };
            var serviceLevels = new[] { new SlaServiceLevel() };

            var catalogueItem = new CatalogueItem
            {
                Solution = new Solution
                {
                    ServiceLevelAgreement = new ServiceLevelAgreements
                    {
                        ServiceHours = serviceHours,
                        Contacts = contacts,
                        ServiceLevels = serviceLevels,
                    },
                },
            };

            var model = new ServiceLevelAgreementDetailsModel(
                catalogueItem,
                new CatalogueItemContentStatus());

            Assert.Equal(serviceHours, model.ServiceAvailabilityTimes);
            Assert.Equal(contacts, model.SlaContacts);
            Assert.Equal(serviceLevels, model.ServiceLevels);
            Assert.Equal(13, model.Index);
        }

        [Fact]
        public static void Constructing_WithoutServiceLevelAgreement_DoesNotSetCollections()
        {
            var catalogueItem = new CatalogueItem
            {
                Solution = new Solution
                {
                    ServiceLevelAgreement = null,
                },
            };

            var model = new ServiceLevelAgreementDetailsModel(
                catalogueItem,
                new CatalogueItemContentStatus());

            Assert.Null(model.ServiceAvailabilityTimes);
            Assert.Null(model.SlaContacts);
            Assert.Null(model.ServiceLevels);
        }
    }
}
