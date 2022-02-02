﻿using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Ordering
{
    public static class OrderSummary
    {
        public static By DownloadPDFCompletedOrder => By.LinkText("Download order summary PDF");

        public static By DownloadPDFIncompleteOrder => By.LinkText("Download PDF");
    }
}
