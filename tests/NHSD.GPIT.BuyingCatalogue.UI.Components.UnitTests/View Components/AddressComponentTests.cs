using System;
using Microsoft.AspNetCore.Html;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Addresses.Models;
using NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.Components.Address;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.UnitTests.View_Components
{
    public static class AddressComponentTests
    {
        [Fact]
        public static void Address_MultipleSetProperties_ExpectedResult()
        {
            var viewComponent = new NhsAddressViewComponent
            {
                ViewComponentContext = ViewComponentContextSetup.GetViewComponentContext(),
            };

            const string expectedResult = "<p>1 John Street<br />Test Area<br />TestingTon<br />County Test<br />T3ST 3ST<br />United Kingdom<br /></p>";

            var model = new Address
            {
                Line1 = "1 John Street",
                Line5 = "Test Area",
                Town = "TestingTon",
                County = "County Test",
                Postcode = "T3ST 3ST",
                Country = "United Kingdom",
            };

            var result = viewComponent.Invoke(model);

            var resultString = result.Value;

            Assert.IsAssignableFrom<HtmlString>(result);

            Assert.Equal(expectedResult, resultString);
        }

        [Fact]
        public static void Address_NoPropertiesSet_ExpectedEmptyPTags()
        {
            var viewComponent = new NhsAddressViewComponent
            {
                ViewComponentContext = ViewComponentContextSetup.GetViewComponentContext(),
            };

            const string expectedResult = "<p></p>";

            var model = new Address();

            var result = viewComponent.Invoke(model);

            var resultString = result.Value;

            Assert.IsAssignableFrom<HtmlString>(result);

            Assert.Equal(expectedResult, resultString);
        }

        [Fact]
        public static void Address_NullAddress_ThrowsArgumentNullException()
        {
            var viewComponent = new NhsAddressViewComponent
            {
                ViewComponentContext = ViewComponentContextSetup.GetViewComponentContext(),
            };

            Assert.Throws<ArgumentNullException>(() => { _ = viewComponent.Invoke(null); });
        }
    }
}
