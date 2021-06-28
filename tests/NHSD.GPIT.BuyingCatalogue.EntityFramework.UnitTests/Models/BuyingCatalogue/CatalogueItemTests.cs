using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.TestData;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.UnitTests.Models.BuyingCatalogue
{
    public static class CatalogueItemTests
    {
        [Theory]
        [CommonAutoData]
        public static void Features_NullSolution_ReturnsNull(CatalogueItem catalogueItem)
        {
            catalogueItem.Solution = null;

            var actual = catalogueItem.Features();

            actual.Should().BeNull();
        }

        [Theory]
        [CommonAutoData]
        public static void Features_NullFeatures_ReturnsNull(CatalogueItem catalogueItem)
        {
            catalogueItem.Solution.Features = null;

            var actual = catalogueItem.Features();

            actual.Should().BeNull();
        }

        [Theory]
        [CommonAutoData]
        public static void Features_EmptyFeatures_ReturnsNull(CatalogueItem catalogueItem)
        {
            catalogueItem.Solution.Features = string.Empty;

            var actual = catalogueItem.Features();

            actual.Should().BeNull();
        }

        [Theory]
        [CommonAutoData]
        public static void Features_SolutionHasValidFeatures_ReturnsFeatures(
            CatalogueItem catalogueItem,
            string[] expected)
        {
            catalogueItem.Solution.Features = JsonConvert.SerializeObject(expected);

            var actual = catalogueItem.Features();

            actual.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [CommonAutoData]
        public static void FirstContact_ValidModel_ReturnsFirstContact(CatalogueItem catalogueItem)
        {
            var expected = catalogueItem.Solution.MarketingContacts.First();

            var actual = catalogueItem.FirstContact();

            actual.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [CommonAutoData]
        public static void FirstContact_NoContactsInSolution_ReturnsEmptyObject(CatalogueItem catalogueItem)
        {
            catalogueItem.Solution.MarketingContacts = null;
            var expected = new MarketingContact();

            var actual = catalogueItem.FirstContact();

            actual.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [CommonAutoData]
        public static void FirstContact_NullSolution_ReturnsEmptyObject(CatalogueItem catalogueItem)
        {
            catalogueItem.Solution = null;
            var expected = new MarketingContact();

            var actual = catalogueItem.FirstContact();

            actual.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [CommonAutoData]
        public static void Frameworks_SolutionHasValidFrameworksSet_ReturnsFrameworkNames(
            CatalogueItem catalogueItem)
        {
            var expected = catalogueItem.Solution.FrameworkSolutions.Select(f => f.Framework.ShortName).ToList();
            expected.Count.Should().BeGreaterThan(1);

            var actual = catalogueItem.Frameworks();

            actual.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [CommonAutoData]
        public static void Frameworks_SolutionHasNullFrameworks_ReturnsEmptyList(CatalogueItem catalogueItem)
        {
            catalogueItem.Solution.FrameworkSolutions = null;

            var actual = catalogueItem.Frameworks();

            actual.Should().BeEmpty();
        }

        [Theory]
        [CommonAutoData]
        public static void Frameworks_FrameworkSolutionNull_ReturnsEmptyList(CatalogueItem catalogueItem)
        {
            catalogueItem.Solution.FrameworkSolutions = null;

            var actual = catalogueItem.Frameworks();

            actual.Should().BeEmpty();
        }

        [Theory]
        [CommonAutoData]
        public static void Frameworks_SolutionNull_ReturnsEmptyList(CatalogueItem catalogueItem)
        {
            catalogueItem.Solution = null;

            var actual = catalogueItem.Frameworks();

            actual.Should().BeEmpty();
        }

        [Theory]
        [CommonAutoData]
        public static void HasAdditionalServices_SolutionCapabilitiesNotEmpty_ReturnsTrue(CatalogueItem catalogueItem)
        {
            catalogueItem.Solution.AdditionalServices.Should().NotBeNullOrEmpty();

            var actual = catalogueItem.HasAdditionalServices();

            actual.Should().BeTrue();
        }

        [Fact]
        public static void HasAdditionalServices_AdditionalServicesEmpty_ReturnsFalse()
        {
            var catalogueItem = new CatalogueItem
            {
                Solution = new Solution { AdditionalServices = new List<AdditionalService>() },
            };

            var actual = catalogueItem.HasAdditionalServices();

            actual.Should().BeFalse();
        }

        [Fact]
        public static void HasAdditionalServices_AdditionalServicesNull_ReturnsFalse()
        {
            var catalogueItem = new CatalogueItem { Solution = new Solution { AdditionalServices = null } };

            var actual = catalogueItem.HasAdditionalServices();

            actual.Should().BeFalse();
        }

        [Fact]
        public static void HasAdditionalServices_SolutionNull_ReturnsFalse()
        {
            var catalogueItem = new CatalogueItem { Solution = null };

            var actual = catalogueItem.HasAdditionalServices();

            actual.Should().BeFalse();
        }

        [Theory]
        [CommonAutoData]
        public static void HasAssociatedServices_AssociatedServiceNotNull_ReturnsTrue(CatalogueItem catalogueItem)
        {
            catalogueItem.AssociatedService.Should().NotBeNull();

            var actual = catalogueItem.HasAssociatedServices();

            actual.Should().BeTrue();
        }

        [Fact]
        public static void HasAssociatedServices_AssociatedServiceNull_HasSupplierAssociatedService_ReturnsTrue()
        {
            var catalogueItem = new CatalogueItem
            {
                AssociatedService = null,
                Supplier = new Supplier
                {
                    CatalogueItems = new List<CatalogueItem>
                    {
                        new() { AssociatedService = new AssociatedService() },
                    },
                },
            };

            var actual = catalogueItem.HasAssociatedServices();

            actual.Should().BeTrue();
        }

        [Fact]
        public static void HasAssociatedServices_AssociatedServiceNull_NoSupplierAssociatedService_ReturnsFalse()
        {
            var catalogueItem = new CatalogueItem
            {
                AssociatedService = null,
                Supplier = new Supplier
                {
                    CatalogueItems = new List<CatalogueItem>
                    {
                        new() { AssociatedService = null },
                    },
                },
            };

            var actual = catalogueItem.HasAssociatedServices();

            actual.Should().BeFalse();
        }

        [Fact]
        public static void HasAssociatedServices_AssociatedServiceNull_SupplierHasNullCatalogueItems_ReturnsFalse()
        {
            var catalogueItem = new CatalogueItem
            {
                AssociatedService = null,
                Supplier = new Supplier
                {
                    CatalogueItems = null,
                },
            };

            var actual = catalogueItem.HasAssociatedServices();

            actual.Should().BeFalse();
        }

        [Fact]
        public static void HasAssociatedServices_AssociatedServiceNull_SupplierNull_ReturnsFalse()
        {
            var catalogueItem = new CatalogueItem
            {
                AssociatedService = null,
                Supplier = null,
            };

            var actual = catalogueItem.HasAssociatedServices();

            actual.Should().BeFalse();
        }

        [Theory]
        [CommonAutoData]
        public static void HasCapabilities_SolutionCapabilitiesNotEmpty_ReturnsTrue(CatalogueItem catalogueItem)
        {
            catalogueItem.Solution.SolutionCapabilities.Any().Should().BeTrue();

            var actual = catalogueItem.HasCapabilities();

            actual.Should().BeTrue();
        }

        [Theory]
        [CommonAutoData]
        public static void HasCapabilities_SolutionCapabilitiesEmpty_ReturnsFalse(CatalogueItem catalogueItem)
        {
            catalogueItem.Solution.SolutionCapabilities = new List<SolutionCapability>();

            var actual = catalogueItem.HasCapabilities();

            actual.Should().BeFalse();
        }

        [Theory]
        [CommonAutoData]
        public static void HasCapabilities_SolutionCapabilitiesNull_ReturnsFalse(CatalogueItem catalogueItem)
        {
            catalogueItem.Solution.SolutionCapabilities = null;

            var actual = catalogueItem.HasCapabilities();

            actual.Should().BeFalse();
        }

        [Theory]
        [CommonAutoData]
        public static void HasCapabilities_SolutionIsNull_ReturnsFalse(CatalogueItem catalogueItem)
        {
            catalogueItem.Solution = null;

            var actual = catalogueItem.HasCapabilities();

            actual.Should().BeFalse();
        }

        [Theory]
        [CommonAutoData]
        public static void HasClientApplication_SolutionHasClientApplication_ReturnsTrue(CatalogueItem catalogueItem)
        {
            catalogueItem.Solution.ClientApplication.Should().NotBeNullOrWhiteSpace();

            var actual = catalogueItem.HasClientApplication();

            actual.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(InvalidStringData.TestData), MemberType = typeof(InvalidStringData))]
        public static void HasClientApplication_SolutionHasInvalidClientApplication_ReturnsFalse(string invalid)
        {
            var catalogueItem = new CatalogueItem { Solution = new Solution { ClientApplication = invalid } };

            var actual = catalogueItem.HasClientApplication();

            actual.Should().BeFalse();
        }

        [Fact]
        public static void HasClientApplication_SolutionHasIsNull_ReturnsFalse()
        {
            var catalogueItem = new CatalogueItem { Solution = null };

            var actual = catalogueItem.HasClientApplication();

            actual.Should().BeFalse();
        }

        [Theory]
        [CommonAutoData]
        public static void HasDevelopmentPlans_SolutionHasDevelopmentPlans_ReturnsTrue(CatalogueItem catalogueItem)
        {
            catalogueItem.Solution.RoadMap.Should().NotBeNullOrWhiteSpace();

            var actual = catalogueItem.HasDevelopmentPlans();

            actual.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(InvalidStringData.TestData), MemberType = typeof(InvalidStringData))]
        public static void HasDevelopmentPlans_SolutionHasInvalidDevelopmentPlans_ReturnsFalse(string invalid)
        {
            var catalogueItem = new CatalogueItem { Solution = new Solution { RoadMap = invalid } };

            var actual = catalogueItem.HasDevelopmentPlans();

            actual.Should().BeFalse();
        }

        [Fact]
        public static void HasDevelopmentPlans_SolutionHasIsNull_ReturnsFalse()
        {
            var catalogueItem = new CatalogueItem { Solution = null };

            var actual = catalogueItem.HasDevelopmentPlans();

            actual.Should().BeFalse();
        }

        [Theory]
        [CommonAutoData]
        public static void HasFeatures_SolutionHasFeatures_ReturnsTrue(CatalogueItem catalogueItem)
        {
            catalogueItem.Solution.Features.Should().NotBeNullOrWhiteSpace();

            var actual = catalogueItem.HasFeatures();

            actual.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(InvalidStringData.TestData), MemberType = typeof(InvalidStringData))]
        public static void HasFeatures_SolutionHasInvalidFeatures_ReturnsFalse(string invalid)
        {
            var catalogueItem = new CatalogueItem { Solution = new Solution { Features = invalid } };

            var actual = catalogueItem.HasFeatures();

            actual.Should().BeFalse();
        }

        [Fact]
        public static void HasFeatures_SolutionHasIsNull_ReturnsFalse()
        {
            var catalogueItem = new CatalogueItem { Solution = null };

            var actual = catalogueItem.HasFeatures();

            actual.Should().BeFalse();
        }

        [Theory]
        [CommonAutoData]
        public static void HasHosting_SolutionHasHosting_ReturnsTrue(CatalogueItem catalogueItem)
        {
            catalogueItem.Solution.Hosting.Should().NotBeNullOrWhiteSpace();

            var actual = catalogueItem.HasHosting();

            actual.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(InvalidStringData.TestData), MemberType = typeof(InvalidStringData))]
        public static void HasHosting_SolutionHasInvalidHosting_ReturnsFalse(string invalid)
        {
            var catalogueItem = new CatalogueItem { Solution = new Solution { Hosting = invalid } };

            var actual = catalogueItem.HasHosting();

            actual.Should().BeFalse();
        }

        [Fact]
        public static void HasHosting_SolutionHasIsNull_ReturnsFalse()
        {
            var catalogueItem = new CatalogueItem { Solution = null };

            var actual = catalogueItem.HasHosting();

            actual.Should().BeFalse();
        }

        [Theory]
        [CommonAutoData]
        public static void HasImplementationDetail_SolutionHasImplementationDetail_ReturnsTrue(CatalogueItem catalogueItem)
        {
            catalogueItem.Solution.ImplementationDetail.Should().NotBeNullOrWhiteSpace();

            var actual = catalogueItem.HasImplementationDetail();

            actual.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(InvalidStringData.TestData), MemberType = typeof(InvalidStringData))]
        public static void HasImplementationDetail_SolutionHasInvalidImplementationDetail_ReturnsFalse(string invalid)
        {
            var catalogueItem = new CatalogueItem { Solution = new Solution { ImplementationDetail = invalid } };

            var actual = catalogueItem.HasImplementationDetail();

            actual.Should().BeFalse();
        }

        [Fact]
        public static void HasImplementationDetail_SolutionHasIsNull_ReturnsFalse()
        {
            var catalogueItem = new CatalogueItem { Solution = null };

            var actual = catalogueItem.HasImplementationDetail();

            actual.Should().BeFalse();
        }

        [Theory]
        [CommonAutoData]
        public static void HasInteroperability_SolutionHasIntegration_ReturnsTrue(CatalogueItem catalogueItem)
        {
            catalogueItem.Solution.Integrations.Should().NotBeNullOrWhiteSpace();

            var actual = catalogueItem.HasInteroperability();

            actual.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(InvalidStringData.TestData), MemberType = typeof(InvalidStringData))]
        public static void HasInteroperability_SolutionHasInvalidIntegration_ReturnsFalse(string invalid)
        {
            var catalogueItem = new CatalogueItem { Solution = new Solution { Integrations = invalid } };

            var actual = catalogueItem.HasInteroperability();

            actual.Should().BeFalse();
        }

        [Fact]
        public static void HasInteroperability_SolutionHasIsNull_ReturnsFalse()
        {
            var catalogueItem = new CatalogueItem { Solution = null };

            var actual = catalogueItem.HasInteroperability();

            actual.Should().BeFalse();
        }

        [Theory]
        [CommonAutoData]
        public static void HasListPrice_CataloguePricesNotEmpty_ReturnsTrue(CatalogueItem catalogueItem)
        {
            catalogueItem.CataloguePrices.Any().Should().BeTrue();

            var actual = catalogueItem.HasListPrice();

            actual.Should().BeTrue();
        }

        [Fact]
        public static void HasListPrice_CataloguePricesIsEmpty_ReturnsFalse()
        {
            var catalogueItem = new CatalogueItem { CataloguePrices = new List<CataloguePrice>() };

            var actual = catalogueItem.HasListPrice();

            actual.Should().BeFalse();
        }

        [Fact]
        public static void HasListPrice_CataloguePricesNull_ReturnsFalse()
        {
            var catalogueItem = new CatalogueItem { CataloguePrices = null };

            var actual = catalogueItem.HasListPrice();

            actual.Should().BeFalse();
        }

        [Theory]
        [CommonAutoData]
        public static void HasServiceLevelAgreement_SolutionHasServiceLevelAgreement_ReturnsTrue(CatalogueItem catalogueItem)
        {
            catalogueItem.Solution.ServiceLevelAgreement.Should().NotBeNullOrWhiteSpace();

            var actual = catalogueItem.HasServiceLevelAgreement();

            actual.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(InvalidStringData.TestData), MemberType = typeof(InvalidStringData))]
        public static void HasServiceLevelAgreement_SolutionHasInvalidServiceLevelAgreement_ReturnsFalse(string invalid)
        {
            var catalogueItem = new CatalogueItem { Solution = new Solution { ServiceLevelAgreement = invalid } };

            var actual = catalogueItem.HasServiceLevelAgreement();

            actual.Should().BeFalse();
        }

        [Fact]
        public static void HasServiceLevelAgreement_SolutionIsNull_ReturnsFalse()
        {
            var catalogueItem = new CatalogueItem { Solution = null };

            var actual = catalogueItem.HasServiceLevelAgreement();

            actual.Should().BeFalse();
        }

        [Theory]
        [CommonAutoData]
        public static void HasSupplierDetails_SupplierNotNull_ReturnsTrue(CatalogueItem catalogueItem)
        {
            catalogueItem.Supplier.Should().NotBeNull();

            var actual = catalogueItem.HasSupplierDetails();

            actual.Should().BeTrue();
        }

        [Fact]
        public static void HasSupplierDetails_SupplierIsNull_ReturnsFalse()
        {
            var catalogueItem = new CatalogueItem { Supplier = null };

            var actual = catalogueItem.HasSupplierDetails();

            actual.Should().BeFalse();
        }

        [Fact]
        public static void HasSupplierDetails_SolutionIsNull_ReturnsFalse()
        {
            var catalogueItem = new CatalogueItem { Solution = null };

            var actual = catalogueItem.HasSupplierDetails();

            actual.Should().BeFalse();
        }

        [Theory]
        [CommonAutoData]
        public static void IsFoundation_OneSolutionIsFoundation_ReturnsTrue(CatalogueItem catalogueItem)
        {
            catalogueItem.Solution.FrameworkSolutions = new List<FrameworkSolution> { new() { IsFoundation = true } };

            var actual = catalogueItem.IsFoundation();

            actual.Should().BeTrue();
        }

        [Theory]
        [CommonAutoData]
        public static void IsFoundation_NoSolutionIsFoundation_ReturnsFalse(CatalogueItem catalogueItem)
        {
            catalogueItem.Solution.FrameworkSolutions.Should().NotBeEmpty();
            catalogueItem.Solution.FrameworkSolutions.ToList().ForEach(f => f.IsFoundation = false);

            var actual = catalogueItem.IsFoundation();

            actual.Should().BeFalse();
        }

        [Theory]
        [CommonAutoData]
        public static void IsFoundation_NullFrameworkSolutions_ReturnsNull(CatalogueItem catalogueItem)
        {
            catalogueItem.Solution.FrameworkSolutions = null;

            var actual = catalogueItem.IsFoundation();

            actual.Should().BeNull();
        }

        [Theory]
        [CommonAutoData]
        public static void IsFoundation_NullSolution_ReturnsNull(CatalogueItem catalogueItem)
        {
            catalogueItem.Solution = null;

            var actual = catalogueItem.IsFoundation();

            actual.Should().BeNull();
        }

        [Theory]
        [CommonAutoData]
        public static void SecondContact_ValidModel_ReturnsSecondContact(CatalogueItem catalogueItem)
        {
            var expected = catalogueItem.Solution.MarketingContacts.Skip(1).First();

            var actual = catalogueItem.SecondContact();

            actual.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [CommonAutoData]
        public static void SecondContact_NoContactsInSolution_ReturnsEmptyObject(CatalogueItem catalogueItem)
        {
            catalogueItem.Solution.MarketingContacts = null;
            var expected = new MarketingContact();

            var actual = catalogueItem.SecondContact();

            actual.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [CommonAutoData]
        public static void SecondContact_NullSolution_ReturnsEmptyObject(CatalogueItem catalogueItem)
        {
            catalogueItem.Solution = null;
            var expected = new MarketingContact();

            var actual = catalogueItem.SecondContact();

            actual.Should().BeEquivalentTo(expected);
        }
    }
}
