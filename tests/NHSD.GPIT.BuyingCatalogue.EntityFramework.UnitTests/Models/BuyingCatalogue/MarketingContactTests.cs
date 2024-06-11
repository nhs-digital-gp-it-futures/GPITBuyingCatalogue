using System;
using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NSubstitute;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.UnitTests.Models.BuyingCatalogue
{
    public static class MarketingContactTests
    {
        [Fact]
        public static void IsEmpty_AllRequiredPropertiesEmpty_ReturnsTrue()
        {
            var marketingContact = new MarketingContact();

            marketingContact.IsEmpty().Should().BeTrue();
        }

        [Theory]
        [AutoData]
        public static void IsEmpty_HasFirstName_ReturnsFalse(string firstName)
        {
            var marketingContact = new MarketingContact { FirstName = firstName };

            marketingContact.IsEmpty().Should().BeFalse();
        }

        [Theory]
        [AutoData]
        public static void IsEmpty_HasLastName_ReturnsFalse(string lastName)
        {
            var marketingContact = new MarketingContact { LastName = lastName };

            marketingContact.IsEmpty().Should().BeFalse();
        }

        [Theory]
        [AutoData]
        public static void IsEmpty_HasDepartment_ReturnsFalse(string department)
        {
            var marketingContact = new MarketingContact { Department = department };

            marketingContact.IsEmpty().Should().BeFalse();
        }

        [Theory]
        [AutoData]
        public static void IsEmpty_HasEmail_ReturnsFalse(string email)
        {
            var marketingContact = new MarketingContact { Email = email };

            marketingContact.IsEmpty().Should().BeFalse();
        }

        [Theory]
        [AutoData]
        public static void IsEmpty_HasPhoneNumber_ReturnsFalse(string phoneNumber)
        {
            var marketingContact = new MarketingContact { PhoneNumber = phoneNumber };

            marketingContact.IsEmpty().Should().BeFalse();
        }

        [Theory]
        [MockAutoData]
        public static void UpdateFrom_ValidContactInput_UpdatesProperties(MarketingContact marketingContact)
        {
            var newContact = new MarketingContact();

            newContact.UpdateFrom(marketingContact);

            newContact.Department.Should().Be(marketingContact.Department);
            newContact.Email.Should().Be(marketingContact.Email);
            newContact.FirstName.Should().Be(marketingContact.FirstName);
            newContact.LastName.Should().Be(marketingContact.LastName);
            newContact.PhoneNumber.Should().Be(marketingContact.PhoneNumber);
        }

        [Fact]
        public static void UpdateFrom_NullContactInput_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new MarketingContact().UpdateFrom(default));
        }

        [Fact]
        public static void NewAndValid_DefaultIdAndNotEmptyContact_ReturnsTrue()
        {
            var marketingContact = new MarketingContact();
            marketingContact.FirstName = marketingContact.LastName = marketingContact.Department = marketingContact.PhoneNumber = marketingContact.Email = "Test";
            marketingContact.Id = default;

            marketingContact.NewAndValid().Should().BeTrue();
        }

        [Fact]
        public static void NewAndValid_IdNotDefault_ReturnsFalse()
        {
            var marketingContact = new MarketingContact { Id = 88 };
            marketingContact.IsEmpty().Should().BeTrue();

            marketingContact.NewAndValid().Should().BeFalse();
        }

        [Fact]
        public static void NewAndValid_IdNotDefault_EmptyContact_ReturnsFalse()
        {
            var mockMarketingContact = new Mock<MarketingContact> { CallBase = true };
            mockMarketingContact.Setup(c => c.IsEmpty()).Returns(true);
            mockMarketingContact.Object.Id = 77;

            mockMarketingContact.Object.NewAndValid().Should().BeFalse();
        }
    }
}
