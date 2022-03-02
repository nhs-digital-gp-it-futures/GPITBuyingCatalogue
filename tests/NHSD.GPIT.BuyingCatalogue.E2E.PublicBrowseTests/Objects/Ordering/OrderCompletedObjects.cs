﻿using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Ordering
{
    internal static class OrderCompletedObjects
    {
        public static By DownloadPdfButton => By.LinkText("Download order summary PDF");

        public static By ReturnToDashboardButton => By.LinkText("Return to orders dashboard");

        public static By ContactProcurementLink => By.LinkText("Get procurement support");
    }
}
