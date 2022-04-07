using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bogus;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.RandomData
{
    public static class Strings
    {
        public static string RandomString(int numChars)
        {
            var faker = new Faker("en_GB");
            return string.Join(string.Empty, faker.Random.AlphaNumeric(numChars));
        }

        public static string RandomPrice(decimal maxValue)
        {
            var faker = new Faker("en_GB");
            return faker.Commerce.Price(0.0001M, maxValue, 4).ToString();
        }

        public static string RandomPriceBetweenRange(decimal minValue, decimal maxValue)
        {
            var faker = new Faker("en_GB");
            return faker.Commerce.Price(0.0001M, maxValue, 4).ToString();
        }

        public static string RandomUrl(int numChars)
        {
            var faker = new Faker("en_GB");
            var url = faker.Internet.Url();
            return string.Join(string.Empty, url, "/", faker.Random.AlphaNumeric(numChars - url.Length - 1));
        }

        public static string RandomFeature()
        {
            var faker = new Faker("en_GB");
            return faker.Rant.Review();
        }

        public static string RandomEmail(int numChars)
        {
            var faker = new Faker("en_GB");
            var email = faker.Internet.Email();
            var length = numChars - email.Length;
            if(length > 0)
                return string.Join(string.Empty, faker.Random.AlphaNumeric(numChars - email.Length), email);

            return email;
        }

        public static DateTime RandomDateSoon()
        {
            var faker = new Faker("en_GB");
            var date = faker.Date.Soon(5);

            return date.Day <= DateTime.UtcNow.Day
                ? date.AddDays(2)
                : date;
        }
    }
}
