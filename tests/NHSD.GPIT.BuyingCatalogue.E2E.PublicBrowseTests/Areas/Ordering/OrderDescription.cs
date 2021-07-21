using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering
{
    public sealed class OrderDescription
        : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private static readonly CallOffId CallOffId = new(90001, 1);

        private static readonly Dictionary<string, string> Parameters = new() { { "OdsCode", "03F" }, { "CallOffId", CallOffId.ToString() } };

        public OrderDescription(LocalWebApplicationFactory factory)
            : base(
                 factory,
                 typeof(OrderDescriptionController),
                 nameof(OrderDescriptionController.OrderDescription),
                 Parameters)
        {
        }

        [Fact]
        public async Task OrderDescription_AllSectionsDisplayed()
        {
            CommonActions.PageTitle().Should().BeEquivalentTo(CommonActions.FormatStringForComparison("Order description"));
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Ordering.OrderDescription.DescriptionInput).Should().BeTrue();

            await using var context = GetEndToEndDbContext();
            var order = await context.Orders.SingleAsync(o => o.Id == CallOffId.Id);

            CommonActions.InputValueEqualToo(Objects.Ordering.OrderDescription.DescriptionInput, order.Description);
        }

        [Fact]
        public void OrderDescription_DeletingAndSaving_ShouldError()
        {
            CommonActions.ClearInputElement(Objects.Ordering.OrderDescription.DescriptionInput);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderDescriptionController),
                nameof(OrderDescriptionController.OrderDescription));

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage("Description", "Enter a description")
                .Should().BeTrue();
        }

        [Fact]
        public async Task OrderDescription_InputText_UpdatesDescription()
        {
            var description = TextGenerators.TextInputAddText(Objects.Ordering.OrderDescription.DescriptionInput, 100);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Order)).Should().BeTrue();

            await using var context = GetEndToEndDbContext();
            var order = await context.Orders.SingleAsync(o => o.Id == CallOffId.Id);

            order.Description.Should().BeEquivalentTo(description);
        }
    }
}
