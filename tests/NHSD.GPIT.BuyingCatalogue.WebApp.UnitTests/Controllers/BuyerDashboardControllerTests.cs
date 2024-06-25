using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.Services.Competitions;
using NHSD.GPIT.BuyingCatalogue.Services.Orders;
using NHSD.GPIT.BuyingCatalogue.Services.Solutions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using NSubstitute;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Controllers;

public static class BuyerDashboardControllerTests
{
    [Fact]
    public static void Constructors_VerifyGuardClauses()
    {
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        var assertion = new GuardClauseAssertion(fixture);
        var constructors = typeof(BuyerDashboardController).GetConstructors();

        assertion.Verify(constructors);
    }

    [Theory]
    [MockAutoData]
    public static async Task Index_ReturnsViewWithModel(
        Organisation organisation,
        [Frozen] IOrganisationsService organisationsService,
        [Frozen] IManageFiltersService manageFiltersService,
        [Frozen] ICompetitionsService competitionsService,
        [Frozen] IOrderService orderService,
        List<Filter> filters,
        List<Competition> competitions,
        List<Order> orders,
        BuyerDashboardController controller)
    {
        organisationsService.GetOrganisationByInternalIdentifier(Arg.Any<string>()).Returns(organisation);
        manageFiltersService.GetFilters(organisation.Id).Returns(filters);
        competitionsService.GetCompetitions(Arg.Any<string>()).Returns(competitions);
        orderService.GetOrders(organisation.Id).Returns(orders);

        var result = (await controller.Index(organisation.InternalIdentifier)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().NotBeNull();
    }
}
