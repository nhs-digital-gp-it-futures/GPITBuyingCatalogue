using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Moq;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Contracts;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.Contracts;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.ImplementationPlans;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Controllers.Contracts
{
    public static class ImplementationPlanControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(ImplementationPlanController).Should().BeDecoratedWith<AuthorizeAttribute>();
            typeof(ImplementationPlanController).Should().BeDecoratedWith<AreaAttribute>(a => a.RouteValue == "Orders");
        }

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(ImplementationPlanController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Index_ReturnsExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            ContractFlags contract,
            CatalogueItem catalogueItem,
            ImplementationPlan defaultPlan,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<IContractsService> mockContractsService,
            [Frozen] Mock<ISolutionsService> mockSolutionsService,
            [Frozen] Mock<IImplementationPlanService> mockImplementationPlanService,
            ImplementationPlanController controller)
        {
            var solution = order.OrderItems.First();

            contract.UseDefaultImplementationPlan = null;
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService);
            solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            mockOrderService
                .Setup(s => s.GetOrderThin(order.CallOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            mockContractsService
                .Setup(x => x.GetContractFlags(order.Id))
                .ReturnsAsync(contract);

            mockSolutionsService
                .Setup(x => x.GetSolutionThin(solution.CatalogueItemId))
                .ReturnsAsync(catalogueItem);

            mockImplementationPlanService
                .Setup(x => x.GetDefaultImplementationPlan())
                .ReturnsAsync(defaultPlan);

            var result = await controller.Index(internalOrgId, order.CallOffId);

            mockOrderService.VerifyAll();
            mockContractsService.VerifyAll();
            mockSolutionsService.VerifyAll();
            mockImplementationPlanService.VerifyAll();

            var expected = new DefaultImplementationPlanModel
            {
                CallOffId = order.CallOffId,
                Solution = catalogueItem.Solution,
                Plan = defaultPlan,
                UseDefaultMilestones = null,
            };

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink));
        }

    }
}
