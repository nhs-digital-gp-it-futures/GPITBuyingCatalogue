using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin
{
    public class AddSolution : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public AddSolution(LocalWebApplicationFactory factory) : base(factory, "admin/catalogue-solutions/add-solution")
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
    }
}
