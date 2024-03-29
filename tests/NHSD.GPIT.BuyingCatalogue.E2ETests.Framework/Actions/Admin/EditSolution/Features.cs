﻿using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.RandomData;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Admin.EditSolution
{
    public sealed class Features : ActionBase
    {
        public Features(IWebDriver driver)
            : base(driver)
        {
        }

        public IEnumerable<string> EnterFeatures(int numFeatures)
        {
            ClearFeatureInputs();

            var features = new List<string>();

            for (int i = 0; i < numFeatures; i++)
            {
                var feature = Strings.RandomFeature();

                if (feature.Length > 100)
                {
                    feature = feature.Remove(100);
                }

                features.Add(feature);

                Driver.FindElement(Objects.Admin.EditSolution.FeaturesObjects.FeatureInput(i + 1)).SendKeys(feature);
            }

            return features;
        }

        private void ClearFeatureInputs()
        {
            for (int i = 0; i < 10; i++)
            {
                Driver.FindElement(Objects.Admin.EditSolution.FeaturesObjects.FeatureInput(i + 1)).Clear();
            }
        }
    }
}
