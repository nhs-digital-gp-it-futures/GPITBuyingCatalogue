using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Marketing;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution
{
    public sealed class Implementation : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public Implementation(LocalWebApplicationFactory factory)
            : base(factory, "/admin/catalogue-solutions/manage/99999-888/implementation")
        {
            AuthorityLogin();
        }

        [Fact]
        public async Task Implementation_TitleDisplayedCorrectly()
        {
            await using var context = GetEndToEndDbContext();
            var solutionName = (await context.CatalogueItems.SingleAsync(s => s.CatalogueItemId == new CatalogueItemId(99999, "888"))).Name;

            CommonActions.PageTitle()
                .Should()
                .BeEquivalentTo(CommonActions.FormatStringForComparison($"Implementation - {solutionName}"));
        }

        [Fact]
        public async Task Implementation_EnterDescription()
        {
            var implementation = TextGenerators.TextInputAddText(CommonSelectors.Description, 1000);

            CommonActions.ClickSave();

            await using var context = GetEndToEndDbContext();
            var solution = await context.Solutions.SingleAsync(s => s.Id == new CatalogueItemId(99999, "888"));
            solution.ImplementationDetail.Should().Be(implementation);
        }

        [Fact]
        public async Task Implementation_GoBackWithoutSaving()
        {
            TextGenerators.TextInputAddText(CommonSelectors.Description, 1000);

            AdminPages.CommonActions.ClickGoBack();

            await using var context = GetEndToEndDbContext();
            var solution = await context.Solutions.SingleAsync(s => s.Id == new CatalogueItemId(99999, "888"));
            var implementation = solution.ImplementationDetail;

            implementation.Should().BeNullOrEmpty();
        }
    }
}
