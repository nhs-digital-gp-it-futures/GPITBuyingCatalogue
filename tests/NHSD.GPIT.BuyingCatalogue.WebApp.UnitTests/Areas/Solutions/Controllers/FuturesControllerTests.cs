using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Controllers
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class FuturesControllerTests
    {
        [Test]
        public static void Constructor_NullLogging_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new FuturesController(null,
                Mock.Of<ISolutionsService>()));
        }

        [Test]
        public static void Constructor_NullSolutionsService_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new FuturesController(Mock.Of<ILogger<FuturesController>>(),
                null));
        }

        [Test]
        public static void Get_Index_ReturnsDefaultView()
        {
            var controller = new FuturesController(Mock.Of<ILogger<FuturesController>>(),
                Mock.Of<ISolutionsService>());

            var result = controller.Index();
            
            Assert.That(result, Is.InstanceOf(typeof(ViewResult)));
            Assert.IsNull(((ViewResult)result).ViewName);
        }

        [Test]
        public static void Get_Compare_ReturnsDefaultView()
        {
            var controller = new FuturesController(Mock.Of<ILogger<FuturesController>>(),
                Mock.Of<ISolutionsService>());

            var result = controller.Compare();

            Assert.That(result, Is.InstanceOf(typeof(ViewResult)));
            Assert.IsNull(((ViewResult)result).ViewName);
        }

        [Test]
        public static void Get_CapabilitiesSelector_ReturnsDefaultView()
        {
            var controller = new FuturesController(Mock.Of<ILogger<FuturesController>>(),
                Mock.Of<ISolutionsService>());

            var result = controller.CapabilitiesSelector();

            Assert.That(result, Is.InstanceOf(typeof(ViewResult)));
            Assert.IsNull(((ViewResult)result).ViewName);
        }

        [Test]
        public static async Task Get_Foundation_ReturnsDefaultViewWithModelPopulated()
        {
            var solutions = new List<CatalogueItem>
            {
                new CatalogueItem{ Name = "Item 1"},
                new CatalogueItem{ Name = "Item 2"}
            };

            var mockSolutionsService = new Mock<ISolutionsService>();
            mockSolutionsService.Setup(x => x.GetFoundationSolutions()).ReturnsAsync(solutions);
            
            var controller = new FuturesController(Mock.Of<ILogger<FuturesController>>(),
                mockSolutionsService.Object);

            var result = await controller.Foundation();

            Assert.That(result, Is.InstanceOf(typeof(ViewResult)));
            Assert.That(((ViewResult)result).Model, Is.InstanceOf(typeof(SolutionsModel)));
            Assert.AreEqual(2, ((SolutionsModel)((ViewResult)result).Model).Solutions.Count);
        }
    }
}
