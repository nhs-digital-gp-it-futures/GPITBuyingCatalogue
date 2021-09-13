using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Suppliers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Controllers
{
    public static class SupplierControllerTests
    {
        [Fact]
        public static async Task Get_Index_GetsAllSuppliers()
        {
            var mockSuppliersService = new Mock<ISuppliersService>();
            var controller = new SuppliersController(
                mockSuppliersService.Object);

            await controller.Index();

            mockSuppliersService.Verify(o => o.GetAllSuppliers());
        }

        [Fact]
        public static async Task Get_Index_ReturnsViewWithExpectedViewModel()
        {
            var mockSuppliers = new Mock<IList<Supplier>>().Object;
            var mockSolutionService = new Mock<ISuppliersService>();

            mockSolutionService.Setup(o => o.GetAllSuppliers()).ReturnsAsync(mockSuppliers);

            var controller = new SuppliersController(
                mockSolutionService.Object);

            var actual = (await controller.Index()).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            Assert.Same(mockSuppliers, actual.Model);
        }
    }
}
