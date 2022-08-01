using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.SupplierDefinedEpics;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.SupplierDefinedEpics
{
    public sealed class AddEpic : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public AddEpic(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(SupplierDefinedEpicsController),
                  nameof(SupplierDefinedEpicsController.AddEpic),
                  null)
        {
        }

        [Fact]
        public void AddEpic_AllSectionsDisplayed()
        {
            CommonActions.PageTitle().Should().Be("Supplier defined Epic details".FormatForComparison());
            CommonActions.LedeText().Should().Be("Provide the following details about the supplier defined Epic.".FormatForComparison());

            CommonActions.GoBackLinkDisplayed().Should().BeTrue();

            CommonActions.ElementIsDisplayed(AddSupplierDefinedEpicObjects.CapabilityInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(AddSupplierDefinedEpicObjects.NameInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(AddSupplierDefinedEpicObjects.DescriptionInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(AddSupplierDefinedEpicObjects.StatusInput).Should().BeTrue();

            CommonActions.SaveButtonDisplayed().Should().BeTrue();
        }

        [Fact]
        public void AddEpic_ClickGoBack_Redirects()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SupplierDefinedEpicsController),
                nameof(SupplierDefinedEpicsController.Dashboard))
                .Should().BeTrue();
        }

        [Fact]
        public void AddEpic_NoInput_ThrowsErrors()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
               typeof(SupplierDefinedEpicsController),
               nameof(SupplierDefinedEpicsController.AddEpic))
               .Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementIsDisplayed(
                AddSupplierDefinedEpicObjects.CapabilityInputError)
                .Should()
                .BeTrue();

            CommonActions.ElementIsDisplayed(
                AddSupplierDefinedEpicObjects.NameInputError)
                .Should()
                .BeTrue();

            CommonActions.ElementIsDisplayed(
                AddSupplierDefinedEpicObjects.DescriptionInputError)
                .Should()
                .BeTrue();

            CommonActions.ElementIsDisplayed(
                AddSupplierDefinedEpicObjects.StatusInputError)
                .Should()
                .BeTrue();
        }

        [Fact]
        public void AddEpic_Valid_SavesEpic()
        {
            var nameText = TextGenerators.TextInputAddText(AddSupplierDefinedEpicObjects.NameInput, 500);
            var descriptionText = TextGenerators.TextInputAddText(AddSupplierDefinedEpicObjects.DescriptionInput, 1000);
            var selectedCapabilityId = CommonActions.SelectRandomDropDownItem(AddSupplierDefinedEpicObjects.CapabilityInput);
            CommonActions.ClickRadioButtonWithText("Active");

            var expectedEpic = new Epic
            {
                Name = nameText,
                Description = descriptionText,
                IsActive = true,
                CompliancyLevel = CompliancyLevel.May,
                CapabilityId = int.Parse(selectedCapabilityId),
                SupplierDefined = true,
            };

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SupplierDefinedEpicsController),
                nameof(SupplierDefinedEpicsController.Dashboard))
                .Should().BeTrue();

            using var context = GetEndToEndDbContext();
            var epic = context.Epics.First(e => e.Name == nameText && e.Description == descriptionText);
            epic
                .Should()
                .BeEquivalentTo(
                    expectedEpic,
                    opt => opt
                        .Excluding(m => m.Id)
                        .Excluding(m => m.LastUpdated)
                        .Excluding(m => m.LastUpdatedBy));
        }
    }
}
