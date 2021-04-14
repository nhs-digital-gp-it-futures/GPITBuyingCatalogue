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
    internal static class DFOCVCControllerTests
    {
        [Test]
        public static void Constructor_NullLogging_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new DFOCVCController(null,
                Mock.Of<ISolutionsService>()));
        }

        [Test]
        public static void Constructor_NullSolutionsService_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new DFOCVCController(Mock.Of<ILogger<DFOCVCController>>(),
                null));
        }

        [Test]
        public static async Task Get_Index_ReturnsDefaultViewWithModelPopulated()
        {
            var solutions = new List<CatalogueItem>
            {
                new CatalogueItem{ Name = "Item 1"},
                new CatalogueItem{ Name = "Item 2"}
            };

            var mockSolutionsService = new Mock<ISolutionsService>();
            mockSolutionsService.Setup(x => x.GetDFOCVCSolutions()).ReturnsAsync(solutions);

            var controller = new DFOCVCController(Mock.Of<ILogger<DFOCVCController>>(),
                mockSolutionsService.Object);

            var result = await controller.Index();

            Assert.That(result, Is.InstanceOf(typeof(ViewResult)));
            Assert.That(((ViewResult)result).Model, Is.InstanceOf(typeof(SolutionsModel)));
            Assert.AreEqual(2, ((SolutionsModel)((ViewResult)result).Model).Solutions.Count);
        }
    }
}
