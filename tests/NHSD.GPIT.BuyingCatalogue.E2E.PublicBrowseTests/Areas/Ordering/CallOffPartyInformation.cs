using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering
{
    public sealed class CallOffPartyInformation
        : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private static readonly CallOffId CallOffId = new(90001, 1);

        private static readonly Dictionary<string, string> Parameters = new() { { "OdsCode", "03F" }, { "CallOffId", CallOffId.ToString() } };

        public CallOffPartyInformation(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(OrderingPartyController),
                  nameof(OrderingPartyController.OrderingParty),
                  Parameters)
        {
        }

        [Fact]
        public void OrderingPartyInformation_AllSectionsDisplayed()
        {
            CommonActions.PageTitle()
                .Should()
                .BeEquivalentTo($"Call-off Ordering Party information for {CallOffId}".FormatForComparison());
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();

            CommonActions.ElementIsDisplayed(Objects.Ordering.CalloffPartyInformation.OrganisationName).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Ordering.CalloffPartyInformation.OrganisationOdsCode).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Ordering.CalloffPartyInformation.OrganisationAddress).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Ordering.CalloffPartyInformation.FirstNameInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Ordering.CalloffPartyInformation.LastNameInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Ordering.CalloffPartyInformation.EmailAddressInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Ordering.CalloffPartyInformation.PhoneNumberInput).Should().BeTrue();
        }

        [Fact]
        public void OrderingPartyInformation_NoTextThrowsError()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderingPartyController),
                nameof(OrderingPartyController.OrderingParty)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage("Contact.FirstName", "First Name Required").Should().BeTrue();
            CommonActions.ElementShowingCorrectErrorMessage("Contact.LastName", "Last Name Required").Should().BeTrue();
            CommonActions.ElementShowingCorrectErrorMessage("Contact.EmailAddress", "Email Address Required").Should().BeTrue();
            CommonActions.ElementShowingCorrectErrorMessage("Contact.TelephoneNumber", "Telephone Number Required").Should().BeTrue();
        }

        [Fact]
        public void OrderingPartyInformation_EmailInputIncorrect_ThrowsError()
        {
            TextGenerators.TextInputAddText(Objects.Ordering.CalloffPartyInformation.FirstNameInput, 100);
            TextGenerators.TextInputAddText(Objects.Ordering.CalloffPartyInformation.LastNameInput, 100);
            TextGenerators.TextInputAddText(Objects.Ordering.CalloffPartyInformation.EmailAddressInput, 256);
            TextGenerators.TextInputAddText(Objects.Ordering.CalloffPartyInformation.PhoneNumberInput, 35);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderingPartyController),
                nameof(OrderingPartyController.OrderingParty)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage("Contact.EmailAddress", "The EmailAddress field is not a valid e-mail address.")
                .Should().BeTrue();
        }

        [Fact]
        public async Task OrderingPartyInformation_InputText_AddsPartyInformation()
        {
            var firstName = TextGenerators.TextInputAddText(Objects.Ordering.CalloffPartyInformation.FirstNameInput, 100);
            var lastName = TextGenerators.TextInputAddText(Objects.Ordering.CalloffPartyInformation.LastNameInput, 100);
            var email = TextGenerators.EmailInputAddText(Objects.Ordering.CalloffPartyInformation.EmailAddressInput, 256);
            var phone = TextGenerators.TextInputAddText(Objects.Ordering.CalloffPartyInformation.PhoneNumberInput, 35);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Order)).Should().BeTrue();

            await using var context = GetEndToEndDbContext();
            var order = await context.Orders.Include(o => o.OrderingPartyContact)
                .SingleAsync(o => o.Id == CallOffId.Id);

            order.OrderingPartyContact.FirstName.Should().BeEquivalentTo(firstName);
            order.OrderingPartyContact.LastName.Should().BeEquivalentTo(lastName);
            order.OrderingPartyContact.Email.Should().BeEquivalentTo(email);
            order.OrderingPartyContact.Phone.Should().BeEquivalentTo(phone);
        }
    }
}
