using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin
{
    public static class DesktopApplicationObjects
    {
        public static By MemorySizeError => By.Id("SelectedMemorySize-error");

        public static By StorageSpaceError => By.Id("StorageSpace-error");

        public static By ProcessingPowerError => By.Id("ProcessingPower-error");

        public static By OperatingSystemsDescriptionError => By.Id("Description-error");
    }
}
