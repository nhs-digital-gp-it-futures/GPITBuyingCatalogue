﻿using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Controllers
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class OrganisationsControllerTests
    {
        [Test]
        public static void Constructor_NullLogging_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new OrganisationsController(null));
        }

        [Test]
        public static void Get_Index_ReturnsViewResult()
        {
            var controller = new OrganisationsController(Mock.Of<ILogger<OrganisationsController>>());

            var result = controller.Index();
            
            Assert.That(result, Is.InstanceOf(typeof(ViewResult)));
        }
    }
}
