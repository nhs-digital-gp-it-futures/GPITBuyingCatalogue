using Bogus;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestModels;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.RandomData
{
    internal static class GenerateUser
    {
        internal static User Generate()
        {
            return new Faker<User>("en_GB")
                .RuleFor(u => u.FirstName, f => f.Name.FirstName())
                .RuleFor(u => u.LastName, f => f.Name.LastName())
                .RuleFor(u => u.TelephoneNumber, f => f.Phone.PhoneNumber())
                .RuleFor(u => u.EmailAddress, (f, u) => f.Internet.Email(u.FirstName, u.LastName))
                .Generate();
        }
    }
}
