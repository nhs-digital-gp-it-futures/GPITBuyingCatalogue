using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.Framework.UnitTests.Extensions
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class MarketingContactExtensionsTests
    {
        [Test]
        [TestCase(null, null, null, null, null, true)]
        [TestCase("", "", "", "", "", true)]
        [TestCase(" ", " ", " ", " ", " ", true)]
        [TestCase("Bill", " ", " ", " ", " ", false)]
        [TestCase("", "Smith", " ", " ", " ", false)]
        [TestCase("", "", "Sales", " ", " ", false)]
        [TestCase("", "", "", "1234 567890", "", false)]
        [TestCase("", "", "", "", "test@test.com", false)]
        [TestCase("Bill", "Smith", "Sales", "1234 567890", "test@test.com", false)]
        public static void MarketingContactExtension_CorrectlyDeterminesIfEmpty(string firstName, string lastName, string department, string phone, string email, bool expected)
        {
            var contact = new MarketingContact
            {
                FirstName = firstName,
                LastName = lastName,
                Department = department,
                PhoneNumber = phone,
                Email = email
            };

            var result = contact.IsEmpty();

            Assert.AreEqual(expected, result);
        }
    }
}
