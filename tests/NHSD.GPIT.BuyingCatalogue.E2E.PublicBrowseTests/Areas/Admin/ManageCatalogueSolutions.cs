using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin
{
    public sealed class ManageCatalogueSolutions : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public ManageCatalogueSolutions(LocalWebApplicationFactory factory)
            : base(factory, "admin/catalogue-solutions")
        {
            AuthorityLogin();
        }

        [Fact]
        public void ManageCatalogueSolutions_SolutionTableDisplayed()
        {
            AdminPages.AddSolution.CatalogueSolutionTableDisplayed().Should().BeTrue();
        }

        [Fact]
        public void ManageCatalogueSolutions_FilterRadioButtonsDisplayed()
        {
            AdminPages.AddSolution.NumberOfFilterRadioButtonsDisplayed().Should().Be(5);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public async Task ManageCatalogueSolutions_FilteredCatalogueSolutionsDisplayedAsync(int index)
        {
            var publicationStatus = AdminPages.AddSolution.FilterCatalogueSolutions(index);

            await using var context = GetEndToEndDbContext();
            var dbSolutions = await context.CatalogueItems
                                            .Where(c => c.PublishedStatus == publicationStatus)
                                            .Where(c => c.CatalogueItemType == CatalogueItemType.Solution)
                                            .ToListAsync();

            AdminPages.AddSolution.GetNumberOfItemsInTable().Should().Be(dbSolutions.Count);
        }
    }
}
