using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
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
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(FuturesController).Should().BeDecoratedWith<AreaAttribute>(x => x.RouteValue == "Solutions");
        }

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
                _ = new FuturesController(Mock.Of<ILogWrapper<FuturesController>>(),
                null));
        }

        [Test]
        public static void Get_Index_ReturnsDefaultView()
        {
            var mockLogger = new Mock<ILogWrapper<FuturesController>>();

            var controller = new FuturesController(mockLogger.Object,
                Mock.Of<ISolutionsService>());

            var result = controller.Index() as ViewResult;

            Assert.That(result, Is.InstanceOf(typeof(ViewResult)));
            Assert.IsNull(((ViewResult)result).ViewName);
            result.Should().NotBeNull();
            result.ViewName.Should().BeNullOrEmpty();
            mockLogger.Verify(x => x.LogTrace("Taking user to Index View"), Times.Once);            
        }
        
        [Test]
        public static void Get_Compare_ReturnsDefaultView()
        {
            var controller = new FuturesController(Mock.Of<ILogWrapper<FuturesController>>(),
                Mock.Of<ISolutionsService>());

            var result = controller.Compare() as ViewResult;

            result.Should().NotBeNull();
            result.ViewName.Should().BeNull();
        } 

        // MJRTODO - This is a better test. Need applying to all

        [Test]
        public static async Task Get_CapabilitiesSelector_ReturnsDefaultView()
        {
            var solutions = new List<Capability>
            {
                new Capability{ Name = "Item 1"},
                new Capability{ Name = "Item 2"},
                new Capability{ Name = "Item 3"},
                new Capability{ Name = "Item 4"}
            };

            var mockSolutionsService = new Mock<ISolutionsService>();
            mockSolutionsService.Setup(x => x.GetFuturesCapabilities()).ReturnsAsync(solutions);

            var controller = new FuturesController(Mock.Of<ILogWrapper<FuturesController>>(),
                mockSolutionsService.Object);

            var result = await controller.CapabilitiesSelector() as ViewResult;            
            result.Should().NotBeNull();

            var model = result.Model as CapabilitiesModel;
            model.Should().NotBeNull();
            
            Assert.AreEqual(2, model.LeftCapabilities.Length);
            Assert.AreEqual(2, model.RightCapabilities.Length);
            Assert.AreEqual("./", model.BackLink);
            Assert.AreEqual("Go back to previous page", model.BackLinkText);
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
            mockSolutionsService.Setup(x => x.GetFuturesFoundationSolutions()).ReturnsAsync(solutions);
            
            var controller = new FuturesController(Mock.Of<ILogWrapper<FuturesController>>(),
                mockSolutionsService.Object);

            var result = await controller.Foundation();

            Assert.That(result, Is.InstanceOf(typeof(ViewResult)));
            Assert.That(((ViewResult)result).Model, Is.InstanceOf(typeof(SolutionsModel)));
            Assert.AreEqual(2, ((SolutionsModel)((ViewResult)result).Model).CatalogueItems.Count);
        }
    }
}
