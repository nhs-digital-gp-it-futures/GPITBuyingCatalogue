using Bogus;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.TestData.Generators;

public static class TestDataGenerator
{
    private static readonly Faker Faker = new("en_GB");

    public static string FirstName() => Faker.Name.FirstName();
    public static string LastName() => Faker.Name.LastName();
    public static string PhoneNumber() => Faker.Phone.PhoneNumber("07#########");
    public static string Email() => Faker.Internet.Email();
}
