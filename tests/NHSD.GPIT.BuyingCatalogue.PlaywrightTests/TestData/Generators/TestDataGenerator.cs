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

    // Returns a random multiple of 5 between min and max (inclusive), e.g. 30..90 step 5
    public static int MultipleOfFive(int min, int max)
    {
        var steps = (max - min) / 5;
        var chosen = _faker.Random.Int(0, steps);
        return min + (chosen * 5);
    }

    // Splits 100 into `parts` weightings, each a multiple of 5, each at least 5, summing to 100.
    public static int[] WeightingsSummingTo100(int parts)
    {
        const int totalUnits = 20; // 100 / 5

        // Start each part at 1 unit (=5%), then distribute the remaining units randomly
        var units = new int[parts];
        for (var i = 0; i < parts; i++)
            units[i] = 1;

        var remaining = totalUnits - parts;
        for (var i = 0; i < remaining; i++)
            units[_faker.Random.Int(0, parts - 1)]++;

        var weightings = new int[parts];
        for (var i = 0; i < parts; i++)
            weightings[i] = units[i] * 5;

        return weightings;
    }
}
