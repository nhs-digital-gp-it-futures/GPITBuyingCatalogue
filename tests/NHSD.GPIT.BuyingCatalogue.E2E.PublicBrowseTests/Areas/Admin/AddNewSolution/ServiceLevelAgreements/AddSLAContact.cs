using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.ServiceLevelAgreements;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution.ServiceLevelAgreements
{
    [Collection(nameof(AdminCollection))]
    public sealed class AddSLAContact : AuthorityTestBase
    {
        private const string ChannelErrorNoInput = "Enter a contact channel";
        private const string ContactInformationNoInput = "Enter contact information";
        private const string FromNoInput = "Error: Enter a from time";
        private const string UntilNoInput = "Error: Enter an until time";
        private const string TimeInvalidFormat = "Error: Enter time in the correct format";
        private const string DuplicateContact = "A contact with these details already exists";

        private static readonly CatalogueItemId SolutionId = new(99998, "001");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
        };

        public AddSLAContact(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(ServiceLevelAgreementsController),
                  nameof(ServiceLevelAgreementsController.AddContact),
                  Parameters)
        {
        }

        [Fact]
        public void AddSLAContact_CorrectlyDisplayed()
        {
            CommonActions.ElementIsDisplayed(CommonSelectors.Header1).Should().BeTrue();
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();

            CommonActions.ElementIsDisplayed(SLAContactObjects.Channel).Should().BeTrue();
            CommonActions.ElementIsDisplayed(SLAContactObjects.ContactInformation).Should().BeTrue();
            CommonActions.ElementIsDisplayed(SLAContactObjects.From).Should().BeTrue();
            CommonActions.ElementIsDisplayed(SLAContactObjects.Until).Should().BeTrue();
            CommonActions.ElementExists(SLAContactObjects.DeleteLink).Should().BeFalse();
        }

        [Fact]
        public void AddSLAContact_ClickGoBack_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ServiceLevelAgreementsController),
                nameof(ServiceLevelAgreementsController.EditServiceLevelAgreement))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void AddSLAContact_NoInput_ErrorThrown()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ServiceLevelAgreementsController),
                nameof(ServiceLevelAgreementsController.AddContact))
                .Should()
                .BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions
                .ElementShowingCorrectErrorMessage(SLAContactObjects.ChannelError, ChannelErrorNoInput)
                .Should()
                .BeTrue();

            CommonActions
                .ElementShowingCorrectErrorMessage(SLAContactObjects.ContactInformationError, ContactInformationNoInput)
                .Should()
                .BeTrue();

            CommonActions
                .ElementShowingCorrectErrorMessage(SLAContactObjects.TimeInputError, FromNoInput)
                .Should()
                .BeTrue();
        }

        [Fact]
        public void AddSLAContact_NoUntilInput_ErrorThrown()
        {
            CommonActions.ElementAddValue(SLAContactObjects.From, "12:30");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ServiceLevelAgreementsController),
                nameof(ServiceLevelAgreementsController.AddContact))
                .Should()
                .BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions
                .ElementShowingCorrectErrorMessage(SLAContactObjects.TimeInputError, UntilNoInput)
                .Should()
                .BeTrue();
        }

        [Fact]
        public void AddSLAContact_FromIncorrectFormat_ErrorThrown()
        {
            CommonActions.ElementAddValue(SLAContactObjects.From, "12");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ServiceLevelAgreementsController),
                nameof(ServiceLevelAgreementsController.AddContact))
                .Should()
                .BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions
                .ElementShowingCorrectErrorMessage(SLAContactObjects.TimeInputError, TimeInvalidFormat)
                .Should()
                .BeTrue();
        }

        [Fact]
        public void AddSLAContact_UntilIncorrectFormat_ErrorThrown()
        {
            CommonActions.ElementAddValue(SLAContactObjects.From, "12:30");
            CommonActions.ElementAddValue(SLAContactObjects.Until, "13");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ServiceLevelAgreementsController),
                nameof(ServiceLevelAgreementsController.AddContact))
                .Should()
                .BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions
                .ElementShowingCorrectErrorMessage(SLAContactObjects.TimeInputError, TimeInvalidFormat)
                .Should()
                .BeTrue();
        }

        [Fact]
        public async Task AddSLAContact_DuplicateContact_ErrorThrown()
        {
            await using var context = GetEndToEndDbContext();
            var existingContact = await context.SlaContacts.FirstAsync(slac => slac.SolutionId == SolutionId);

            CommonActions.ElementAddValue(SLAContactObjects.Channel, existingContact.Channel);
            CommonActions.ElementAddValue(SLAContactObjects.ContactInformation, existingContact.ContactInformation);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ServiceLevelAgreementsController),
                nameof(ServiceLevelAgreementsController.AddContact))
                .Should()
                .BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions
                .ElementShowingCorrectErrorMessage(SLAContactObjects.ChannelError, DuplicateContact)
                .Should()
                .BeTrue();

            CommonActions
                .ElementShowingCorrectErrorMessage(SLAContactObjects.ContactInformationError, DuplicateContact)
                .Should()
                .BeTrue();
        }

        [Fact]
        public async Task AddSLAContact_ValidContact_ExpectedResult()
        {
            const string timefrom = "12:30";
            const string timeUntil = "13:30";

            var channel = TextGenerators.TextInputAddText(SLAContactObjects.Channel, 300);
            var contactInformation = TextGenerators.TextInputAddText(SLAContactObjects.ContactInformation, 1000);
            CommonActions.ElementAddValue(SLAContactObjects.From, timefrom);
            CommonActions.ElementAddValue(SLAContactObjects.Until, timeUntil);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ServiceLevelAgreementsController),
                nameof(ServiceLevelAgreementsController.EditServiceLevelAgreement))
                .Should()
                .BeTrue();

            await using var context = GetEndToEndDbContext();
            var contact = await context.SlaContacts.FirstAsync(x => string.Equals(x.Channel, channel) && string.Equals(x.ContactInformation, contactInformation));

            contact.Should().NotBeNull();

            contact.Channel.FormatForComparison().Should().Be(channel.FormatForComparison());
            contact.ContactInformation.FormatForComparison().Should().Be(contactInformation.FormatForComparison());
            contact.TimeFrom.ToString("HH:mm").FormatForComparison().Should().Be(timefrom.FormatForComparison());
            contact.TimeUntil.ToString("HH:mm").FormatForComparison().Should().Be(timeUntil.FormatForComparison());

            context.Remove(contact);
            await context.SaveChangesAsync();
        }
    }
}
