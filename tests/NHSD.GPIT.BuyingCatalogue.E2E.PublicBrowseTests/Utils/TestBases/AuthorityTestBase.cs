using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.UrlGenerators;
using Xunit.Abstractions;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases
{
    public abstract class AuthorityTestBase : TestBase
    {
        protected AuthorityTestBase(
            LocalWebApplicationFactory factory,
            Type controller,
            string methodName,
            IDictionary<string, string> parameters,
            ITestOutputHelper output = null)
            : base(
                  factory,
                  UrlGenerator.GenerateUrlFromMethod(controller, methodName, parameters),
                  output)
        {
            AuthorityLogin();
        }

        protected AuthorityTestBase(
            LocalWebApplicationFactory factory,
            Type controller,
            string methodName,
            IDictionary<string, string> parameters,
            IDictionary<string, string> queryParameters)
            : base(factory, UrlGenerator.GenerateUrlFromMethod(controller, methodName, parameters, queryParameters))
        {
            AuthorityLogin();
        }
    }
}
