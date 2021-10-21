using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.TestData;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.UnitTests.Models.BuyingCatalogue
{
    public static class CatalogueItemTests
    {
        [Theory]
        [CommonAutoData]
        public static void CatalogueItemCapability_ValidSolutionCapabilities_ReturnsFirst(
            CatalogueItem catalogueItem)
        {
            var expected = catalogueItem.CatalogueItemCapabilities.First();
            var capabilityId = expected.Capability.Id;

            var actual = catalogueItem.CatalogueItemCapability(capabilityId);

            actual.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [CommonAutoData]
        public static void CatalogueItemCapability_SolutionCapabilityHasNullCapability_ReturnsEmpty(
            CatalogueItem catalogueItem,
            int capabilityId)
        {
            var expected = new CatalogueItemCapability { CatalogueItemId = catalogueItem.Id };
            catalogueItem.CatalogueItemCapabilities = new List<CatalogueItemCapability>
            {
                new(),
            };

            var actual = catalogueItem.CatalogueItemCapability(capabilityId);

            actual.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [CommonAutoData]
        public static void CatalogueItemCapability_EmptySolutionCapabilities_ReturnsEmpty(
            CatalogueItem catalogueItem,
            int capabilityId)
        {
            var expected = new CatalogueItemCapability { CatalogueItemId = catalogueItem.Id };
            catalogueItem.CatalogueItemCapabilities = new List<CatalogueItemCapability>();

            var actual = catalogueItem.CatalogueItemCapability(capabilityId);

            actual.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [CommonAutoData]
        public static void CatalogueItemCapability_NullSolutionCapabilities_ReturnsEmpty(
            CatalogueItem catalogueItem,
            int capabilityId)
        {
            var expected = new CatalogueItemCapability { CatalogueItemId = catalogueItem.Id };
            catalogueItem.CatalogueItemCapabilities = null;

            var actual = catalogueItem.CatalogueItemCapability(capabilityId);

            actual.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [CommonAutoData]
        public static void CatalogueItemCapability_NullSolution_ReturnsEmpty(
            CatalogueItem catalogueItem,
            int capabilityId)
        {
            var expected = new CatalogueItemCapability { CatalogueItemId = catalogueItem.Id };
            catalogueItem.Solution = null;

            var actual = catalogueItem.CatalogueItemCapability(capabilityId);

            actual.Should().BeEquivalentTo(expected);
        }

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
        public static void Features_NullFeatures_ReturnsNull(Solution solution)
        {
            solution.Features = null;

            var actual = solution.CatalogueItem.Features();

            actual.Should().BeNull();
        }

        [Theory]
        [CommonAutoData]
        public static void Features_EmptyFeatures_ReturnsNull(Solution solution)
        {
            solution.Features = string.Empty;

            var actual = solution.CatalogueItem.Features();

            actual.Should().BeNull();
        }

        [Theory]
        [CommonAutoData]
        public static void Features_SolutionHasValidFeatures_ReturnsFeatures(
            Solution solution,
            string[] expected)
        {
            solution.Features = JsonSerializer.Serialize(expected);

            var actual = solution.CatalogueItem.Features();

            actual.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [CommonAutoData]
        public static void FirstContact_ValidModel_ReturnsFirstContact(Solution solution)
        {
            var expected = solution.MarketingContacts.First();

            var actual = solution.CatalogueItem.FirstContact();

            actual.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [CommonAutoData]
        public static void FirstContact_NoContactsInSolution_ReturnsEmptyObject(Solution solution)
        {
            solution.MarketingContacts = null;
            var expected = new MarketingContact();

            var actual = solution.CatalogueItem.FirstContact();

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
            Solution solution)
        {
            var expected = solution.FrameworkSolutions.Select(f => f.Framework.ShortName).ToList();
            expected.Count.Should().BeGreaterThan(1);

            var actual = solution.CatalogueItem.Frameworks();

            actual.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [CommonAutoData]
        public static void Frameworks_SolutionHasNullFrameworks_ReturnsEmptyList(Solution solution)
        {
            solution.FrameworkSolutions = null;

            var actual = solution.CatalogueItem.Frameworks();

            actual.Should().BeEmpty();
        }

        [Theory]
        [CommonAutoData]
        public static void Frameworks_FrameworkSolutionNull_ReturnsEmptyList(Solution solution)
        {
            solution.FrameworkSolutions = null;

            var actual = solution.CatalogueItem.Frameworks();

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
        public static void HasAdditionalServices_SolutionCapabilitiesNotEmpty_ReturnsTrue(Solution solution)
        {
            solution.AdditionalServices.Should().NotBeNullOrEmpty();

            var actual = solution.CatalogueItem.HasAdditionalServices();

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
        public static void HasAssociatedServices_AssociatedServiceNotNull_ReturnsTrue(AssociatedService associatedService)
        {
            associatedService.Should().NotBeNull();

            var actual = associatedService.CatalogueItem.HasAssociatedServices();

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
            catalogueItem.CatalogueItemCapabilities.Any().Should().BeTrue();

            var actual = catalogueItem.HasCapabilities();

            actual.Should().BeTrue();
        }

        [Theory]
        [CommonAutoData]
        public static void HasCapabilities_SolutionCapabilitiesEmpty_ReturnsFalse(CatalogueItem catalogueItem)
        {
            catalogueItem.CatalogueItemCapabilities = new List<CatalogueItemCapability>();

            var actual = catalogueItem.HasCapabilities();

            actual.Should().BeFalse();
        }

        [Theory]
        [CommonAutoData]
        public static void HasCapabilities_SolutionCapabilitiesNull_ReturnsFalse(CatalogueItem catalogueItem)
        {
            catalogueItem.CatalogueItemCapabilities = null;

            var actual = catalogueItem.HasCapabilities();

            actual.Should().BeFalse();
        }

        [Theory]
        [CommonAutoData]
        public static void HasCapabilities_SolutionIsNull_ReturnsFalse(CatalogueItem catalogueItem)
        {
            catalogueItem.CatalogueItemCapabilities = null;

            var actual = catalogueItem.HasCapabilities();

            actual.Should().BeFalse();
        }

        [Theory]
        [CommonAutoData]
        public static void HasClientApplication_SolutionHasClientApplication_ReturnsTrue(Solution solution)
        {
            solution.ClientApplication.Should().NotBeNullOrWhiteSpace();

            var actual = solution.CatalogueItem.HasClientApplication();

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
        public static void HasDevelopmentPlans_SolutionHasDevelopmentPlans_ReturnsTrue(Solution solution)
        {
            solution.RoadMap.Should().NotBeNullOrWhiteSpace();

            var actual = solution.CatalogueItem.HasDevelopmentPlans();

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
        public static void HasFeatures_SolutionHasFeatures_ReturnsTrue(Solution solution)
        {
            solution.Features.Should().NotBeNullOrWhiteSpace();

            var actual = solution.CatalogueItem.HasFeatures();

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
        public static void HasHosting_SolutionHasHosting_ReturnsTrue(Solution solution)
        {
            solution.Hosting.Should().NotBeNull();

            var actual = solution.CatalogueItem.HasHosting();

            actual.Should().BeTrue();
        }

        [Theory]
        [CommonAutoData]
        public static void HasHosting_SolutionHasInvalidHosting_ReturnsFalse(Solution solution)
        {
            solution.Hosting = null;

            var actual = solution.CatalogueItem.HasHosting();

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
        public static void HasImplementationDetail_SolutionHasImplementationDetail_ReturnsTrue(Solution solution)
        {
            solution.ImplementationDetail.Should().NotBeNullOrWhiteSpace();

            var actual = solution.CatalogueItem.HasImplementationDetail();

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
        public static void HasInteroperability_SolutionHasIntegration_ReturnsTrue(Solution solution)
        {
            solution.Integrations.Should().NotBeNullOrWhiteSpace();

            var actual = solution.CatalogueItem.HasInteroperability();

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
        public static void IsFoundation_OneSolutionIsFoundation_ReturnsTrue(Solution solution)
        {
            solution.FrameworkSolutions = new List<FrameworkSolution> { new() { IsFoundation = true } };

            var actual = solution.CatalogueItem.IsFoundation();

            actual.Should().BeTrue();
        }

        [Theory]
        [CommonAutoData]
        public static void IsFoundation_NoSolutionIsFoundation_ReturnsFalse(Solution solution)
        {
            solution.FrameworkSolutions.Should().NotBeEmpty();
            solution.FrameworkSolutions.ToList().ForEach(f => f.IsFoundation = false);

            var actual = solution.CatalogueItem.IsFoundation();

            actual.Should().BeFalse();
        }

        [Theory]
        [CommonAutoData]
        public static void IsFoundation_NullFrameworkSolutions_ReturnsNull(Solution solution)
        {
            solution.FrameworkSolutions = null;

            var actual = solution.CatalogueItem.IsFoundation();

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
        public static void SecondContact_ValidModel_ReturnsSecondContact(Solution solution)
        {
            var expected = solution.MarketingContacts.Skip(1).First();

            var actual = solution.CatalogueItem.SecondContact();

            actual.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [CommonAutoData]
        public static void SecondContact_NoContactsInSolution_ReturnsEmptyObject(Solution solution)
        {
            solution.MarketingContacts = null;
            var expected = new MarketingContact();

            var actual = solution.CatalogueItem.SecondContact();

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

        [Theory]
        [CommonInlineAutoData(PublicationStatus.Published, true)]
        [CommonInlineAutoData(PublicationStatus.InRemediation, true)]
        [CommonInlineAutoData(PublicationStatus.Draft, false)]
        [CommonInlineAutoData(PublicationStatus.Suspended, false)]
        [CommonInlineAutoData(PublicationStatus.Unpublished, false)]
        public static void IsBrowsable_WithPublicationStatus_ReturnsExpectedResult(
            PublicationStatus publicationStatus,
            bool expectedResult,
            CatalogueItem catalogueItem)
        {
            catalogueItem.PublishedStatus = publicationStatus;

            catalogueItem
                .IsBrowsable
                .Should()
                .Be(expectedResult);
        }
    }
}
