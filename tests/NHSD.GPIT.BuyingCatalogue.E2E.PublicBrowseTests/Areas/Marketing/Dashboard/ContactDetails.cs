using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Marketing.Dashboard
{
    public sealed class ContactDetails : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public ContactDetails(LocalWebApplicationFactory factory) : base(factory, "marketing/supplier/solution/99999-99/section/contact-details")
        {
            using var context = GetBCContext();
            var contacts = context.MarketingContacts.Where(s => s.SolutionId == "99999-99");
            context.MarketingContacts.RemoveRange(contacts);
            context.SaveChanges();
        }

        [Fact]
        public async Task ContactDetails_AddFirstContactOnly()
        {
            driver.Navigate().Refresh();

            var contact = Utils.RandomData.ContactDetails.CreateMarketingContact();

            MarketingPages.ContactDetailsActions.AddMarketingContact(contact);

            CommonActions.ClickSave();

            using var context = GetBCContext();
            var solution = await context.Solutions.Include(s => s.MarketingContacts).SingleAsync(s => s.Id == "99999-99");

            solution.MarketingContacts.First().Should().BeEquivalentTo(contact, options => options.Excluding(s => s.Id)
                                                                                                  .Excluding(s => s.SolutionId)
                                                                                                  .Excluding(s => s.LastUpdated)
                                                                                                  .Excluding(s => s.Solution));
        }

        [Fact]
        public async Task ContactDetails_AddBothContacts()
        {
            driver.Navigate().Refresh();

            var firstContact = Utils.RandomData.ContactDetails.CreateMarketingContact();
            var secondContact = Utils.RandomData.ContactDetails.CreateMarketingContact();

            MarketingPages.ContactDetailsActions.AddMarketingContact(firstContact, 1);
            MarketingPages.ContactDetailsActions.AddMarketingContact(secondContact, 2);

            CommonActions.ClickSave();

            using var context = GetBCContext();
            var solution = await context.Solutions.Include(s => s.MarketingContacts).SingleAsync(s => s.Id == "99999-99");

            solution.MarketingContacts.First().Should().BeEquivalentTo(firstContact, options => options.Excluding(s => s.Id)
                                                                                                  .Excluding(s => s.SolutionId)
                                                                                                  .Excluding(s => s.LastUpdated)
                                                                                                  .Excluding(s => s.Solution));

            
            solution.MarketingContacts.Last().Should().BeEquivalentTo(secondContact, options => options.Excluding(s => s.Id)
                                                                                                  .Excluding(s => s.SolutionId)
                                                                                                  .Excluding(s => s.LastUpdated)
                                                                                                  .Excluding(s => s.Solution));
        }

        [Fact]
        public void ContactDetails_SectionMarkedComplete()
        {
            driver.Navigate().Refresh();

            var contact = Utils.RandomData.ContactDetails.CreateMarketingContact();

            MarketingPages.ContactDetailsActions.AddMarketingContact(contact);

            CommonActions.ClickSave();

            MarketingPages.DashboardActions.SectionMarkedComplete("Contact details").Should().BeTrue();
        }

        [Fact]
        public void ContactDetails_SectionMarkedIncomplete()
        {
            driver.Navigate().Refresh();

            CommonActions.ClickGoBackLink();

            MarketingPages.DashboardActions.SectionMarkedComplete("Contact details").Should().BeFalse();
        }
    }
}
