using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
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
    public sealed class NewOrderDescription
        : BuyerTestBase
    {
        private const string InternalOrgId = "IB-QWO";
        private const OrderTriageValue Option = OrderTriageValue.Over250K;

        private static readonly Dictionary<string, string> Parameters =
            new()
            {
                { nameof(InternalOrgId), InternalOrgId },
            };

        private static readonly Dictionary<string, string> QueryParameters =
            new() { { nameof(Option), Option.ToString() }, };

        public NewOrderDescription(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(OrderDescriptionController),
                  nameof(OrderDescriptionController.NewOrderDescription),
                  Parameters,
                  queryParameters: QueryParameters)
        {
        }

        [Fact]
        public void NewOrderDescription_AllSectionsDisplayed()
        {
            CommonActions.PageTitle().Should().BeEquivalentTo("Order description - NHS Humber and North Yorkshire ICB - 03F".FormatForComparison());
            CommonActions.LedeText().Should().BeEquivalentTo(OrderDescriptionModel.NewOrderAdviceText.FormatForComparison());
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Ordering.OrderDescription.DescriptionInput).Should().BeTrue();
        }

        [Fact]
        public void NewOrderDescription_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                  typeof(OrderController),
                  nameof(OrderController.NewOrder))
                    .Should().BeTrue();
        }

        [Fact]
        public void NewOrderDescription_NoTextThrowsError()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderDescriptionController),
                nameof(OrderDescriptionController.NewOrderDescription))
                    .Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage("Description", "Enter an order description")
                .Should().BeTrue();
        }

        [Fact]
        public async Task NewOrderDescription_InputText_CreatesOrder()
        {
            var description = TextGenerators.TextInputAddText(Objects.Ordering.OrderDescription.DescriptionInput, 100);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Order)).Should().BeTrue();

            await using var context = GetEndToEndDbContext();
            var order = await context.Orders.OrderByDescending(o => o.Created).FirstAsync();

            order.Description.Should().Be(description);
        }
    }
}
