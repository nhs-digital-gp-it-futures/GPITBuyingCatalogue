using System;
using AutoFixture;
using FluentAssertions;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.UnitTests.Models.BuyingCatalogue
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class MarketingContactTests
    {
        private static readonly Fixture Fixture = new();

        [Test]
        public static void IsEmpty_AllRequiredPropertiesEmpty_ReturnsTrue()
        {
            var marketingContact = new MarketingContact();

            marketingContact.IsEmpty().Should().BeTrue();
        }

        [Test]
        public static void IsEmpty_HasFirstName_ReturnsFalse()
        {
            var marketingContact = new MarketingContact { FirstName = Fixture.Create<string>(), };

            marketingContact.IsEmpty().Should().BeFalse();
        }

        [Test]
        public static void IsEmpty_HasLastName_ReturnsFalse()
        {
            var marketingContact = new MarketingContact { LastName = Fixture.Create<string>(), };

            marketingContact.IsEmpty().Should().BeFalse();
        }

        [Test]
        public static void IsEmpty_HasDepartment_ReturnsFalse()
        {
            var marketingContact = new MarketingContact { Department = Fixture.Create<string>(), };

            marketingContact.IsEmpty().Should().BeFalse();
        }

        [Test]
        public static void IsEmpty_HasEmail_ReturnsFalse()
        {
            var marketingContact = new MarketingContact { Email = Fixture.Create<string>(), };

            marketingContact.IsEmpty().Should().BeFalse();
        }

        [Test]
        public static void IsEmpty_HasPhoneNumber_ReturnsFalse()
        {
            var marketingContact = new MarketingContact { PhoneNumber = Fixture.Create<string>(), };

            marketingContact.IsEmpty().Should().BeFalse();
        }

        [Test, CommonAutoData]
        public static void UpdateFrom_ValidContactInput_UpdatesProperties(MarketingContact marketingContact)
        {
            var newContact = new MarketingContact();

            newContact.UpdateFrom(marketingContact);

            newContact.Department.Should().Be(marketingContact.Department);
            newContact.Email.Should().Be(marketingContact.Email);
            newContact.FirstName.Should().Be(marketingContact.FirstName);
            newContact.LastName.Should().Be(marketingContact.LastName);
            newContact.LastUpdated.Should().BeCloseTo(marketingContact.LastUpdated);
            newContact.PhoneNumber.Should().Be(marketingContact.PhoneNumber);
        }

        [Test]
        public static void UpdateFrom_NullContactInput_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new MarketingContact().UpdateFrom(default));
        }

        [Test]
        public static void NewAndValid_DefaultIdAndNotEmptyContact_ReturnsTrue()
        {
            var mockMarketingContact = new Mock<MarketingContact> { CallBase = true, };
            mockMarketingContact.Setup(x => x.IsEmpty())
                .Returns(false);
            mockMarketingContact.Object.Id.Should().Be(default);

            mockMarketingContact.Object.NewAndValid().Should().BeTrue();
        }

        [Test]
        public static void NewAndValid_IdNotDefault_ReturnsFalse()
        {
            var marketingContact = new MarketingContact { Id = 88 };
            marketingContact.IsEmpty().Should().BeTrue();

            marketingContact.NewAndValid().Should().BeFalse();
        }

        [Test]
        public static void NewAndValid_IdNotDefault_EmptyContact_ReturnsFalse()
        {
            var mockMarketingContact = new Mock<MarketingContact> { CallBase = true, };
            mockMarketingContact.Setup(x => x.IsEmpty())
                .Returns(true);
            mockMarketingContact.Object.Id = 77;

            mockMarketingContact.Object.NewAndValid().Should().BeFalse();
        }
    }
}
