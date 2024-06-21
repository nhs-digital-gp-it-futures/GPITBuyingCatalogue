using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.Components.ActionLink;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.UnitTests.View_Components
{
    public static class ActionLinkComponentTests
    {
        [Theory]
        [MockAutoData]
        public static async Task ActionLink_ExpectedResult(string url, string text)
        {
            var viewComponent = new NhsActionLinkViewComponent
            {
                ViewComponentContext = ViewComponentContextSetup.GetViewComponentContext(),
            };

            var model = await viewComponent.InvokeAsync(url, text) as ViewViewComponentResult;
            model.ViewName.Should().Be("ActionLink");

            var modelData = model.ViewData.Model as ActionLinkModel;
            modelData.Url.Should().Be(url);
            modelData.Text.Should().Be(text);
        }
    }
}
