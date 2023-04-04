using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation.Shared;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.Supplier
{
    [Collection(nameof(OrderingCollection))]
    public sealed class NewContact : BuyerTestBase
    {
        private const string InternalOrgId = "CG-03F";
        private static readonly CallOffId CallOffId = new(91002, 1);

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(InternalOrgId), InternalOrgId },
            { nameof(CallOffId), CallOffId.ToString() },
        };

        public NewContact(LocalWebApplicationFactory factory)
            : base(factory, typeof(SupplierController), nameof(SupplierController.NewContact), Parameters)
        {
        }

        [Fact]
        public void NewContact_AllSectionsDisplayed()
        {
            CommonActions.PageTitle().Should().BeEquivalentTo($"Add a contact - Order {CallOffId}".FormatForComparison());
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(NewContactObjects.FirstNameInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(NewContactObjects.LastNameInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(NewContactObjects.DepartmentInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(NewContactObjects.PhoneNumberInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(NewContactObjects.EmailInput).Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
        }

        [Fact]
        public void NewContact_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SupplierController),
                nameof(SupplierController.Supplier)).Should().BeTrue();
        }

        [Fact]
        public void NewContact_NoPersonalDetails_ExpectedResult()
        {
            TextGenerators.TextInputAddText(NewContactObjects.FirstNameInput, 0);
            TextGenerators.TextInputAddText(NewContactObjects.LastNameInput, 0);
            TextGenerators.TextInputAddText(NewContactObjects.DepartmentInput, 0);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SupplierController),
                nameof(SupplierController.NewContact)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                NewContactObjects.FirstNameError,
                ContactModelValidator.PersonalDetailsMissingErrorMessage).Should().BeTrue();
        }

        [Fact]
        public void NewContact_NoFirstName_ExpectedResult()
        {
            TextGenerators.TextInputAddText(NewContactObjects.FirstNameInput, 0);
            TextGenerators.TextInputAddText(NewContactObjects.LastNameInput, 20);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SupplierController),
                nameof(SupplierController.NewContact)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                NewContactObjects.FirstNameError,
                ContactModelValidator.FirstNameMissingErrorMessage).Should().BeTrue();
        }

        [Fact]
        public void NewContact_NoLastName_ExpectedResult()
        {
            TextGenerators.TextInputAddText(NewContactObjects.FirstNameInput, 20);
            TextGenerators.TextInputAddText(NewContactObjects.LastNameInput, 0);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SupplierController),
                nameof(SupplierController.NewContact)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                NewContactObjects.LastNameError,
                ContactModelValidator.LastNameMissingErrorMessage).Should().BeTrue();
        }

        [Fact]
        public void NewContact_NoContactDetails_ExpectedResult()
        {
            TextGenerators.TextInputAddText(NewContactObjects.PhoneNumberInput, 0);
            TextGenerators.TextInputAddText(NewContactObjects.EmailInput, 0);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SupplierController),
                nameof(SupplierController.NewContact)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                NewContactObjects.PhoneNumberError,
                ContactModelValidator.ContactDetailsMissingErrorMessage).Should().BeTrue();
        }

        [Fact]
        public void NewContact_EmailAddressWrongFormat_ExpectedResult()
        {
            TextGenerators.TextInputAddText(NewContactObjects.EmailInput, 20);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SupplierController),
                nameof(SupplierController.NewContact)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                NewContactObjects.EmailError,
                ContactModelValidator.EmailAddressFormatErrorMessage).Should().BeTrue();
        }

        [Fact]
        public void NewContact_AcceptableValues_ExpectedResult()
        {
            using var context = GetEndToEndDbContext();

            var organisationId = context.Organisations.First(o => o.InternalIdentifier == InternalOrgId).Id;

            var order = new Order
            {
                OrderNumber = context.NextOrderNumber().Result,
                Revision = 1,
                OrderingPartyId = organisationId,
                Created = System.DateTime.UtcNow,
                IsDeleted = false,
                Description = "This is an Order Description",
                OrderingPartyContact = new Contact
                {
                    FirstName = "Clark",
                    LastName = "Kent",
                    Email = "Clark.Kent@TheDailyPlanet.com",
                    Phone = "123456789",
                },
                SupplierId = 99998,
            };

            context.Orders.Add(order);
            context.SaveChanges();

            var parameters = new Dictionary<string, string>()
            {
                { nameof(InternalOrgId), InternalOrgId },
                { nameof(CallOffId), $"{order.CallOffId}" },
            };

            NavigateToUrl(
                typeof(SupplierController),
                nameof(SupplierController.NewContact),
                parameters);

            TextGenerators.TextInputAddText(NewContactObjects.FirstNameInput, 20);
            TextGenerators.TextInputAddText(NewContactObjects.LastNameInput, 20);
            TextGenerators.EmailInputAddText(NewContactObjects.EmailInput, 50);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SupplierController),
                nameof(SupplierController.Supplier)).Should().BeTrue();

            context.Orders.Remove(order);
            context.SaveChanges();
        }

        [Fact]
        public void NewContact_AcceptableValues_EditContact_ExpectedResult()
        {
            using var context = GetEndToEndDbContext();

            var organisationId = context.Organisations.First(o => o.InternalIdentifier == InternalOrgId).Id;

            var order = new Order
            {
                OrderNumber = context.NextOrderNumber().Result,
                Revision = 1,
                OrderingPartyId = organisationId,
                Created = System.DateTime.UtcNow,
                IsDeleted = false,
                Description = "This is an Order Description",
                OrderingPartyContact = new Contact
                {
                    FirstName = "Clark",
                    LastName = "Kent",
                    Email = "Clark.Kent@TheDailyPlanet.com",
                    Phone = "123456789",
                },
                SupplierId = 99998,
            };

            context.Orders.Add(order);
            context.SaveChanges();

            var parameters = new Dictionary<string, string>()
            {
                { nameof(InternalOrgId), InternalOrgId },
                { nameof(CallOffId), $"{order.CallOffId}" },
            };

            NavigateToUrl(
                typeof(SupplierController),
                nameof(SupplierController.NewContact),
                parameters);

            var firstName = TextGenerators.TextInputAddText(NewContactObjects.FirstNameInput, 20);
            var lastName = TextGenerators.TextInputAddText(NewContactObjects.LastNameInput, 20);
            var department = TextGenerators.TextInputAddText(NewContactObjects.DepartmentInput, 20);
            var phoneNumber = TextGenerators.TextInputAddText(NewContactObjects.PhoneNumberInput, 20);
            var email = TextGenerators.EmailInputAddText(NewContactObjects.EmailInput, 50);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SupplierController),
                nameof(SupplierController.Supplier)).Should().BeTrue();

            NavigateToUrl(
                typeof(SupplierController),
                nameof(SupplierController.NewContact),
                parameters);

            CommonActions.ElementIsDisplayed(NewContactObjects.FirstNameInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(NewContactObjects.LastNameInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(NewContactObjects.DepartmentInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(NewContactObjects.PhoneNumberInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(NewContactObjects.EmailInput).Should().BeTrue();

            CommonActions.InputValueEqualTo(NewContactObjects.FirstNameInput, firstName).Should().BeTrue();
            CommonActions.InputValueEqualTo(NewContactObjects.LastNameInput, lastName).Should().BeTrue();
            CommonActions.InputValueEqualTo(NewContactObjects.DepartmentInput, department).Should().BeTrue();
            CommonActions.InputValueEqualTo(NewContactObjects.PhoneNumberInput, phoneNumber).Should().BeTrue();
            CommonActions.InputValueEqualTo(NewContactObjects.EmailInput, email).Should().BeTrue();

            context.Orders.Remove(order);
            context.SaveChanges();
        }
    }
}
