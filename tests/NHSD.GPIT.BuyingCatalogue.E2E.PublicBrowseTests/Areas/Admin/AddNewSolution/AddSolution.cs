using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;
using Objects = NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution
{
    public sealed class AddSolution : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public AddSolution(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(AddCatalogueSolutionController),
                  nameof(AddCatalogueSolutionController.Index),
                  null)
        {
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
            CommonActions.ClickCheckboxByLabel("GP IT Futures Framework");
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

            dbSolution.Supplier.Id.Should().Be(99999);
            dbSolution.PublishedStatus.Should().Be(PublicationStatus.Draft);
        }

        [Fact]
        public void AddSolution_SolutionModelStateInvalid_ThrowsErrors()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AddCatalogueSolutionController),
                nameof(AddCatalogueSolutionController.Index)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementIsDisplayed(AddSolutionObjects.SolutionNameError).Should().BeTrue();
            CommonActions.ElementIsDisplayed(AddSolutionObjects.SupplierNameError).Should().BeTrue();
            CommonActions.ElementIsDisplayed(AddSolutionObjects.SolutionFrameworkError).Should().BeTrue();
        }
    }
}
