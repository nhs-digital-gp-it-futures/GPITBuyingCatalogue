using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using FluentAssertions;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.NativeDesktop;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Models.NativeDesktop
{
    public static class AdditionalInformationModelTests
    {
        [Fact]
        public static void AdditionalInformation_StringLengthAttribute_ExpectedValue()
        {
            typeof(AdditionalInformationModel)
                .GetProperty(nameof(AdditionalInformationModel.AdditionalInformation))
                .GetCustomAttribute<StringLengthAttribute>()
                .MaximumLength.Should().Be(500);
        }

        [Fact]
        public static void Constructor_NullCatalogueItem_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new AdditionalInformationModel(null));
        }

        [Fact]
        public static void WithCatalogueItem_PropertiesCorrectlySet()
        {
            var clientApplication = new ClientApplication { NativeDesktopAdditionalInformation = "Some additional information" };

            var json = JsonConvert.SerializeObject(clientApplication);
            var catalogueItem = new CatalogueItem
            {
                CatalogueItemId = new CatalogueItemId(1, "123"),
                Solution = new Solution { ClientApplication = json },
            };

            var model = new AdditionalInformationModel(catalogueItem);

            Assert.Equal("/marketing/supplier/solution/1-123/section/native-desktop", model.BackLink);
            Assert.Equal("Some additional information", model.AdditionalInformation);
        }

        [Fact]
        public static void WithoutCatalogueItem_PropertiesAreDefaulted()
        {
            var model = new AdditionalInformationModel();

            Assert.Equal("./", model.BackLink);
            Assert.False(model.IsComplete);
            Assert.Null(model.AdditionalInformation);
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData(" ", false)]
        [InlineData("Some additional information", true)]
        public static void IsCompleteIsCorrectlySet(string additionalInformation, bool? expected)
        {
            var clientApplication = new ClientApplication { NativeDesktopAdditionalInformation = additionalInformation };
            var json = JsonConvert.SerializeObject(clientApplication);
            var catalogueItem = new CatalogueItem { Solution = new Solution { ClientApplication = json } };

            var model = new AdditionalInformationModel(catalogueItem);

            Assert.Equal(expected, model.IsComplete);
        }
    }
}
