using System;
using System.Linq;
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

            AdminPages.AddUser.ClickAddUserButton();

            var confirmationMessage = AdminPages.AddUser.GetConfirmationMessage();

            confirmationMessage.Should().BeEquivalentTo($"{user.FirstName} {user.LastName} account added");
            smtp.ReceivedEmailCount.Should().Be(1);
        }

        [Fact]
        public async Task Organisation_ViewUserDetails()
        {
            var currentOrgId = Guid.Parse("b7ee5261-43e7-4589-907b-5eef5e98c085");
            var user = await AddUser(currentOrgId);

            AdminPages.Organisation.ViewUserDetails(user.Id);

            using var context = GetBCContext();
            var organisationName = (await context.Organisations.SingleAsync(s => s.OrganisationId == currentOrgId)).Name;

            AdminPages.UserDetails.GetOrganisationName().Should().BeEquivalentTo(organisationName);
            AdminPages.UserDetails.GetUserName().Should().BeEquivalentTo($"{user.FirstName} {user.LastName}");
            AdminPages.UserDetails.GetContactDetails().Should().BeEquivalentTo(user.PhoneNumber);
            AdminPages.UserDetails.GetEmailAddress().Should().BeEquivalentTo(user.Email);
        }

        [Fact]
        public async Task Organisation_DisableUser()
        {
            var currentOrgId = Guid.Parse("b7ee5261-43e7-4589-907b-5eef5e98c085");
            var user = await AddUser(currentOrgId);

            AdminPages.Organisation.ViewUserDetails(user.Id);

            AdminPages.UserDetails.DisableEnableUser();

            using var context = GetBCContext();
            var dbUser = await context.AspNetUsers.SingleAsync(u => u.Id == user.Id);
            dbUser.Disabled.Should().BeTrue();
        }

        [Fact]
        public async Task Organisation_EnableUser()
        {
            var currentOrgId = Guid.Parse("b7ee5261-43e7-4589-907b-5eef5e98c085");
            var user = await AddUser(currentOrgId, false);

            AdminPages.Organisation.ViewUserDetails(user.Id);

            AdminPages.UserDetails.DisableEnableUser();

            using var context = GetBCContext();
            var dbUser = await context.AspNetUsers.SingleAsync(u => u.Id == user.Id);
            dbUser.Disabled.Should().BeFalse();
        }

        [Fact]
        public async Task Organisation_AddRelatedOrganisation()
        {
            var currentOrgId = Guid.Parse("b7ee5261-43e7-4589-907b-5eef5e98c085");
            AdminPages.Organisation.ClickAddRelatedOrgButton();

            var relatedOrgId = AdminPages.AddRelatedOrganisation.SelectOrganisation(currentOrgId.ToString());

            AdminPages.AddRelatedOrganisation.ClickAddRelatedOrgButton();

            var organisation = AdminPages.Organisation.GetRelatedOrganisation(relatedOrgId);

            using var context = GetBCContext();
            var relatedOrgIds = (await context.RelatedOrganisations.ToListAsync()).Where(s => s.OrganisationId == currentOrgId);
            relatedOrgIds.Select(s => s.RelatedOrganisationId).Should().Contain(relatedOrgId);

            var relatedOrganisation = await context.Organisations.SingleAsync(s => s.OrganisationId == relatedOrgId);

            organisation.OrganisationName.Should().BeEquivalentTo(relatedOrganisation.Name);
            organisation.OdsCode.Should().BeEquivalentTo(relatedOrganisation.OdsCode);
        }

        [Fact]
        public async Task Organisation_RemoveRelatedOrganisation()
        {
            var currentOrgId = Guid.Parse("b7ee5261-43e7-4589-907b-5eef5e98c085");
            var relatedOrgId = await AddRelatedOrganisation(currentOrgId);

            AdminPages.Organisation.RemoveRelatedOrganisation(relatedOrgId);

            using var context = GetBCContext();
            var relationships = await context.RelatedOrganisations.ToListAsync();
            var selectedRelationships = relationships.Where(o => o.OrganisationId == currentOrgId);

            selectedRelationships.Select(s => s.RelatedOrganisationId).Should().NotContain(relatedOrgId);
        }

        private static JsonSerializerOptions JsonOptions()
        {
            return  new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        private async Task<Guid> AddRelatedOrganisation(Guid currentOrgId)
        {
            using var context = GetBCContext();
            var organisations = (await context.Organisations.ToListAsync()).Where(o => o.OrganisationId != currentOrgId).ToList();

            var relatedOrganisation = new RelatedOrganisation
            {
                OrganisationId = currentOrgId,
                RelatedOrganisationId = organisations[new Random().Next(organisations.Count())].OrganisationId,
            };

            context.RelatedOrganisations.Add(relatedOrganisation);
            await context.SaveChangesAsync();

            driver.Navigate().Refresh();

            return relatedOrganisation.RelatedOrganisationId;
        }

        private async Task<AspNetUser> AddUser(Guid currentOrgId, bool IsEnabled = true)
        {
            var user = GenerateUser.GenerateAspNetUser(currentOrgId, DefaultPassword, IsEnabled);
            using var context = GetBCContext();
            context.Add(user);
            await context.SaveChangesAsync();
            driver.Navigate().Refresh();

            return user;
        }
    }
}
