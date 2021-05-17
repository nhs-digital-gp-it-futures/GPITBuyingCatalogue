using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.RandomData;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Common
{
    internal sealed class TextGenerators : ActionBase
    {
        public TextGenerators(IWebDriver driver) : base(driver)
        {
        }

        /// <summary>
        /// Generates Random Text for TextArea's and TextInputs to the Target Length
        /// </summary>
        /// <param name="targetField"></param>
        /// <param name="numChars"></param>
        /// <returns></returns>
        public string TextInputAddText(By targetField, int numChars)
        {
            Driver.FindElement(targetField).Clear();
            var text = Strings.RandomString(numChars);
            Driver.FindElement(targetField).SendKeys(text);
            return text;
        }

        /// <summary>
        /// Generates a Valid random URL for TextArea's or TextInputs to the Target Length
        /// </summary>
        /// <param name="targetField"></param>
        /// <param name="numChars"></param>
        /// <returns></returns>
        public string UrlInputAddText(By targetField, int numChars)
        {
            Driver.FindElement(targetField).Clear();
            var url = Strings.RandomUrl(numChars);
            Driver.FindElement(targetField).SendKeys(url);
            return url;
        }
    }
}
