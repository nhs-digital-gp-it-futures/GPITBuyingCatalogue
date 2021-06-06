using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.UnitTests.Models.BuyingCatalogue
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class CatalogueItemTests
    {
        private static readonly string[] InvalidStrings = { null, string.Empty, "    " };
        
        [Test, CommonAutoData]
        public static void Features_NullSolution_ReturnsNull(CatalogueItem catalogueItem)
        {
            catalogueItem.Solution = null;

            var actual = catalogueItem.Features();

            actual.Should().BeNull();
        }

        [Test, CommonAutoData]
        public static void Features_NullFeatures_ReturnsNull(CatalogueItem catalogueItem)
        {
            catalogueItem.Solution.Features = null;

            var actual = catalogueItem.Features();

            actual.Should().BeNull();
        }

        [Test, CommonAutoData]
        public static void Features_EmptyFeatures_ReturnsNull(CatalogueItem catalogueItem)
        {
            catalogueItem.Solution.Features = string.Empty;

            var actual = catalogueItem.Features();

            actual.Should().BeNull();
        }

        [Test, CommonAutoData]
        public static void Features_SolutionHasValidFeatures_ReturnsFeatures(
            CatalogueItem catalogueItem,
            string[] expected)
        {
            catalogueItem.Solution.Features = JsonConvert.SerializeObject(expected);

            var actual = catalogueItem.Features();

            actual.Should().BeEquivalentTo(expected);
        }
        
        [Test, CommonAutoData]
        public static void FirstContact_ValidModel_ReturnsFirstContact(CatalogueItem catalogueItem)
        {
            var expected = catalogueItem.Solution.MarketingContacts.First();

            var actual = catalogueItem.FirstContact();

            actual.Should().BeEquivalentTo(expected);
        }

        [Test, CommonAutoData]
        public static void FirstContact_NoContactsInSolution_ReturnsEmptyObject(CatalogueItem catalogueItem)
        {
            catalogueItem.Solution.MarketingContacts = null;
            var expected = new MarketingContact();

            var actual = catalogueItem.FirstContact();

            actual.Should().BeEquivalentTo(expected);
        }

        [Test, CommonAutoData]
        public static void FirstContact_NullSolution_ReturnsEmptyObject(CatalogueItem catalogueItem)
        {
            catalogueItem.Solution = null;
            var expected = new MarketingContact();

            var actual = catalogueItem.FirstContact();

            actual.Should().BeEquivalentTo(expected);
        }

        [Test, CommonAutoData]
        public static void Framework_SolutionHasValidFrameworkSet_ReturnsFramework(
            CatalogueItem catalogueItem,
            string expected)
        {
            catalogueItem.Solution.FrameworkSolutions.First().Framework.Name = expected;

            var actual = catalogueItem.Framework();

            actual.Should().Be(expected);
        }

        [Test, CommonAutoData]
        public static void Framework_SolutionHasNullFramework_ReturnsNull(CatalogueItem catalogueItem)
        {
            catalogueItem.Solution.FrameworkSolutions.First().Framework = null;

            var actual = catalogueItem.Framework();

            actual.Should().BeNull();
        }

        [Test, CommonAutoData]
        public static void Framework_FrameworkSolutionNull_ReturnsNull(CatalogueItem catalogueItem)
        {
            catalogueItem.Solution.FrameworkSolutions = null;

            var actual = catalogueItem.Framework();

            actual.Should().BeNull();
        }

        [Test, CommonAutoData]
        public static void Framework_SolutionNull_ReturnsNull(CatalogueItem catalogueItem)
        {
            catalogueItem.Solution = null;

            var actual = catalogueItem.Framework();

            actual.Should().BeNull();
        }

        [Test, CommonAutoData]
        public static void HasCapabilities_SolutionCapabilitiesNotEmpty_ReturnsTrue(CatalogueItem catalogueItem)
        {
            catalogueItem.Solution.SolutionCapabilities.Any().Should().BeTrue();
            
            var actual = catalogueItem.HasCapabilities();

            actual.Should().BeTrue();
        }

        [Test, CommonAutoData]
        public static void HasCapabilities_SolutionCapabilitiesEmpty_ReturnsFalse(CatalogueItem catalogueItem)
        {
            catalogueItem.Solution.SolutionCapabilities = new List<SolutionCapability>();
            
            var actual = catalogueItem.HasCapabilities();

            actual.Should().BeFalse();
        }

        [Test, CommonAutoData]
        public static void HasCapabilities_SolutionCapabilitiesNull_ReturnsFalse(CatalogueItem catalogueItem)
        {
            catalogueItem.Solution.SolutionCapabilities = null;
            
            var actual = catalogueItem.HasCapabilities();

            actual.Should().BeFalse();
        }

        [Test, CommonAutoData]
        public static void HasCapabilities_SolutionIsNull_ReturnsFalse(CatalogueItem catalogueItem)
        {
            catalogueItem.Solution = null;
            
            var actual = catalogueItem.HasCapabilities();

            actual.Should().BeFalse();
        }

        [Test, CommonAutoData]
        public static void HasClientApplication_SolutionHasClientApplication_ReturnsTrue(CatalogueItem catalogueItem)
        {
            catalogueItem.Solution.ClientApplication.Should().NotBeNullOrWhiteSpace();
            
            var actual = catalogueItem.HasClientApplication();

            actual.Should().BeTrue();
        }

        [TestCaseSource(nameof(InvalidStrings))]
        public static void HasClientApplication_SolutionHasInvalidClientApplication_ReturnsFalse(string invalid)
        {
            var catalogueItem = new CatalogueItem { Solution = new Solution { ClientApplication = invalid } };
            
            var actual = catalogueItem.HasClientApplication();

            actual.Should().BeFalse();
        }

        [Test]
        public static void HasClientApplication_SolutionHasIsNull_ReturnsFalse()
        {
            var catalogueItem = new CatalogueItem { Solution = null };
            
            var actual = catalogueItem.HasClientApplication();

            actual.Should().BeFalse();
        }
        
        [Test, CommonAutoData]
        public static void HasFeatures_SolutionHasFeatures_ReturnsTrue(CatalogueItem catalogueItem)
        {
            catalogueItem.Solution.Features.Should().NotBeNullOrWhiteSpace();
            
            var actual = catalogueItem.HasFeatures();

            actual.Should().BeTrue();
        }

        [TestCaseSource(nameof(InvalidStrings))]
        public static void HasFeatures_SolutionHasInvalidFeatures_ReturnsFalse(string invalid)
        {
            var catalogueItem = new CatalogueItem { Solution = new Solution { Features = invalid } };
            
            var actual = catalogueItem.HasFeatures();

            actual.Should().BeFalse();
        }

        [Test]
        public static void HasFeatures_SolutionHasIsNull_ReturnsFalse()
        {
            var catalogueItem = new CatalogueItem { Solution = null };
            
            var actual = catalogueItem.HasFeatures();

            actual.Should().BeFalse();
        }
        
        [Test, CommonAutoData]
        public static void HasHosting_SolutionHasHosting_ReturnsTrue(CatalogueItem catalogueItem)
        {
            catalogueItem.Solution.Hosting.Should().NotBeNullOrWhiteSpace();
            
            var actual = catalogueItem.HasHosting();

            actual.Should().BeTrue();
        }

        [TestCaseSource(nameof(InvalidStrings))]
        public static void HasHosting_SolutionHasInvalidHosting_ReturnsFalse(string invalid)
        {
            var catalogueItem = new CatalogueItem { Solution = new Solution { Hosting = invalid } };
            
            var actual = catalogueItem.HasHosting();

            actual.Should().BeFalse();
        }

        [Test]
        public static void HasHosting_SolutionHasIsNull_ReturnsFalse()
        {
            var catalogueItem = new CatalogueItem { Solution = null };
            
            var actual = catalogueItem.HasHosting();

            actual.Should().BeFalse();
        }
        
        [Test, CommonAutoData]
        public static void HasImplementationDetail_SolutionHasImplementationDetail_ReturnsTrue(CatalogueItem catalogueItem)
        {
            catalogueItem.Solution.ImplementationDetail.Should().NotBeNullOrWhiteSpace();
            
            var actual = catalogueItem.HasImplementationDetail();

            actual.Should().BeTrue();
        }

        [TestCaseSource(nameof(InvalidStrings))]
        public static void HasImplementationDetail_SolutionHasInvalidImplementationDetail_ReturnsFalse(string invalid)
        {
            var catalogueItem = new CatalogueItem { Solution = new Solution { ImplementationDetail = invalid } };
            
            var actual = catalogueItem.HasImplementationDetail();

            actual.Should().BeFalse();
        }

        [Test]
        public static void HasImplementationDetail_SolutionHasIsNull_ReturnsFalse()
        {
            var catalogueItem = new CatalogueItem { Solution = null };
            
            var actual = catalogueItem.HasImplementationDetail();

            actual.Should().BeFalse();
        }
        
        [Test, CommonAutoData]
        public static void HasServiceLevelAgreement_SolutionHasServiceLevelAgreement_ReturnsTrue(CatalogueItem catalogueItem)
        {
            catalogueItem.Solution.ServiceLevelAgreement.Should().NotBeNullOrWhiteSpace();
            
            var actual = catalogueItem.HasServiceLevelAgreement();

            actual.Should().BeTrue();
        }

        [TestCaseSource(nameof(InvalidStrings))]
        public static void HasServiceLevelAgreement_SolutionHasInvalidServiceLevelAgreement_ReturnsFalse(string invalid)
        {
            var catalogueItem = new CatalogueItem { Solution = new Solution { ServiceLevelAgreement = invalid } };
            
            var actual = catalogueItem.HasServiceLevelAgreement();

            actual.Should().BeFalse();
        }

        [Test]
        public static void HasServiceLevelAgreement_SolutionHasIsNull_ReturnsFalse()
        {
            var catalogueItem = new CatalogueItem { Solution = null };
            
            var actual = catalogueItem.HasServiceLevelAgreement();

            actual.Should().BeFalse();
        }

        [Test, CommonAutoData]
        public static void IsFoundation_OneSolutionIsFoundation_ReturnsTrue(CatalogueItem catalogueItem)
        {
            catalogueItem.Solution.FrameworkSolutions = new List<FrameworkSolution> { new() { IsFoundation = true } };

            var actual = catalogueItem.IsFoundation();

            actual.Should().BeTrue();
        }

        [Test, CommonAutoData]
        public static void IsFoundation_NoSolutionIsFoundation_ReturnsFalse(CatalogueItem catalogueItem)
        {
            catalogueItem.Solution.FrameworkSolutions.Should().NotBeEmpty();
            catalogueItem.Solution.FrameworkSolutions.ToList().ForEach(f => f.IsFoundation = false);

            var actual = catalogueItem.IsFoundation();

            actual.Should().BeFalse();
        }

        [Test, CommonAutoData]
        public static void IsFoundation_NullFrameworkSolutions_ReturnsNull(CatalogueItem catalogueItem)
        {
            catalogueItem.Solution.FrameworkSolutions = null;

            var actual = catalogueItem.IsFoundation();

            actual.Should().BeNull();
        }

        [Test, CommonAutoData]
        public static void IsFoundation_NullSolution_ReturnsNull(CatalogueItem catalogueItem)
        {
            catalogueItem.Solution = null;

            var actual = catalogueItem.IsFoundation();

            actual.Should().BeNull();
        }

        [Test, CommonAutoData]
        public static void SecondContact_ValidModel_ReturnsSecondContact(CatalogueItem catalogueItem)
        {
            var expected = catalogueItem.Solution.MarketingContacts.Skip(1).First();

            var actual = catalogueItem.SecondContact();

            actual.Should().BeEquivalentTo(expected);
        }

        [Test, CommonAutoData]
        public static void SecondContact_NoContactsInSolution_ReturnsEmptyObject(CatalogueItem catalogueItem)
        {
            catalogueItem.Solution.MarketingContacts = null;
            var expected = new MarketingContact();

            var actual = catalogueItem.SecondContact();

            actual.Should().BeEquivalentTo(expected);
        }

        [Test, CommonAutoData]
        public static void SecondContact_NullSolution_ReturnsEmptyObject(CatalogueItem catalogueItem)
        {
            catalogueItem.Solution = null;
            var expected = new MarketingContact();

            var actual = catalogueItem.SecondContact();

            actual.Should().BeEquivalentTo(expected);
        }
    }
}
