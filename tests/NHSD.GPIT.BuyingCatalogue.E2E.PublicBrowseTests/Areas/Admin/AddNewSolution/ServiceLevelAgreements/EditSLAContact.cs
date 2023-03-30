using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.ServiceLevelAgreements;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution.ServiceLevelAgreements
{
    [Collection(nameof(AdminCollection))]
    public sealed class EditSLAContact : AuthorityTestBase, IDisposable
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
            var solution = await context.CatalogueItems.FirstOrDefaultAsync(c => c.Id == SolutionSingleContactPublishedId);

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
        public void EditSLAContact_DuplicateContact_ErrorThrown()
        {
            using var context = GetEndToEndDbContext();
            var serviceLevelAgreement = context.ServiceLevelAgreements.Include(p => p.Contacts).First(p => p.SolutionId == SolutionId);

            var contact = new SlaContact()
            {
                SolutionId = new CatalogueItemId(99998, "002"),
                Channel = "This is a Channel 2",
                ContactInformation = "This is Contact Information 2",
                TimeFrom = DateTime.UtcNow,
                TimeUntil = DateTime.UtcNow.AddHours(5),
            };

            serviceLevelAgreement.Contacts.Add(contact);
            context.SaveChanges();

            var parameters = new Dictionary<string, string>
            {
                { nameof(SolutionId), SolutionId.ToString() },
                { nameof(ContactId), contact.Id.ToString() },
            };

            Driver.Navigate().Refresh();

            CommonActions.ElementAddValue(SLAContactObjects.Channel, contact.Channel);
            CommonActions.ElementAddValue(SLAContactObjects.ContactInformation, contact.ContactInformation);

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
        public async Task EditSLAContact_DuplicateContactWithApplicableDays_ErrorThrown()
        {
            await using var context = GetEndToEndDbContext();
            var existingContact = await context.SlaContacts.FirstAsync(slac => slac.Id == 3);
            existingContact.ApplicableDays = "Test";

            await context.SaveChangesAsync();

            CommonActions.ElementAddValue(SLAContactObjects.Channel, existingContact.Channel);
            CommonActions.ElementAddValue(SLAContactObjects.ContactInformation, existingContact.ContactInformation);
            CommonActions.ElementAddValue(SLAContactObjects.ApplicableDays, existingContact.ApplicableDays);

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

            CommonActions
                .ElementShowingCorrectErrorMessage(SLAContactObjects.ApplicableDaysError, DuplicateContact)
                .Should()
                .BeTrue();
        }

        [Fact]
        public void EditSLAContact_ValidContact_ExpectedResult()
        {
            const string timefrom = "12:30";
            const string timeUntil = "13:30";

            using var context = GetEndToEndDbContext();
            var serviceLevelAgreement = context.ServiceLevelAgreements.Include(p => p.Contacts).First(p => p.SolutionId == SolutionId);

            var contact = new SlaContact()
            {
                SolutionId = new CatalogueItemId(99998, "002"),
                Channel = "This is a Channel 2",
                ContactInformation = "This is Contact Information 2",
                TimeFrom = DateTime.UtcNow,
                TimeUntil = DateTime.UtcNow.AddHours(5),
            };

            serviceLevelAgreement.Contacts.Add(contact);
            context.SaveChanges();

            var parameters = new Dictionary<string, string>
            {
                { nameof(SolutionId), SolutionId.ToString() },
                { nameof(ContactId), contact.Id.ToString() },
            };

            NavigateToUrl(
                  typeof(ServiceLevelAgreementsController),
                  nameof(ServiceLevelAgreementsController.EditContact),
                  parameters);

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

            using var updatedContext = GetEndToEndDbContext();
            var updatedContact = updatedContext.SlaContacts.FirstOrDefault(slac => slac.Id == contact.Id);

            updatedContact.Should().NotBeNull();

            updatedContact.Channel.FormatForComparison().Should().Be(channel.FormatForComparison());
            updatedContact.ContactInformation.FormatForComparison().Should().Be(contactInformation.FormatForComparison());
            updatedContact.TimeFrom.ToString("HH:mm").FormatForComparison().Should().Be(timefrom.FormatForComparison());
            updatedContact.TimeUntil.ToString("HH:mm").FormatForComparison().Should().Be(timeUntil.FormatForComparison());

            updatedContext.Remove(updatedContact);
            updatedContext.SaveChanges();
        }

        [Fact]
        public void EditSLAContact_ValidContactWithApplicableDays_ExpectedResult()
        {
            const string timefrom = "12:30";
            const string timeUntil = "13:30";

            using var context = GetEndToEndDbContext();
            var serviceLevelAgreement = context.ServiceLevelAgreements.Include(p => p.Contacts).First(p => p.SolutionId == SolutionId);

            var contact = new SlaContact()
            {
                SolutionId = new CatalogueItemId(99998, "002"),
                Channel = "This is a Channel 2",
                ContactInformation = "This is Contact Information 2",
                TimeFrom = DateTime.UtcNow,
                TimeUntil = DateTime.UtcNow.AddHours(5),
            };

            serviceLevelAgreement.Contacts.Add(contact);
            context.SaveChanges();

            var parameters = new Dictionary<string, string>
            {
                { nameof(SolutionId), SolutionId.ToString() },
                { nameof(ContactId), contact.Id.ToString() },
            };

            NavigateToUrl(
                  typeof(ServiceLevelAgreementsController),
                  nameof(ServiceLevelAgreementsController.EditContact),
                  parameters);

            var channel = TextGenerators.TextInputAddText(SLAContactObjects.Channel, 300);
            var contactInformation = TextGenerators.TextInputAddText(SLAContactObjects.ContactInformation, 1000);
            var applicableDays = TextGenerators.TextInputAddText(SLAContactObjects.ApplicableDays, 1000);
            CommonActions.ElementAddValue(SLAContactObjects.From, timefrom);
            CommonActions.ElementAddValue(SLAContactObjects.Until, timeUntil);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ServiceLevelAgreementsController),
                nameof(ServiceLevelAgreementsController.EditServiceLevelAgreement))
                .Should()
                .BeTrue();

            using var updatedContext = GetEndToEndDbContext();
            var updatedContact = updatedContext.SlaContacts.FirstOrDefault(slac => slac.Id == contact.Id);

            updatedContact.Should().NotBeNull();
            updatedContact.Channel.FormatForComparison().Should().Be(channel.FormatForComparison());
            updatedContact.ContactInformation.FormatForComparison().Should().Be(contactInformation.FormatForComparison());
            updatedContact.ApplicableDays.FormatForComparison().Should().Be(applicableDays.FormatForComparison());
            updatedContact.TimeFrom.ToString("HH:mm").FormatForComparison().Should().Be(timefrom.FormatForComparison());
            updatedContact.TimeUntil.ToString("HH:mm").FormatForComparison().Should().Be(timeUntil.FormatForComparison());

            updatedContext.Remove(updatedContact);
            updatedContext.SaveChanges();
        }

        public void Dispose()
        {
            using var context = GetEndToEndDbContext();

            var solution = context.CatalogueItems.FirstOrDefault(c => c.Id == SolutionSingleContactPublishedId);

            if (solution is not null)
            {
                solution.PublishedStatus = EntityFramework.Catalogue.Models.PublicationStatus.Published;
                context.SaveChanges();
            }
        }
    }
}
