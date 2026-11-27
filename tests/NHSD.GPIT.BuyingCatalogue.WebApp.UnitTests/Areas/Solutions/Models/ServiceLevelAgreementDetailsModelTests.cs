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
                new CatalogueItemContentStatus(),
                new SolutionSections()));

        [Fact]
        public static void Constructing_WithServiceLevelAgreement_SetsProperties()
        {
            var serviceHours = new[] { new ServiceAvailabilityTimes() };
            var contacts = new[] { new SlaContact() };
            var serviceLevels = new[] { new SlaServiceLevel() };

            var catalogueItem = new CatalogueItem
            {
                Supplier = new Supplier
                {
                    Name = "Test Supplier",
                },

                Solution = new Solution
                {
                    FrameworkSolutions =
                    [
                        new FrameworkSolution
                        {
                            Framework = new NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models.Framework()
                            {
                                Id = "test-framework",
                                Name = "Test Framework",
                            },
                        },
                    ],
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
                new CatalogueItemContentStatus(),
                new SolutionSections());

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
                Supplier = new Supplier
                {
                    Name = "Test Supplier",
                },
                Solution = new Solution
                {
                    FrameworkSolutions =
                    [
                        new FrameworkSolution
                        {
                            Framework = new NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models.Framework()
                            {
                                Id = "test-framework",
                                Name = "Test Framework",
                            },
                        },
                    ],
                    ServiceLevelAgreement = null,
                },
            };

            var model = new ServiceLevelAgreementDetailsModel(
                catalogueItem,
                new CatalogueItemContentStatus(),
                new SolutionSections());

            Assert.Null(model.ServiceAvailabilityTimes);
            Assert.Null(model.SlaContacts);
            Assert.Null(model.ServiceLevels);
        }
    }
}
