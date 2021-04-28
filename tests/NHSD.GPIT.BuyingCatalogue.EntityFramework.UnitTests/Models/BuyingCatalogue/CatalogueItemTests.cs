using System.Linq;
using AutoFixture;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.UnitTests.Models.BuyingCatalogue
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class CatalogueItemTests
    {
        private static readonly Fixture Fixture = new();

        [Test, IgnoreCircularReferenceAutoData]
        public static void FirstContact_ValidModel_ReturnsFirstContact(CatalogueItem catalogueItem)
        {
            var expected = catalogueItem.Solution.MarketingContacts.First();

            var actual = catalogueItem.FirstContact();
            
            actual.Should().BeEquivalentTo(expected);
        }

        [Test, IgnoreCircularReferenceAutoData]
        public static void FirstContact_NoContactsInSolution_ReturnsEmptyObject(CatalogueItem catalogueItem)
        {
            catalogueItem.Solution.MarketingContacts = null;
            var expected = new MarketingContact();

            var actual = catalogueItem.FirstContact();
            
            actual.Should().BeEquivalentTo(expected);
        }
        
        [Test, IgnoreCircularReferenceAutoData]
        public static void FirstContact_NullSolution_ReturnsEmptyObject(CatalogueItem catalogueItem)
        {
            catalogueItem.Solution = null;
            var expected = new MarketingContact();

            var actual = catalogueItem.FirstContact();
            
            actual.Should().BeEquivalentTo(expected);
        }
        
        [Test, IgnoreCircularReferenceAutoData]
        public static void SecondContact_ValidModel_ReturnsSecondContact(CatalogueItem catalogueItem)
        {
            var expected = catalogueItem.Solution.MarketingContacts.Skip(1).First();

            var actual = catalogueItem.SecondContact();
            
            actual.Should().BeEquivalentTo(expected);
        }

        [Test, IgnoreCircularReferenceAutoData]
        public static void SecondContact_NoContactsInSolution_ReturnsEmptyObject(CatalogueItem catalogueItem)
        {
            catalogueItem.Solution.MarketingContacts = null;
            var expected = new MarketingContact();

            var actual = catalogueItem.SecondContact();
            
            actual.Should().BeEquivalentTo(expected);
        }
        
        [Test, IgnoreCircularReferenceAutoData]
        public static void SecondContact_NullSolution_ReturnsEmptyObject(CatalogueItem catalogueItem)
        {
            catalogueItem.Solution = null;
            var expected = new MarketingContact();

            var actual = catalogueItem.SecondContact();
            
            actual.Should().BeEquivalentTo(expected);
        }
    }
}