﻿using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering.Contracts;

public static class DataProcessingPlanObjects
{
    public static By UseDefaultDataProcessingError => By.Id("use-default-data-processing-error");

    public static By DataProcessingInformationLink => By.LinkText("Data processing information");

    public static string VariationsToDefaultDataProcessingInfo => "No, I've agreed variations to the default data processing information with the supplier";
}
