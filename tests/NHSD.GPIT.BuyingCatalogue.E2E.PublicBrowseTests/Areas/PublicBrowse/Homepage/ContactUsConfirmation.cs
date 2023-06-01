using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;
using Xunit;
using Xunit.Abstractions;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.PublicBrowse.Homepage
{
    [Collection(nameof(SharedContextCollection))]
    public class ContactUsConfirmation : AnonymousTestBase
    {
        private static readonly Dictionary<string, string> Parameters = new();

        public ContactUsConfirmation(LocalWebApplicationFactory factory, ITestOutputHelper testOutputHelper)
               : base(
                   factory,
                   typeof(HomeController),
                   nameof(HomeController.ContactUsConfirmation),
                   null,
                   testOutputHelper)
        {
        }

        [Fact]
        public void ContactUsConfirmation_AllSectionsDisplayed()
        {
            RunTest(() =>
            {
                CommonActions.PageTitle().Should().Be("Your message has been sent".FormatForComparison());
                CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            });
        }

        [Fact]
        public void ContactUsConfirmation_ClickBacklink()
        {
            RunTest(() =>
            {
                CommonActions.ClickGoBackLink();

                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(HomeController),
                    nameof(HomeController.Index))
                    .Should().BeTrue();
            });
        }
    }
}
