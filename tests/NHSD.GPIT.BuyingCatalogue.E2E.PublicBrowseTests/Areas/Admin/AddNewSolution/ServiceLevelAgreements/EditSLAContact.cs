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
    public sealed class EditSLAContact : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        private const string ChannelErrorNoInput = "Enter a contact channel";
        private const string ContactInformationNoInput = "Enter contact information";
        private const string FromNoInput = "Error: Enter a from time";
        private const string DuplicateContact = "A contact with these details already exists";

        private const int ContactId = 2;
        private const int ContactSinglePublishedId = 1;
        private static readonly CatalogueItemId SolutionId = new(99998, "002");
        private static readonly CatalogueItemId SolutionSingleContactPublishedId = new(99998, "001");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
            { nameof(ContactId), ContactId.ToString() },
        };

        private static readonly Dictionary<string, string> ParametersPublished = new()
        {
            { nameof(SolutionId), SolutionSingleContactPublishedId.ToString() },
            { nameof(ContactId), ContactSinglePublishedId.ToString() },
        };

        public EditSLAContact(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(ServiceLevelAgreementsController),
                  nameof(ServiceLevelAgreementsController.EditContact),
                  Parameters)
        {
        }

        [Fact]
        public void EditSLAContact_CorrectlyDisplayed()
        {
            CommonActions.ElementIsDisplayed(CommonSelectors.Header1).Should().BeTrue();
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();

            CommonActions.ElementIsDisplayed(SLAContactObjects.Channel).Should().BeTrue();
            CommonActions.ElementIsDisplayed(SLAContactObjects.ContactInformation).Should().BeTrue();
            CommonActions.ElementIsDisplayed(SLAContactObjects.From).Should().BeTrue();
            CommonActions.ElementIsDisplayed(SLAContactObjects.Until).Should().BeTrue();
            CommonActions.ElementIsDisplayed(SLAContactObjects.DeleteLink).Should().BeTrue();
        }

        [Fact]
        public void EditSLAContact_ClickGoBack_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ServiceLevelAgreementsController),
                nameof(ServiceLevelAgreementsController.EditServiceLevelAgreement))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void EditSLAContact_ClickDeleteLink_ExpectedResult()
        {
            CommonActions.ClickLinkElement(SLAContactObjects.DeleteLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ServiceLevelAgreementsController),
                nameof(ServiceLevelAgreementsController.DeleteContact))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void EditSLAContact_SingleContactPublished_DeleteLinkHidden()
        {
            NavigateToUrl(
                typeof(ServiceLevelAgreementsController),
                nameof(ServiceLevelAgreementsController.EditContact),
                ParametersPublished);

            CommonActions.ElementExists(SLAContactObjects.DeleteLink).Should().BeFalse();
        }

        [Fact]
        public async Task EditSLAContact_SingleContactUnpublished_DeleteLinkShown()
        {
            await using var context = GetEndToEndDbContext();
            var solution = await context.CatalogueItems.SingleOrDefaultAsync(c => c.Id == SolutionSingleContactPublishedId);

            solution.PublishedStatus = EntityFramework.Catalogue.Models.PublicationStatus.Unpublished;

            await context.SaveChangesAsync();

            NavigateToUrl(
                typeof(ServiceLevelAgreementsController),
                nameof(ServiceLevelAgreementsController.EditContact),
                ParametersPublished);

            CommonActions.ElementIsDisplayed(SLAContactObjects.DeleteLink).Should().BeTrue();
        }

        [Fact]
        public void AddSLAContact_NoInput_ErrorThrown()
        {
            CommonActions.ClearInputElement(SLAContactObjects.Channel);
            CommonActions.ClearInputElement(SLAContactObjects.ContactInformation);
            CommonActions.ClearInputElement(SLAContactObjects.From);
            CommonActions.ClearInputElement(SLAContactObjects.Until);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ServiceLevelAgreementsController),
                nameof(ServiceLevelAgreementsController.EditContact))
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
        public async Task EditSLAContact_DuplicateContact_ErrorThrown()
        {
            await using var context = GetEndToEndDbContext();
            var existingContact = await context.SlaContacts.SingleAsync(slac => slac.Id == 3);

            CommonActions.ElementAddValue(SLAContactObjects.Channel, existingContact.Channel);
            CommonActions.ElementAddValue(SLAContactObjects.ContactInformation, existingContact.ContactInformation);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ServiceLevelAgreementsController),
                nameof(ServiceLevelAgreementsController.EditContact))
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
        public async Task EditSLAContact_ValidContact_ExpectedResult()
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
            var contact = await context.SlaContacts.SingleOrDefaultAsync(slac => slac.Id == ContactId);

            contact.Should().NotBeNull();

            contact.Channel.FormatForComparison().Should().Be(channel.FormatForComparison());
            contact.ContactInformation.FormatForComparison().Should().Be(contactInformation.FormatForComparison());
            contact.TimeFrom.ToString("HH:mm").FormatForComparison().Should().Be(timefrom.FormatForComparison());
            contact.TimeUntil.ToString("HH:mm").FormatForComparison().Should().Be(timeUntil.FormatForComparison());
        }

        public void Dispose()
        {
            using var context = GetEndToEndDbContext();

            var solution = context.CatalogueItems.SingleOrDefault(c => c.Id == SolutionSingleContactPublishedId);

            if (solution is not null)
            {
                solution.PublishedStatus = EntityFramework.Catalogue.Models.PublicationStatus.Published;
                context.SaveChanges();
            }
        }
    }
}
