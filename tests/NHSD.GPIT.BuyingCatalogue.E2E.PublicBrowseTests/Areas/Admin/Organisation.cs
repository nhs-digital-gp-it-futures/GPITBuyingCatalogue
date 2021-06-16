using System;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.RandomData;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin
{
    public sealed class Organisation : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public Organisation(LocalWebApplicationFactory factory) : base(factory, "admin/organisations/b7ee5261-43e7-4589-907b-5eef5e98c085")
        {
            Login();
        }

        [Fact]
        public async Task Organisation_OrganisationDetailsDisplayed()
        {
            using var context = GetBCContext();
            var jsonString = (await context.Organisations.SingleAsync(s => s.OrganisationId == Guid.Parse("b7ee5261-43e7-4589-907b-5eef5e98c085"))).Address;
            var dbAddress = JsonSerializer.Deserialize<Address>(jsonString, JsonOptions());

            var pageAddress = AdminPages.Organisation.GetAddress();

            foreach (var prop in dbAddress.GetType().GetProperties())
            {
                if (prop.GetValue(dbAddress) is not null)
                {
                    var value = prop.GetValue(dbAddress).ToString();
                    pageAddress.Should().Contain(value);
                }
            }
        }

        [Fact]
        public async Task Organisation_OdsCodeDisplayed()
        {
            using var context = GetBCContext();
            var dbOdsCode = (await context.Organisations.SingleAsync(s => s.OrganisationId == Guid.Parse("b7ee5261-43e7-4589-907b-5eef5e98c085"))).OdsCode;

            var pageOdsCode = AdminPages.Organisation.GetOdsCode();

            pageOdsCode.Should().Be(dbOdsCode);
        }

        [Fact]
        public void Organisation_UsersSectionDisplayed()
        {
            AdminPages.Organisation.AddUserButtonDisplayed().Should().BeTrue();
            AdminPages.Organisation.UserTableDisplayed().Should().BeTrue();
        }

        [Fact]
        public void Organisation_RelatedOrganisationsSectionDisplayed()
        {
            AdminPages.Organisation.AddRelatedOrganisationButtonDisplayed().Should().BeTrue();
            AdminPages.Organisation.RelatedOrganisationTableDisplayed().Should().BeTrue();
        }

        [Fact]
        public void Organisation_AddUser()
        {
            AdminPages.Organisation.ClickAddUserButton();

            var user = GenerateUser.Generate();

            AdminPages.AddUser.EnterFirstName(user.FirstName);
            AdminPages.AddUser.EnterLastName(user.LastName);
            AdminPages.AddUser.EnterTelephoneNumber(user.TelephoneNumber);
            AdminPages.AddUser.EnterEmailAddress(user.EmailAddress);

            // Same info for the Save functionality on the add user page
            AdminPages.AddUser.ClickAddUserButton();

            var confirmationMessage = AdminPages.AddUser.GetConfirmationMessage();

            confirmationMessage.Should().BeEquivalentTo($"{user.FirstName} {user.LastName} account added");
        }

        private static JsonSerializerOptions JsonOptions()
        {
            return  new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }
    }
}
