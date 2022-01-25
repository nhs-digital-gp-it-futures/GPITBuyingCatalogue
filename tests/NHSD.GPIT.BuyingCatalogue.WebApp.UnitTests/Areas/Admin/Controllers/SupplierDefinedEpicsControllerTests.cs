using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierDefinedEpics;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Controllers
{
    public static class SupplierDefinedEpicsControllerTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(SupplierDefinedEpicsController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Dashboard_ReturnsView(
            SupplierDefinedEpicsController controller)
        {
            var result = (await controller.Dashboard()).As<ViewResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Dashboard_ReturnsModelWithEpics(
            [Frozen] Mock<ISupplierDefinedEpicsService> supplierDefinedEpicsService,
            SupplierDefinedEpicsController controller)
        {
            var epics = new List<Epic>
            {
                new Epic
                {
                    Capability = new Capability
                    {
                        Name = "Test Capability",
                    },
                    Name = "Test Epic",
                    Id = "S00001",
                },
            };

            var expectedModel = new SupplierDefinedEpicsDashboardModel(epics);

            supplierDefinedEpicsService.Setup(s => s.GetSupplierDefinedEpics())
                .ReturnsAsync(epics);

            var result = (await controller.Dashboard()).As<ViewResult>();

            result.Should().NotBeNull();

            var model = result.Model.As<SupplierDefinedEpicsDashboardModel>();
            model.Should().NotBeNull();
            model.Should().BeEquivalentTo(expectedModel);
        }
    }
}
