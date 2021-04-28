using Bogus;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.RandomData
{
    internal static class ContactDetails
    {
        internal static MarketingContact CreateMarketingContact()
        {
            MarketingContact contact = new Faker<MarketingContact>("en_GB")
                .RuleFor(c => c.FirstName, f => f.Name.FirstName())
                .RuleFor(c => c.LastName, f => f.Name.LastName())
                .RuleFor(c => c.PhoneNumber, f => f.Phone.PhoneNumber())
                .RuleFor(c => c.Email, (f, c) => f.Internet.Email(c.FirstName, c.LastName))
                .RuleFor(c => c.Department, f => f.Name.JobTitle())
                .Generate();

            return contact;
        }
    }
}
