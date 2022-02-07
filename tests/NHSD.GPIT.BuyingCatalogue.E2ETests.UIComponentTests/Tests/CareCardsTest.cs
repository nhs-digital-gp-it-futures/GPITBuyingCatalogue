﻿using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Objects;
using NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Utils;
using NHSD.GPIT.BuyingCatalogue.UI.Components.WebApp.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Tests
{
    public sealed class CareCardsTest : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public CareCardsTest(LocalWebApplicationFactory factory)

             : base(factory,
                        typeof(HomeController),
                        nameof(HomeController.CareCard),
                        null)
        {
        }

        [Fact]
        public void CareCards_TitleIsDisplayed()
        {
            CommonActions.IsElementDisplayed(CommonObject.Header1).Should().BeTrue();
        }

        [Fact]
        public void CareCards_NonUrgentAdviceTitleIsDisplayed()
        {
            CommonActions.IsElementDisplayed(CareCardObjects.NonUrgentAdvice).Should().BeTrue();
        }

        [Fact]
        public void CareCards_UrgentTestCareTitleIsDisplayed()
        {
            CommonActions.IsElementDisplayed(CareCardObjects.UrgentTestCare).Should().BeTrue();
        }

        [Fact]
        public void CareCards_ImmediateCareCardTitleIsDisplayed()
        {
            CommonActions.IsElementDisplayed(CareCardObjects.ImmediateCareCardTitle).Should().BeTrue();
        }
    }
}
