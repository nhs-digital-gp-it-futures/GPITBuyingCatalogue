using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.ManageOrders;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.ManageOrders
{
    [Collection(nameof(AdminCollection))]
    public sealed class DeleteNotLatest
        : AuthorityTestBase
    {
        private const string InternalOrgId = "CG-03F";
        private static readonly CallOffId CallOffId = new(90009, 1);

        private static readonly Dictionary<string, string> Parameters =
            new()
            {
                { nameof(InternalOrgId), InternalOrgId },
                { nameof(CallOffId), CallOffId.ToString() },
            };

        public DeleteNotLatest(LocalWebApplicationFactory factory)
        : base(
              factory,
              typeof(ManageOrdersController),
              nameof(ManageOrdersController.DeleteNotLatest),
              Parameters)
        {
        }

        [Fact]
        public void DeleteNotLatest_AllSectionsDisplayed()
        {
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(DeleteNotLatestObjects.ReturnToOrdersDashboard);
        }

        [Fact]
        public void DeleteNotLatest_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions
            .PageLoadedCorrectGetIndex(
                typeof(ManageOrdersController),
                nameof(ManageOrdersController.ViewOrder))
            .Should()
            .BeTrue();
        }

        [Fact]
        public void DeleteNotLatest_ClickReturnToDashboard_ExpectedResult()
        {
            CommonActions.ClickLinkElement(DeleteNotLatestObjects.ReturnToOrdersDashboard);

            CommonActions
            .PageLoadedCorrectGetIndex(
                typeof(ManageOrdersController),
                nameof(ManageOrdersController.Index))
            .Should()
            .BeTrue();
        }
    }
}
