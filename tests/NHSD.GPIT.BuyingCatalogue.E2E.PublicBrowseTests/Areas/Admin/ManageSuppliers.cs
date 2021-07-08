using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin
{
    public class ManageSuppliers : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public ManageSuppliers(LocalWebApplicationFactory factory) : base(factory, "admin/manage-suppliers")
        {
            Login();
        }

        [Fact(Skip = "Data test id not displaying on vc:nhs-action-link")]
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
