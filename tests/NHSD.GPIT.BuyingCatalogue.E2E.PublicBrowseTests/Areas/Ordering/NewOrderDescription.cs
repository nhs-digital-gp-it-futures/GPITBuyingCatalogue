using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering
{
    public sealed class NewOrderDescription
        : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public NewOrderDescription(LocalWebApplicationFactory factory)
            : base(factory, "order/organisation/03F/order/neworder/description")
        {
            BuyerLogin();
        }

        [Fact]
        public void NewOrderDescription_AllSectionsDisplayed()
        {
            CommonActions.PageTitle().Should().BeEquivalentTo("Order description");
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
            OrderingPages.OrderDescription.DescriptionInputDisplayed().Should().BeTrue();
        }

        [Fact(Skip = "Broken due to routing issue")]
        public void NewOrderDescription_NoTextThrowsError()
        {
            CommonActions.ClickSave();
            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            OrderingPages.OrderDescription.DescriptionInputShowingError("Enter a description").Should().BeTrue();
        }

        [Fact(Skip = "Broken due to routing issue")]
        public async Task NewOrderDescription_InputText_CreatesOrder()
        {
            var description = TextGenerators.TextInputAddText(Objects.Ordering.OrderDescription.DescriptionInput, 100);

            CommonActions.ClickSave();

            await using var context = GetEndToEndDbContext();
            var order = await context.Orders.OrderByDescending(o => o.Created).FirstAsync();

            order.Description.Should().Be(description);
        }
    }
}
