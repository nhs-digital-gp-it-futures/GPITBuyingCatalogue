using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Marketing;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution
{
    public sealed class Description : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public Description(LocalWebApplicationFactory factory)
            : base(factory, "/admin/catalogue-solutions/manage/99999-888/description")
        {
            Login();
        }

        [Fact]
        public async Task Description_TitleDisplayedCorrectly()
        {
            await using var context = GetEndToEndDbContext();
            var solutionName = (await context.CatalogueItems.SingleAsync(s => s.CatalogueItemId == new CatalogueItemId(99999, "888"))).Name;

            PublicBrowsePages.CommonActions.PageTitle().Should().BeEquivalentTo($"Description - {solutionName}");
        }

        [Fact]
        public async Task Description_EnterAllFieldsAsync()
        {
            var summary = TextGenerators.TextInputAddText(CommonSelectors.Summary, 350);
            var fullDescription = TextGenerators.TextInputAddText(CommonSelectors.Description, 1000);
            var link = TextGenerators.UrlInputAddText(CommonSelectors.Link, 1000);

            CommonActions.ClickSave();

            await using var context = GetEndToEndDbContext();
            var solution = await context.Solutions.SingleAsync(s => s.Id == new CatalogueItemId(99999, "888"));
            solution.Summary.Should().Be(summary);
            solution.FullDescription.Should().Be(fullDescription);
            solution.AboutUrl.Should().Be(link);
        }

        [Fact]
        public async Task Description_GoBackWithoutSaving()
        {
            await using var context = GetEndToEndDbContext();
            var solution = await context.Solutions.SingleAsync(s => s.Id == new CatalogueItemId(99999, "888"));
            solution.FullDescription = string.Empty;
            solution.AboutUrl = string.Empty;

            await context.SaveChangesAsync();

            TextGenerators.TextInputAddText(CommonSelectors.Description, 1000);
            TextGenerators.UrlInputAddText(CommonSelectors.Link, 1000);

            AdminPages.CommonActions.ClickGoBack();

            var fullDescription = solution.FullDescription;
            var link = solution.AboutUrl;

            fullDescription.Should().BeNullOrEmpty();
            link.Should().BeNullOrEmpty();
        }
    }
}
