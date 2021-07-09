using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.RandomData;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Common
{
    internal sealed class TextGenerators : ActionBase
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
        /// <param name="targetField">the which which to add the random URL.</param>
        /// <param name="numChars">number of characters in random URL.</param>
        /// <returns>random URL string.</returns>
        public string UrlInputAddText(By targetField, int numChars)
        {
            Driver.FindElement(targetField).Clear();
            var url = Strings.RandomUrl(numChars);
            Driver.FindElement(targetField).SendKeys(url);
            return url;
        }
    }
}
