using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Models;

public static class BuyerDashboardModelTests
{
    [Theory]
    [CommonInlineAutoData(true, BuyerDashboardModel.AccountManagerAdvice)]
    [CommonInlineAutoData(false, BuyerDashboardModel.BuyerAdvice)]
    public static void Construct_SetsAdvice(
        bool isAccountManager,
        string expectedAdvice,
        string organisationName,
        string organisationId)
    {
        var model = new BuyerDashboardModel(organisationId, organisationName, isAccountManager);

        model.Advice.Should().Be(expectedAdvice);
    }
}
