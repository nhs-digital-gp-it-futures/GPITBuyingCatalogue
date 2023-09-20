using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering.Contracts;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.Contracts;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.Contracts.DeliveryDates;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation.Shared;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.Contracts.DeliveryDates.Edit.Base
{
    [Collection(nameof(OrderingCollection))]
    public abstract class EditDates : BuyerTestBase
    {
        private readonly int orderId;
        private readonly CatalogueItemId catalogueItemId;
        private readonly string itemName;
        private readonly string itemType;

        protected EditDates(LocalWebApplicationFactory factory, Dictionary<string, string> parameters)
            : base(factory, typeof(DeliveryDatesController), nameof(DeliveryDatesController.EditDates), parameters)
        {
            orderId = int.Parse(parameters["OrderId"]);
            catalogueItemId = CatalogueItemId.ParseExact(parameters["CatalogueItemId"]);
            itemName = parameters["ItemName"];
            itemType = parameters["ItemType"];
        }

        protected abstract string BackLinkMethod { get; }

        protected abstract bool DisplayEditDatesLink { get; }

        protected abstract string OnwardMethod { get; }

        [Fact]
        public void EditDates_AllSectionsDisplayed()
        {
            CommonActions.PageTitle().Should().BeEquivalentTo($"Planned delivery dates for {itemType} - {itemName}".FormatForComparison());
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(DeliveryDatesObjects.EditDatesEditDeliveryDateLink).Should().Be(DisplayEditDatesLink);
            CommonActions.SaveButtonDisplayed().Should().BeTrue();

            var recipients = GetEndToEndDbContext()
                .OrderItemRecipients
                .Include(x => x.Recipient)
                    .ThenInclude(x => x.OdsOrganisation)
                .Where(x => x.OrderId == orderId
                    && x.CatalogueItemId == catalogueItemId)
                .OrderBy(x => x.Recipient.OdsOrganisation.Name)
                .ToList();

            recipients.Select((x, i) => (x, i)).ForEach(x =>
            {
                (OrderItemRecipient recipient, var index) = x;

                CommonActions.ElementTextEqualTo(
                    DeliveryDatesObjects.EditDatesDayInput(index),
                    $"{recipient.DeliveryDate!.Value.Day:00}");

                CommonActions.ElementTextEqualTo(
                    DeliveryDatesObjects.EditDatesMonthInput(index),
                    $"{recipient.DeliveryDate!.Value.Month:00}");

                CommonActions.ElementTextEqualTo(
                    DeliveryDatesObjects.EditDatesYearInput(index),
                    $"{recipient.DeliveryDate!.Value.Year:0000}");
            });
        }

        [Fact]
        public void EditDates_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(typeof(DeliveryDatesController), BackLinkMethod).Should().BeTrue();
        }

        [Fact]
        public void EditDates_ClickSave_ExpectedResult()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(typeof(DeliveryDatesController), OnwardMethod).Should().BeTrue();
        }

        [Fact]
        public void EditDates_ClickEditDeliveryDateLink_ExpectedResult()
        {
            if (!DisplayEditDatesLink)
            {
                return;
            }

            CommonActions.ClickLinkElement(DeliveryDatesObjects.EditDatesEditDeliveryDateLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DeliveryDatesController),
                nameof(DeliveryDatesController.SelectDate)).Should().BeTrue();
        }

        [Fact]
        public void EditDates_EditDate_DayMissing_ExpectedResult()
        {
            CommonActions.ElementAddValue(DeliveryDatesObjects.EditDatesDayInput(0), string.Empty);
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DeliveryDatesController),
                nameof(DeliveryDatesController.EditDates)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            var description = Driver.FindElement(DeliveryDatesObjects.EditDatesDescription(0)).GetAttribute("value");
            var expected = string.Format(DateInputModelValidator.DayMissingWithDescriptionErrorMessage, description);

            CommonActions.ElementTextEqualTo(
                DeliveryDatesObjects.EditDatesDateError,
                $"Error: {expected}").Should().BeTrue();
        }

        [Fact]
        public void EditDates_EditDate_MonthMissing_ExpectedResult()
        {
            CommonActions.ElementAddValue(DeliveryDatesObjects.EditDatesMonthInput(0), string.Empty);
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DeliveryDatesController),
                nameof(DeliveryDatesController.EditDates)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            var description = Driver.FindElement(DeliveryDatesObjects.EditDatesDescription(0)).GetAttribute("value");
            var expected = string.Format(DateInputModelValidator.MonthMissingWithDescriptionErrorMessage, description);

            CommonActions.ElementTextEqualTo(
                DeliveryDatesObjects.EditDatesDateError,
                $"Error: {expected}").Should().BeTrue();
        }

        [Fact]
        public void EditDates_EditDate_YearMissing_ExpectedResult()
        {
            CommonActions.ElementAddValue(DeliveryDatesObjects.EditDatesYearInput(0), string.Empty);
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DeliveryDatesController),
                nameof(DeliveryDatesController.EditDates)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            var description = Driver.FindElement(DeliveryDatesObjects.EditDatesDescription(0)).GetAttribute("value");
            var expected = string.Format(DateInputModelValidator.YearMissingWithDescriptionErrorMessage, description);

            CommonActions.ElementTextEqualTo(
                DeliveryDatesObjects.EditDatesDateError,
                $"Error: {expected}").Should().BeTrue();
        }

        [Fact]
        public void EditDates_EditDate_YearTooShort_ExpectedResult()
        {
            CommonActions.ElementAddValue(DeliveryDatesObjects.EditDatesYearInput(0), "20");
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DeliveryDatesController),
                nameof(DeliveryDatesController.EditDates)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            var description = Driver.FindElement(DeliveryDatesObjects.EditDatesDescription(0)).GetAttribute("value");
            var expected = string.Format(DateInputModelValidator.YearWrongLengthWithDescriptionErrorMessage, description);

            CommonActions.ElementTextEqualTo(
                DeliveryDatesObjects.EditDatesDateError,
                $"Error: {expected}").Should().BeTrue();
        }

        [Fact]
        public void EditDates_EditDate_DateNotValid_ExpectedResult()
        {
            CommonActions.ElementAddValue(DeliveryDatesObjects.EditDatesDayInput(0), "99");
            CommonActions.ElementAddValue(DeliveryDatesObjects.EditDatesMonthInput(0), "99");
            CommonActions.ElementAddValue(DeliveryDatesObjects.EditDatesYearInput(0), "9999");
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DeliveryDatesController),
                nameof(DeliveryDatesController.EditDates)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            var description = Driver.FindElement(DeliveryDatesObjects.EditDatesDescription(0)).GetAttribute("value");
            var expected = string.Format(DateInputModelValidator.DateInvalidWithDescriptionErrorMessage, description);

            CommonActions.ElementTextEqualTo(
                DeliveryDatesObjects.EditDatesDateError,
                $"Error: {expected}").Should().BeTrue();
        }

        [Fact]
        public void EditDates_EditDate_DateInThePast_ExpectedResult()
        {
            CommonActions.ElementAddValue(DeliveryDatesObjects.EditDatesDayInput(0), "01");
            CommonActions.ElementAddValue(DeliveryDatesObjects.EditDatesMonthInput(0), "01");
            CommonActions.ElementAddValue(DeliveryDatesObjects.EditDatesYearInput(0), "2000");
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DeliveryDatesController),
                nameof(DeliveryDatesController.EditDates)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            var description = Driver.FindElement(DeliveryDatesObjects.EditDatesDescription(0)).GetAttribute("value");
            var expected = string.Format(RecipientDateModelValidator.DeliveryDateInThePastErrorMessage, description);

            CommonActions.ElementTextEqualTo(
                DeliveryDatesObjects.EditDatesDateError,
                $"Error: {expected}").Should().BeTrue();
        }

        [Fact]
        public void EditDates_EditDate_DateBeforeCommencementDate_ExpectedResult()
        {
            var commencementDate = GetEndToEndDbContext().Orders.First(x => x.Id == orderId).CommencementDate!.Value;
            var date = commencementDate.AddDays(-1);

            CommonActions.ElementAddValue(DeliveryDatesObjects.EditDatesDayInput(0), $"{date.Day:00}");
            CommonActions.ElementAddValue(DeliveryDatesObjects.EditDatesMonthInput(0), $"{date.Month:00}");
            CommonActions.ElementAddValue(DeliveryDatesObjects.EditDatesYearInput(0), $"{date.Year:0000}");
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DeliveryDatesController),
                nameof(DeliveryDatesController.EditDates)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            var description = Driver.FindElement(DeliveryDatesObjects.EditDatesDescription(0)).GetAttribute("value");
            var expected = RecipientDateModelValidator.CommencementDateErrorMessage(description, commencementDate);

            CommonActions.ElementTextEqualTo(
                DeliveryDatesObjects.EditDatesDateError,
                $"Error: {expected}").Should().BeTrue();
        }
    }
}
