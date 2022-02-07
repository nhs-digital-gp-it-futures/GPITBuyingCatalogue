using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Objects;
using NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Utils;
using NHSD.GPIT.BuyingCatalogue.UI.Components.WebApp.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Tests
{
    public sealed class DoAndDontListsTest : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public DoAndDontListsTest(LocalWebApplicationFactory factory)

             : base(factory,
                        typeof(HomeController),
                        nameof(HomeController.DoAndDontList),
                        null)
        {
        }

        [Fact]
        public void DoAndDontLists_GetDoAndDontListsHeader1()
        {
            CommonActions.GetElementText(CommonObject.Header1).
               Contains("Do and Dont list")
               .Should().BeTrue();
        }

        [Fact]
        public void DoAndDontLists_GetDoHeader2()
        {
            CommonActions.GetElementText(DoAndDontListsObjects.DoList).
               Contains("Do")
               .Should().BeTrue();
        }

        [Fact]
        public void DoAndDontLists_GetDontListsHeader2()
        {
            CommonActions.GetElementText(DoAndDontListsObjects.DontList).
               Contains("Don't")
               .Should().BeTrue();
        }
    }
}
