using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Marketing.Dashboard
{
    public sealed class ContactDetails : TestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        public ContactDetails(LocalWebApplicationFactory factory)
            : base(factory, "marketing/supplier/solution/99999-002/section/contact-details")
        {
            AuthorityLogin();
        }

        [Fact]
        public async Task ContactDetails_AddFirstContactOnly()
        {
            Driver.Navigate().Refresh();

            var contact = Utils.RandomData.ContactDetails.CreateMarketingContact();

            MarketingPages.ContactDetailsActions.AddMarketingContact(contact);

            CommonActions.ClickSave();

            await using var context = GetEndToEndDbContext();
            var solution = await context.Solutions.Include(s => s.MarketingContacts).SingleAsync(s => s.Id == new CatalogueItemId(99999, "002"));

            solution.MarketingContacts.First().Should().BeEquivalentTo(
                contact,
                options => options.Excluding(s => s.Id).Excluding(s => s.SolutionId).Excluding(s => s.LastUpdated));
        }

        [Fact]
        public async Task ContactDetails_AddBothContacts()
        {
            Driver.Navigate().Refresh();

            var firstContact = Utils.RandomData.ContactDetails.CreateMarketingContact();
            var secondContact = Utils.RandomData.ContactDetails.CreateMarketingContact();

            MarketingPages.ContactDetailsActions.AddMarketingContact(firstContact, 1);
            MarketingPages.ContactDetailsActions.AddMarketingContact(secondContact, 2);

            CommonActions.ClickSave();

            await using var context = GetEndToEndDbContext();
            var solution = await context.Solutions.Include(s => s.MarketingContacts).SingleAsync(s => s.Id == new CatalogueItemId(99999, "002"));

            solution.MarketingContacts.First().Should().BeEquivalentTo(
                firstContact,
                options => options.Excluding(s => s.Id).Excluding(s => s.SolutionId).Excluding(s => s.LastUpdated));

            solution.MarketingContacts.Last().Should().BeEquivalentTo(
                secondContact,
                options => options.Excluding(s => s.Id).Excluding(s => s.SolutionId).Excluding(s => s.LastUpdated));
        }

        [Fact]
        public void ContactDetails_SectionMarkedComplete()
        {
            Driver.Navigate().Refresh();

            var contact = Utils.RandomData.ContactDetails.CreateMarketingContact();

            MarketingPages.ContactDetailsActions.AddMarketingContact(contact);

            CommonActions.ClickSave();

            MarketingPages.DashboardActions.SectionMarkedComplete("Contact details").Should().BeTrue();
        }

        [Fact]
        public void ContactDetails_SectionMarkedIncomplete()
        {
            Driver.Navigate().Refresh();

            CommonActions.ClickGoBackLink();

            MarketingPages.DashboardActions.SectionMarkedComplete("Contact details").Should().BeFalse();
        }

        public void Dispose()
        {
            using var context = GetEndToEndDbContext();
            var contacts = context.MarketingContacts.Where(s => s.SolutionId == new CatalogueItemId(99999, "002"));
            context.RemoveRange(contacts);
            context.SaveChanges();
            ClearClientApplication(new CatalogueItemId(99999, "002"));
        }
    }
}
