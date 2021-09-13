using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin
{
    public sealed class ManageSuppliers : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public ManageSuppliers(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(SuppliersController),
                  nameof(SuppliersController.Index),
                  null)
        {
        }

        [Fact]
        public void ManageSuppliers_AddSuppliersOrgLinkDisplayed()
        {
            AdminPages.AddSolution.AddSuppliersOrgLinkDisplayed().Should().BeTrue();
        }

        [Fact]
        public void ManageSuppliers_SuppliersOrgsTableDisplayed()
        {
            AdminPages.AddSolution.SuppliersOrgTableDisplayed().Should().BeTrue();
        }

        [Fact]
        public void ManageSuppliers_SuppliersEditLinkDisplayed()
        {
            AdminPages.AddSolution.SuppliersEditLinkDisplayed().Should().BeTrue();
        }

        [Fact]
        public async Task ManageSuppliers_NumberOfSuppliersOrgsInTableAsync()
        {
            await using var context = GetEndToEndDbContext();
            var dbsuppliers = await context.Suppliers.CountAsync();

            AdminPages.AddSolution.GetNumberOfSuppliersInTable().Should().Be(dbsuppliers);
        }
    }
}
