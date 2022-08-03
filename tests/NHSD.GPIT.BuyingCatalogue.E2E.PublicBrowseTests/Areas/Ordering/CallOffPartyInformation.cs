using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
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

            CommonActions.ElementIsDisplayed(Objects.Ordering.CalloffPartyInformation.FirstNameInputError).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Ordering.CalloffPartyInformation.LastNameInputError).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Ordering.CalloffPartyInformation.EmailAddressInputError).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Ordering.CalloffPartyInformation.PhoneNumberInputError).Should().BeTrue();
        }

        [Fact]
        public void OrderingPartyInformation_InputText_AddsPartyInformation()
        {
            using var context = GetEndToEndDbContext();
            var organisationid = context.Organisations.First(p => p.InternalIdentifier == InternalOrgId).Id;

            var order = new Order
            {
                OrderingPartyId = organisationid,
                Created = DateTime.UtcNow,
                OrderStatus = OrderStatus.InProgress,
                IsDeleted = false,
                Description = "This is an Order Description",
            };

            context.Orders.Add(order);
            context.SaveChanges();

            var callOffId = new CallOffId(order.Id, 1);
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

            using var updatedContext = GetEndToEndDbContext();
            var updatedOrder = updatedContext.Orders.Include(o => o.OrderingPartyContact)
                .Single(o => o.Id == callOffId.Id);

            updatedOrder.OrderingPartyContact.FirstName.Should().BeEquivalentTo(firstName);
            updatedOrder.OrderingPartyContact.LastName.Should().BeEquivalentTo(lastName);
            updatedOrder.OrderingPartyContact.Email.Should().BeEquivalentTo(email);
            updatedOrder.OrderingPartyContact.Phone.Should().BeEquivalentTo(phone);
        }
    }
}
