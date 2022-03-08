﻿using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.UrlGenerators;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases
{
    public abstract class AuthorityTestBase : TestBase
    {
        protected AuthorityTestBase(
            LocalWebApplicationFactory factory,
            Type controller,
            string methodName,
            IDictionary<string, string> parameters)
            : base(
                  factory,
                  UrlGenerator.GenerateUrlFromMethod(controller, methodName, parameters))
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
