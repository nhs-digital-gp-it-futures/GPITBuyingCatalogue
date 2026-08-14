using Bogus;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.TestData.Generators;

public static class TestDataGenerator
{
    private static readonly Faker _faker = new Faker("en_GB");

    public static string FirstName() => _faker.Name.FirstName();
    public static string LastName() => _faker.Name.LastName();
    public static string PhoneNumber() => _faker.Phone.PhoneNumber("01### ######");
    public static string Email() => _faker.Internet.Email();
    public static string StreetAddress() => _faker.Address.StreetAddress();
    public static string SecondaryAddress() => _faker.Address.SecondaryAddress();
    public static string City() => _faker.Address.City();
    public static string County() => _faker.Address.County();
    public static string Sentence() => _faker.Lorem.Sentence();
}
