using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Framework.UnitTests.Extensions
{
    public static class MarketingContactExtensionsTests
    {
        [Theory]
        [InlineData(null, null, null, null, null, true)]
        [InlineData("", "", "", "", "", true)]
        [InlineData(" ", " ", " ", " ", " ", true)]
        [InlineData("Bill", " ", " ", " ", " ", false)]
        [InlineData("", "Smith", " ", " ", " ", false)]
        [InlineData("", "", "Sales", " ", " ", false)]
        [InlineData("", "", "", "1234 567890", "", false)]
        [InlineData("", "", "", "", "test@test.com", false)]
        [InlineData("Bill", "Smith", "Sales", "1234 567890", "test@test.com", false)]
        public static void MarketingContactExtension_CorrectlyDeterminesIfEmpty(
            string firstName,
            string lastName,
            string department,
            string phone,
            string email,
            bool expected)
        {
            var contact = new MarketingContact
            {
                FirstName = firstName,
                LastName = lastName,
                Department = department,
                PhoneNumber = phone,
                Email = email,
            };

            var result = contact.IsEmpty();

            Assert.Equal(expected, result);
        }
    }
}
