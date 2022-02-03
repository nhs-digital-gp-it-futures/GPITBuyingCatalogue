using System;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Objects;
using NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Utils;
using NHSD.GPIT.BuyingCatalogue.UI.Components.WebApp.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Tests
{
    public sealed class TimeInputTest : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public TimeInputTest(LocalWebApplicationFactory factory)
                        : base(factory,
                        typeof(HomeController),
                        nameof(HomeController.TimeInput),
                        null)

        {
        }

        [Fact]
        public void TimeInput_EnterValueInSingleFrom()
        {
            var time = DateTime.Now.ToString("HH:mm");
            CommonActions.SendTextToElement(TimeInputObject.SingleFrom, time);

            CommonActions.GetElementValue(TimeInputObject.SingleFrom).Should().Be(time);
        }

        [Fact]
        public void TimeInput_EnterValueIntoUntilInputBox()
        {
            var time = DateTime.Now.ToString("HH:mm");
            CommonActions.SendTextToElement(TimeInputObject.Until, time);

            CommonActions.GetElementValue(TimeInputObject.Until).Should().Be(time);
        }

        [Fact]
        public void TimeInput_EnterValueIntoFromInputBox()
        {
            var time = DateTime.Now.ToString("HH:mm");
            CommonActions.SendTextToElement(TimeInputObject.From, time);

            CommonActions.GetElementValue(TimeInputObject.From).Should().Be(time);
        }
    }
}
