using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.PublicBrowse.Solution
{
   public class ImplementationDetails : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public ImplementationDetails(LocalWebApplicationFactory factory) : base(factory, "solutions/futures/99999-001/implementation")
        {
        }

        [Theory]
        [InlineData("implementation")]
        public void ImplementationDetails_ImplementationNameDisplayed(string pageTitle)
        {
            PublicBrowsePages.SolutionAction.ImplementationNameDisplayed(pageTitle).Should().BeTrue();
        }

        [Fact]
        public async Task ImplementationDetails_VerifyContent()
        {
            using var context = GetBCContext();
            var info = (await context.Solutions.SingleAsync(s => s.Id == "99999-001")).ImplementationDetail;

            var implementationContent = PublicBrowsePages.SolutionAction.GetSummaryAndDescriptions();

            implementationContent
                .Any(s => s.Contains(info, StringComparison.CurrentCultureIgnoreCase))
                .Should().BeTrue();
        }

    }
}
