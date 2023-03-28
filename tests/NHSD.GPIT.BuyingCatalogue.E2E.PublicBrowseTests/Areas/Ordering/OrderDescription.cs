using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.OrderDescription;
using Xunit;
using Objects = NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering
{
    [Collection(nameof(OrderingCollection))]
    public sealed class OrderDescription
        : BuyerTestBase
    {
        private const string InternalOrgId = "IB-QWO";
        private static readonly CallOffId CallOffId = new(90001, 1);

        private static readonly Dictionary<string, string> Parameters =
            new()
            {
                { nameof(InternalOrgId), InternalOrgId },
                { nameof(CallOffId), CallOffId.ToString() },
            };

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
            CommonActions.PageTitle().Should().BeEquivalentTo($"Order description - Order {CallOffId}".FormatForComparison());
            CommonActions.LedeText().Should().BeEquivalentTo(OrderDescriptionModel.AdviceText.FormatForComparison());
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Ordering.OrderDescription.DescriptionInput).Should().BeTrue();

            var order = await GetEndToEndDbContext().Order(CallOffId);

            CommonActions.InputValueEqualTo(Objects.Ordering.OrderDescription.DescriptionInput, order.Description);
        }

        [Fact]
        public void OrderDescription_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Order)).Should().BeTrue();
        }

        [Fact]
        public void OrderDescription_DeletingAndSaving_ShouldError()
        {
            CommonActions.ClearInputElement(Objects.Ordering.OrderDescription.DescriptionInput);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderDescriptionController),
                nameof(OrderDescriptionController.OrderDescription)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage("Description", "Enter an order description")
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

            var order = await GetEndToEndDbContext().Order(CallOffId);

            order.Description.Should().BeEquivalentTo(description);
        }
    }
}
