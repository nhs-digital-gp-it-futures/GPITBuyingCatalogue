using System;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Objects;
using NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Utils;
using NHSD.GPIT.BuyingCatalogue.UI.Components.WebApp.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Tests
{
    public sealed class DateInputTest : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public DateInputTest(LocalWebApplicationFactory factory)

             : base(factory,
                        typeof(HomeController),
                        nameof(HomeController.DateInput),
                        null)
        {
        }

        [Fact]
        public void DateInput_VerifyThatDateInputPageIsLoaded()
        {
            CommonActions.PageLoadedCorrectGetIndex(
                 typeof(HomeController),
                 nameof(HomeController.DateInput))
                 .Should()
                 .BeTrue();
        }

        [Fact]
        public void DateInput_TitleDisplaysCorrectly()
        {
            CommonActions.PageTitle()
               .Should()
               .BeEquivalentTo($"Dateinput-TagHelpers".FormatForComparison());
        }

        [Fact]
        public void DateInputBox_EnterDayIntoTheDateInputBox()
        {
            var date = DateTime.Today;
            CommonActions.SendTextToElement(DateInputObject.Day, date.Day.ToString());

            CommonActions.GetElementValue(DateInputObject.Day).Should().Be(date.Day.ToString());
        }

        [Fact]
        public void DateInputBox_EnterYearIntoTheDateInputBox()
        {
            var date = DateTime.Today;
            CommonActions.SendTextToElement(DateInputObject.Year, date.Year.ToString());

            CommonActions.GetElementValue(DateInputObject.Year).Should().Be(date.Year.ToString());
        }

        [Fact]
        public void DateInputBox_EnterMonthIntoTheDateInputBox()
        {
            var date = DateTime.Today;
            CommonActions.SendTextToElement(DateInputObject.Month, date.Month.ToString());

            CommonActions.GetElementValue(DateInputObject.Month).Should().Be(date.Month.ToString());
        }
    }
}
