using System;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Addresses.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations;
using NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.Components.ActionLink;
using NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.Components.Address;
using SparkyTestHelpers.AspNetMvc.Core;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.UnitTests.View_Components
{
    public sealed class ViewComponentUnitTests : IDisposable
    {
        private ViewComponentContext viewComponentContext;

        public ViewComponentUnitTests()
        {
            var httpContext = new DefaultHttpContext();

            var viewContext = new ViewContext { HttpContext = httpContext };
            viewComponentContext = new ViewComponentContext { ViewContext = viewContext };
        }

        public void Dispose()
        {
            viewComponentContext = null;
        }

        [Theory]
        [InlineData("hello/world", "This is a test link")]
        [InlineData("", "This is a test link")]
        [InlineData("hello/world", "")]
        [InlineData("https://hello.world/a/subpage", "AHttpTestLink")]
        [InlineData("", "")]
        public void ActionLink_ExpectedResult(string url, string text)
        {
            var viewComponent = new NhsActionLinkViewComponent { ViewComponentContext = viewComponentContext };

            _ = new ViewComponentTester<NhsActionLinkViewComponent>(viewComponent)
                .Invocation(x => () => x.InvokeAsync(url, text))
                .ExpectingViewName("ActionLink")
                .ExpectingModel<ActionLinkModel>(model =>
                {
                    Assert.Equal(url, model.Url);
                    Assert.Equal(text, model.Text);
                })
                .TestView();
        }

        [Fact]
        public void Address_MultipleSetProperties_ExpectedResult()
        {
            var viewComponent = new NhsAddressViewComponent { ViewComponentContext = viewComponentContext };

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
        public void Address_NoPropertiesSet_ExpectedEmptyPTags()
        {
            var viewComponent = new NhsAddressViewComponent { ViewComponentContext = viewComponentContext };

            const string expectedResult = "<p></p>";

            var model = new Address();

            var result = viewComponent.Invoke(model);

            var resultString = result.Value;

            Assert.IsAssignableFrom<HtmlString>(result);

            Assert.Equal(expectedResult, resultString);
        }

        [Fact]
        public void Address_NullAddress_ThrowsArgumentNullException()
        {
            var viewComponent = new NhsAddressViewComponent { ViewComponentContext = viewComponentContext };

            Assert.Throws<ArgumentNullException>(() => { _ = viewComponent.Invoke(null); });
        }
    }
}
