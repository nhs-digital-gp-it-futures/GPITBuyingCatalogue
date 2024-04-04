using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.RandomData;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common
{
    public sealed class TextGenerators : ActionBase
    {
        public TextGenerators(IWebDriver driver)
            : base(driver)
        {
        }


        /// <summary>
        /// Generates Random Text for TextArea's and TextInputs to the Target Length.
        /// </summary>
        /// <param name="targetField">the field which to add the random string.</param>
        /// <param name="numChars">number of character in random string.</param>
        /// <returns> random string.</returns>
        public string TextInputAddText(By targetField, int numChars)
        {
            Driver.FindElement(targetField).Clear();
            var text = Strings.RandomString(numChars);
            Driver.FindElement(targetField).SendKeys(text);
            return text;
        }

        public string TextInput(int  numChars) 
        {
            var text = Strings.RandomString(numChars);            
            return text;
        }

        /// <summary>
        /// Generates a Random Price for Price inputs between 0.0001 and maxValue.
        /// </summary>
        /// <param name="targetField">the field which to add the random string.</param>
        /// <param name="maxValue">the maximum value of the Price, Defaults to 1000.</param>
        /// <returns>random string.</returns>
        public string PriceInputAddPrice(By targetField, decimal maxValue = 1000M)
        {
            Driver.FindElement(targetField).Clear();
            var price = Strings.RandomPrice(maxValue);
            Driver.FindElement(targetField).SendKeys(price);
            return price;
        }

        /// <summary>
        /// Generates a Random Price for Price inputs between a minimum and maximum value.
        /// </summary>
        /// <param name="targetField">the field which to add the random string.</param>
        /// <param name="minValue">the minimum value of the Price, Defaults to 0.0001.</param>
        /// <param name="maxValue">the maximum value of the Price, Defaults to 1000.</param>
        /// <returns>random string.</returns>
        public string PriceInputAddPriceBetweenRange(By targetField, decimal minValue = 0.001M, decimal maxValue = 1000M)
        {
            Driver.FindElement(targetField).Clear();
            var price = Strings.RandomPriceBetweenRange(minValue, maxValue);
            Driver.FindElement(targetField).SendKeys(price);
            return price;
        }

        /// <summary>
        /// Generates a Valid random URL for TextArea's or TextInputs to the Target Length.
        /// </summary>
        /// <param name="targetField">the field which to add the random URL.</param>
        /// <param name="numChars">number of characters in random URL.</param>
        /// <returns>random URL string.</returns>
        public string UrlInputAddText(By targetField, int numChars)
        {
            Driver.FindElement(targetField).Clear();
            var url = Strings.RandomUrl(numChars);
            Driver.FindElement(targetField).SendKeys(url);
            return url;
        }

        /// <summary>
        /// Generates a Valid random Email for TextArea's or TextInputs to the Target Length.
        /// </summary>
        /// <param name="targetField">the field which to add the random Email.</param>
        /// <param name="numChars">number of characters in random Email.</param>
        /// <returns>random Email String.</returns>
        public string EmailInputAddText(By targetField, int numChars)
        {
            Driver.FindElement(targetField).Clear();
            var email = Strings.RandomEmail(numChars);
            Driver.FindElement(targetField).SendKeys(email);
            return email;
        }

        /// <summary>
        /// Generates a random date up to 5 days in the future and applies it to date input fields.
        /// </summary>
        /// <param name="targetDayField">the field which to add the day.</param>
        /// <param name="targetMonthField">the field which to add the month.</param>
        /// <param name="targetYearField">the field which to add the year.</param>
        /// <returns>random date DateTime.</returns>
        public DateTime DateInputAddDateSoon(
            By targetDayField,
            By targetMonthField,
            By targetYearField)
        {
            Driver.FindElement(targetDayField).Clear();
            Driver.FindElement(targetMonthField).Clear();
            Driver.FindElement(targetYearField).Clear();

            var date = Strings.RandomDateSoon();

            Driver.FindElement(targetDayField).SendKeys(date.Day.ToString());
            Driver.FindElement(targetMonthField).SendKeys(date.Month.ToString());
            Driver.FindElement(targetYearField).SendKeys(date.Year.ToString());

            return date;
        }

        public int NumberInputAddRandomNumber(By targetField, int lowerRange = 0, int upperRange = int.MaxValue)
        {
            Driver.FindElement(targetField).Clear();
            var random = new Random();
            var number = random.Next(lowerRange, upperRange);

            Driver.FindElement(targetField).SendKeys(number.ToString());
            return number;
        }

        public string FirstNameInputAddText(By targetField, int numChars)
        {
            Driver.FindElement(targetField).Clear();
            var firstName = Strings.RandomFirstName(numChars);
            Driver.FindElement(targetField).SendKeys(firstName.ToString());

            return firstName;
        }

        public string OrganisationInputAddText(By targetField, int numChars)
        {
            Driver.FindElement(targetField).Clear();
            var organisationName = Strings.RandomOrganisationName(numChars);
            Driver.FindElement(targetField).SendKeys(organisationName.ToString());

            return organisationName;
        }

        public string LastNameInputAddText(By targetField, int numChars)
        {
            Driver.FindElement(targetField).Clear();
            var lastName = Strings.RandomLastName(numChars);
            Driver.FindElement(targetField).SendKeys(lastName.ToString());

            return lastName;
        }

        public string PhoneNumberInputAddText(By targetField, int numChars)
        {
            Driver.FindElement(targetField).Clear();
            var phone = Strings.RandomPhoneNumber(numChars);
            Driver.FindElement(targetField).SendKeys(phone.ToString());

            return phone;
        }
    }
}
