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
    public static string MilestoneName() => $"Milestone {_faker.Random.AlphaNumeric(6)}";
    public static string PaymentTrigger() => _faker.Lorem.Sentence();
    public static string ScoreOneToFive() => _faker.Random.Int(1, 5).ToString();

    public static int MultipleOfFive(int min, int max)
    {
        var stepCount = (max - min) / 5;
        return min + (_faker.Random.Int(0, stepCount) * 5);
    }

    public static int[] WeightingsSummingTo100(int parts)
    {
        const int totalUnits = 20;

        var units = Enumerable.Repeat(1, parts).ToArray();

        for (var i = 0; i < totalUnits - parts; i++)
            units[_faker.Random.Int(0, parts - 1)]++;

        return units.Select(u => u * 5).ToArray();
    }
}
