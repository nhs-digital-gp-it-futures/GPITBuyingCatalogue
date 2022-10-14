using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.UrlGenerators;
using Xunit.Abstractions;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases
{
    public abstract class AccountManagerTestBase : TestBase
    {
        protected AccountManagerTestBase(
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
            AccountManagerLogin();
        }

        protected AccountManagerTestBase(
            LocalWebApplicationFactory factory,
            Type controller,
            string methodName,
            IDictionary<string, string> parameters,
            IDictionary<string, string> queryParameters,
            ITestOutputHelper testOutputHelper = null)
            : base(
                factory,
                testOutputHelper,
                UrlGenerator.GenerateUrlFromMethod(controller, methodName, parameters, queryParameters))
        {
            AccountManagerLogin();
        }
    }
}
