using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin
{
    public class ManageCatalogueSolutions : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public ManageCatalogueSolutions(LocalWebApplicationFactory factory) : base(factory, "admin")
        {
            Login();
        }

        [Fact]
        public void ManageCatalogueSolutions_AddSolutionLinkDisplayed()
        {
            AdminPages.AddSolution.AddSolutionLinkDisplayed().Should().BeTrue();
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
                                            .Where(c => c.SupplierId == "99999")
                                            .Where(c => c.PublishedStatus == publicationStatus)
                                            // .Where(c => c.CatalogueItemType == CatalogueItemType.Solution) ***awaiting bug fix***
                                            .ToListAsync();

            AdminPages.AddSolution.GetNumberOfItemsInTable().Should().Be(dbSolutions.Count);
        }
    }
}
