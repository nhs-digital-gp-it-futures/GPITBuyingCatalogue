﻿namespace NHSD.GPIT.BuyingCatalogue.UI.Components.UnitTests.View_Components
{
    using System;
    using Microsoft.AspNetCore.Html;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewComponents;
    using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
    using NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.Components.ActionLink;
    using NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.Components.Address;
    using NUnit.Framework;
    using SparkyTestHelpers.AspNetMvc.Core;

    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal class ViewComponentUnitTests
    {
        private ViewComponentContext viewComponentContext;

        [OneTimeSetUp]
        public void ViewComponentTestSetup()
        {
            var httpContext = new DefaultHttpContext();

            var viewContext = new ViewContext { HttpContext = httpContext };
            viewComponentContext = new ViewComponentContext { ViewContext = viewContext };
        }

        [OneTimeTearDown]
        public void ViewComponentTestTearDown()
        {
            viewComponentContext = null;
        }

        [Test]
        [TestCase("hello/world", "This is a test link")]
        [TestCase("", "This is a test link")]
        [TestCase("hello/world", "")]
        [TestCase("https://hello.world/a/subpage", "AHttpTestLink")]
        [TestCase("", "")]
        public void ActionLink_ExpectedResult(string url, string text)
        {
            var viewComponent = new NhsActionLinkViewComponent { ViewComponentContext = viewComponentContext };

            _ = new ViewComponentTester<NhsActionLinkViewComponent>(viewComponent)
                .Invocation(x => () => x.InvokeAsync(url, text))
                .ExpectingViewName("ActionLink")
                .ExpectingModel<ActionLinkModel>(model =>
                {
                    Assert.AreEqual(url, model.Url);
                    Assert.AreEqual(text, model.Text);
                })
                .TestView();
        }

        [Test]
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

            Assert.IsInstanceOf<HtmlString>(result);

            Assert.AreEqual(expectedResult, resultString);
        }

        [Test]
        public void Address_NoPropertiesSet_ExpectedEmptyPTags()
        {
            var viewComponent = new NhsAddressViewComponent { ViewComponentContext = viewComponentContext };

            const string expectedResult = "<p></p>";

            var model = new Address();

            var result = viewComponent.Invoke(model);

            var resultString = result.Value;

            Assert.IsInstanceOf<HtmlString>(result);

            Assert.AreEqual(expectedResult, resultString);
        }

        [Test]
        public void Address_NullAddress_ThrowsArgumentNullException()
        {
            var viewComponent = new NhsAddressViewComponent { ViewComponentContext = viewComponentContext };

            Assert.Throws<ArgumentNullException>(() => { _ = viewComponent.Invoke(null); });
        }
    }
}
