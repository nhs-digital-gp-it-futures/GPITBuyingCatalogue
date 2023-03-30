using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using Xunit;
using Objects = NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering
{
    public sealed class CallOffPartyInformation
        : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private const string InternalOrgId = "CG-03F";
        private static readonly CallOffId CallOffId = new(90001, 1);

        private static readonly Dictionary<string, string> Parameters =
            new()
            {
                { nameof(InternalOrgId), InternalOrgId },
                { nameof(CallOffId), CallOffId.ToString() },
            };

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
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();

            CommonActions.ElementIsDisplayed(Objects.Ordering.CalloffPartyInformation.FirstNameInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Ordering.CalloffPartyInformation.LastNameInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Ordering.CalloffPartyInformation.EmailAddressInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Ordering.CalloffPartyInformation.PhoneNumberInput).Should().BeTrue();
        }

        [Fact]
        public void OrderingPartyInformation_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Order)).Should().BeTrue();
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

            CommonActions.ElementShowingCorrectErrorMessage("Contact.FirstName", "Enter a first name").Should().BeTrue();
            CommonActions.ElementShowingCorrectErrorMessage("Contact.LastName", "Enter a last name").Should().BeTrue();
            CommonActions.ElementShowingCorrectErrorMessage("Contact.EmailAddress", "Enter an email address").Should().BeTrue();
            CommonActions.ElementShowingCorrectErrorMessage("Contact.TelephoneNumber", "Enter a telephone number").Should().BeTrue();
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

            CommonActions.ElementShowingCorrectErrorMessage("Contact.EmailAddress", "Enter an email address in the correct format, like name@example.com")
                .Should().BeTrue();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task OrderingPartyInformation_InputText_AddsPartyInformation(bool orderWithOrderingPartyContact)
        {
            await using var context = GetEndToEndDbContext();

            var organisationId = context.Organisations.First(p => p.InternalIdentifier == InternalOrgId).Id;

            var order = new Order
            {
                OrderNumber = await context.NextOrderNumber(),
                Revision = 1,
                OrderingPartyId = organisationId,
                Created = DateTime.UtcNow,
                IsDeleted = false,
                Description = "This is an Order Description",
            };

            if (orderWithOrderingPartyContact)
            {
                order.OrderingPartyContact = new Contact
                {
                    FirstName = "Clark",
                    LastName = "Kent",
                    Email = "Clark.Kent@TheDailyPlanet.com",
                    Phone = "123456789",
                };
            }

            context.Orders.Add(order);
            context.SaveChanges();

            var callOffId = order.CallOffId;

            var parameters = new Dictionary<string, string>
            {
                { nameof(InternalOrgId), InternalOrgId },
                { nameof(CallOffId), callOffId.ToString() },
            };

            NavigateToUrl(
                  typeof(OrderingPartyController),
                  nameof(OrderingPartyController.OrderingParty),
                  parameters);

            var firstName = TextGenerators.TextInputAddText(Objects.Ordering.CalloffPartyInformation.FirstNameInput, 100);
            var lastName = TextGenerators.TextInputAddText(Objects.Ordering.CalloffPartyInformation.LastNameInput, 100);
            var email = TextGenerators.EmailInputAddText(Objects.Ordering.CalloffPartyInformation.EmailAddressInput, 256);
            var phone = TextGenerators.TextInputAddText(Objects.Ordering.CalloffPartyInformation.PhoneNumberInput, 35);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Order)).Should().BeTrue();

            await using var updatedContext = GetEndToEndDbContext();

            var updatedOrder = updatedContext.Orders
                .Include(o => o.OrderingPartyContact)
                .First(o => o.OrderNumber == callOffId.OrderNumber && o.Revision == callOffId.Revision);

            updatedOrder.OrderingPartyContact.FirstName.Should().BeEquivalentTo(firstName);
            updatedOrder.OrderingPartyContact.LastName.Should().BeEquivalentTo(lastName);
            updatedOrder.OrderingPartyContact.Email.Should().BeEquivalentTo(email);
            updatedOrder.OrderingPartyContact.Phone.Should().BeEquivalentTo(phone);
        }
    }
}
