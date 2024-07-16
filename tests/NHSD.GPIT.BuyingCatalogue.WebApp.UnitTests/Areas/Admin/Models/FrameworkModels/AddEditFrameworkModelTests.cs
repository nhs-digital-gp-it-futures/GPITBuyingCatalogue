using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.FrameworkModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.FrameworkModels
{
    public class AddEditFrameworkModelTests
    {
        [Theory]
        [MockAutoData]
        public static void GetPageTitle_Add(
            AddEditFrameworkModel model)
        {
            model.FrameworkId = null;

            var title = model.GetPageTitle();

            title.Should().Be(AddEditFrameworkModel.AddPageTitle);
        }

        [Theory]
        [MockAutoData]
        public static void GetPageTitle_Edit(
            AddEditFrameworkModel model)
        {
            var title = model.GetPageTitle();

            title.Should().Be(AddEditFrameworkModel.EditPageTitle);
        }
    }
}
