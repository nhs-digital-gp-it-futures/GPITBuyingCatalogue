using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution
{
    public sealed class AddSolution : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public AddSolution(LocalWebApplicationFactory factory)
            : base(factory, "admin/catalogue-solutions/add-solution")
        {
            Login();
        }

        [Fact]
        public void AddSolution_AddSolutionSectionDisplayed()
        {
            AdminPages.AddSolution.SolutionNameFieldDisplayed().Should().BeTrue();
            AdminPages.AddSolution.SupplierNameFieldDisplayed().Should().BeTrue();
            AdminPages.AddSolution.SaveSolutionButtonDisplayed().Should().BeTrue();
        }

        [Fact]
        public void AddSolution_FrameworksDisplayed()
        {
            AdminPages.AddSolution.FrameworkNamesDisplayed().Should().BeTrue();
        }

        [Fact]
        public void AddSolution_FoundationSolutionDisplayed()
        {
            AdminPages.AddSolution.FoundationSolutionDisplayed().Should().BeTrue();
        }

        [Fact]
        public async Task AddSolution_SaveSolution()
        {
            var solutionName = $"TestSolution {DateTime.UtcNow}";

            AdminPages.AddSolution.EnterSolutionName(solutionName);

            AdminPages.AddSolution.SelectSupplier("99999");

            AdminPages.AddSolution.CheckFrameworkByIndex(0);

            AdminPages.AddSolution.ClickSaveButton();

            await using var context = GetEndToEndDbContext();

            var dbSolution = await context.CatalogueItems
                .Include(c => c.Solution)
                .Include(c => c.Supplier)
                .SingleAsync(c => c.Name == solutionName);

            dbSolution.Supplier.Id.Should().Be("99999");
            dbSolution.PublishedStatus.Should().Be(PublicationStatus.Draft);
        }
    }
}
