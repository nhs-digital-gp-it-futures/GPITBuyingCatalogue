﻿using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering
{
    public static class AmendOrderObjects
    {
        public static By CancelLink => By.LinkText("Cancel");

        public static By ProcurementSupportLink => By.LinkText("Get procurement support");
    }
}
