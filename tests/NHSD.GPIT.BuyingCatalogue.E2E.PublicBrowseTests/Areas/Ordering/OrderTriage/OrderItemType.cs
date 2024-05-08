using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.OrderTriage
{
    [Collection(nameof(OrderingCollection))]
    public sealed class OrderItemType : BuyerTestBase
    {
        private const string InternalOrgId = "CG-03F";

        private static readonly Dictionary<string, string> Parameters =
            new()
            {
                { nameof(InternalOrgId), InternalOrgId },
            };

        public OrderItemType(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(OrderTriageController),
                  nameof(OrderTriageController.OrderItemType),
                  Parameters)
        {
        }

        [Fact]
        public void AllSectionsDisplayed()
        {
            using var context = GetEndToEndDbContext();
            var organisation = context.Organisations.First(o => o.InternalIdentifier == InternalOrgId);
            CommonActions.PageTitle().Should().Be($"What do you want to order? - {organisation.Name}".FormatForComparison());
            CommonActions.LedeText().Should().Be("If you want to order both, select Catalogue Solution and you'll be able to add Associated Services afterwards.".FormatForComparison());

            CommonActions.GetNumberOfRadioButtonsDisplayed().Should().Be(2);
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
        }

        [Fact]
        public void Submit_NoSelection_SetsModelError()
        {
            CommonActions.ClickSave();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(OrderItemTypeObjects.OrderItemTypeInputError, "Error: Select Catalogue Solution or Associated Service").Should().BeTrue();
        }

        [Fact]
        public void Submit_CatalogueSolution_NavigatesToCorrectPage()
        {
            CommonActions.ClickRadioButtonWithValue(CatalogueItemType.Solution.ToString());

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.ReadyToStart)).Should().BeTrue();
        }

        [Fact]
        public void Submit_AssociatedService_NavigatesToCorrectPage()
        {
            CommonActions.ClickRadioButtonWithValue(CatalogueItemType.AssociatedService.ToString());

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.ReadyToStart)).Should().BeTrue();
        }

        [Fact]
        public void ClickBacklink_NavigatesCorrectly()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DashboardController),
                nameof(DashboardController.Organisation)).Should().BeTrue();
        }
    }
}
