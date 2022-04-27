using System;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.RandomData;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Utils.RandomData;
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
    }
}
