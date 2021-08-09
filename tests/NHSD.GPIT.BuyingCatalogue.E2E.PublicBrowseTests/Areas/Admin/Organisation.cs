﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using netDumbster.smtp;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.RandomData;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin
{
    public sealed class Organisation : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        private static readonly Guid OrganisationId = Guid.Parse("b7ee5261-43e7-4589-907b-5eef5e98c085");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(OrganisationId), OrganisationId.ToString() },
        };

        private readonly SimpleSmtpServer smtp;

        public Organisation(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(OrganisationsController),
                  nameof(OrganisationsController.Details),
                  Parameters)
        {
            smtp = SimpleSmtpServer.Start(9999);
        }

        [Fact]
        public async Task Organisation_OrganisationDetailsDisplayed()
        {
            await using var context = GetEndToEndDbContext();
            var dbAddress = (await context.Organisations.SingleAsync(s => s.Id == OrganisationId)).Address;

            var pageAddress = AdminPages.Organisation.GetAddress().ToList();

            foreach (var prop in dbAddress.GetType().GetProperties())
            {
                if (prop.GetValue(dbAddress) is null)
                    continue;

                var value = prop.GetValue(dbAddress)?.ToString();
                pageAddress.First().Should().Contain(value);
            }
        }

        [Fact]
        public async Task Organisation_OdsCodeDisplayed()
        {
            await using var context = GetEndToEndDbContext();
            var dbOdsCode = (await context.Organisations.SingleAsync(s => s.Id == OrganisationId)).OdsCode;

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
            var user = await AddUser();

            AdminPages.Organisation.ViewUserDetails(user.Id);

            await using var context = GetEndToEndDbContext();
            var organisationName = (await context.Organisations.SingleAsync(s => s.Id == OrganisationId)).Name;

            AdminPages.UserDetails.GetOrganisationName().Should().BeEquivalentTo(organisationName);
            AdminPages.UserDetails.GetUserName().Should().BeEquivalentTo($"{user.FirstName} {user.LastName}");
            AdminPages.UserDetails.GetContactDetails().Should().BeEquivalentTo(user.PhoneNumber);
            AdminPages.UserDetails.GetEmailAddress().Should().BeEquivalentTo(user.Email);
        }

        [Fact]
        public async Task Organisation_DisableUser()
        {
            var user = await AddUser();

            AdminPages.Organisation.ViewUserDetails(user.Id);

            AdminPages.UserDetails.DisableEnableUser();

            await using var context = GetEndToEndDbContext();
            var dbUser = await context.AspNetUsers.SingleAsync(u => u.Id == user.Id);
            dbUser.Disabled.Should().BeTrue();
        }

        [Fact]
        public async Task Organisation_EnableUser()
        {
            var user = await AddUser(false);

            AdminPages.Organisation.ViewUserDetails(user.Id);

            AdminPages.UserDetails.DisableEnableUser();

            await using var context = GetEndToEndDbContext();
            var dbUser = await context.AspNetUsers.SingleAsync(u => u.Id == user.Id);
            dbUser.Disabled.Should().BeFalse();
        }

        [Fact]
        public async Task Organisation_AddRelatedOrganisation()
        {
            AdminPages.Organisation.ClickAddRelatedOrgButton();

            var relatedOrgId = AdminPages.AddRelatedOrganisation.SelectOrganisation(OrganisationId);

            AdminPages.AddRelatedOrganisation.ClickAddRelatedOrgButton();

            var organisation = AdminPages.Organisation.GetRelatedOrganisation(relatedOrgId);

            await using var context = GetEndToEndDbContext();
            var relatedOrgIds = await context.RelatedOrganisations.Where(s => s.OrganisationId == OrganisationId).ToListAsync();
            relatedOrgIds.Select(s => s.RelatedOrganisationId).Should().Contain(relatedOrgId);

            var relatedOrganisation = await context.Organisations.SingleAsync(s => s.Id == relatedOrgId);

            organisation.OrganisationName.Should().BeEquivalentTo(relatedOrganisation.Name);
            organisation.OdsCode.Should().BeEquivalentTo(relatedOrganisation.OdsCode);
        }

        [Fact]
        public async Task Organisation_RemoveRelatedOrganisation()
        {
            var relatedOrgId = await AddRelatedOrganisation();

            AdminPages.Organisation.RemoveRelatedOrganisation(relatedOrgId);

            await using var context = GetEndToEndDbContext();
            var relationships = await context.RelatedOrganisations.ToListAsync();
            var selectedRelationships = relationships.Where(o => o.OrganisationId == OrganisationId);

            selectedRelationships.Select(s => s.RelatedOrganisationId).Should().NotContain(relatedOrgId);
        }

        public void Dispose()
        {
            smtp.Dispose();
        }

        private async Task<Guid> AddRelatedOrganisation()
        {
            await using var context = GetEndToEndDbContext();
            var organisations = await context.Organisations.Where(o => o.Id != OrganisationId).ToListAsync();

            var relatedOrganisation = new RelatedOrganisation
            {
                OrganisationId = OrganisationId,
                RelatedOrganisationId = organisations[new Random().Next(organisations.Count)].Id,
            };

            context.RelatedOrganisations.Add(relatedOrganisation);
            await context.SaveChangesAsync();

            Driver.Navigate().Refresh();

            return relatedOrganisation.RelatedOrganisationId;
        }

        private async Task<AspNetUser> AddUser(bool isEnabled = true)
        {
            var user = GenerateUser.GenerateAspNetUser(OrganisationId, DefaultPassword, isEnabled);
            await using var context = GetEndToEndDbContext();
            context.Add(user);
            await context.SaveChangesAsync();
            Driver.Navigate().Refresh();

            return user;
        }
    }
}
