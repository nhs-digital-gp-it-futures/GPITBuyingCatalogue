using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.UrlGenerators;
using Xunit.Abstractions;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases
{
    public abstract class BuyerTestBase : TestBase
    {
        protected BuyerTestBase(
            LocalWebApplicationFactory factory,
            Type controller,
            string methodName,
            IDictionary<string, string> parameters = null,
            ITestOutputHelper testOutputHelper = null)
            : base(
                  factory,
                  testOutputHelper,
                  UrlGenerator.GenerateUrlFromMethod(controller, methodName, parameters))
        {
            BuyerLogin();
        }

        protected BuyerTestBase(
            LocalWebApplicationFactory factory,
            Type controller,
            string methodName,
            IDictionary<string, string> parameters,
            string buyerEmail,
            ITestOutputHelper testOutputHelper = null)
            : base(
                  factory,
                  testOutputHelper,
                  UrlGenerator.GenerateUrlFromMethod(controller, methodName, parameters))
        {
            BuyerLogin(buyerEmail);
        }
    }
}
