using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.UrlGenerators;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases
{
    public abstract class BuyerTestBase : TestBase
    {
        protected BuyerTestBase(
            LocalWebApplicationFactory factory,
            Type controller,
            string methodName,
            IDictionary<string, string> parameters)
            : base(
                  factory,
                  UrlGenerator.GenerateUrlFromMethod(controller, methodName, parameters))
        {
            BuyerLogin();
        }

        protected BuyerTestBase(
            LocalWebApplicationFactory factory,
            Type controller,
            string methodName,
            IDictionary<string, string> parameters,
            string buyerEmail)
            : base(
                  factory,
                  UrlGenerator.GenerateUrlFromMethod(controller, methodName, parameters))
        {
            BuyerLogin(buyerEmail);
        }
    }
}
