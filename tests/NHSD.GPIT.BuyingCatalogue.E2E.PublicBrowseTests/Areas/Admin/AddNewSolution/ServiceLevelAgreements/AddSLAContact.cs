using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Admin.ServiceLevelAgreements;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution.ServiceLevelAgreements
{
    public sealed class AddSLAContact : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        private const string ChannelErrorNoInput = "Enter a contact channel";
        private const string ContactInformationNoInput = "Enter contact information";
        private const string FromNoInput = "Error: Enter a from time";
        private const string UntilNoInput = "Error: Enter an until time";
        private const string FromInvalidFormat = "Error: Enter From in the correct format";
        private const string UntilInvalidFormat = "Error: Enter Until in the correct format";
        private const string DuplicateContact = "A contact with these details already exists";

        private const int ExistingContactId = 1;

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
                .ElementShowingCorrectErrorMessage(SLAContactObjects.TimeInputError, FromInvalidFormat)
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
                .ElementShowingCorrectErrorMessage(SLAContactObjects.TimeInputError, UntilInvalidFormat)
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
            var contact = await context.SlaContacts.SingleOrDefaultAsync(slac => slac.SolutionId == SolutionId && slac.Id != ExistingContactId);

            contact.Should().NotBeNull();

            contact.Channel.FormatForComparison().Should().Be(channel.FormatForComparison());
            contact.ContactInformation.FormatForComparison().Should().Be(contactInformation.FormatForComparison());
            contact.TimeFrom.ToString("HH:mm").FormatForComparison().Should().Be(timefrom.FormatForComparison());
            contact.TimeUntil.ToString("HH:mm").FormatForComparison().Should().Be(timeUntil.FormatForComparison());
        }

        public void Dispose()
        {
            using var context = GetEndToEndDbContext();

            var contact = context.SlaContacts.SingleOrDefault(slac => slac.Id != ExistingContactId && slac.SolutionId == SolutionId);

            if (contact is not null)
            {
                context.SlaContacts.Remove(contact);
                context.SaveChanges();
            }
        }
    }
}
