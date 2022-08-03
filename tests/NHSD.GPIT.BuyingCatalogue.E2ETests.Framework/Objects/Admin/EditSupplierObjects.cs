using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin
{
    public static class EditSupplierObjects
    {
        public static By MandatorySectionsMissingError => By.Id("edit-supplier-error");
    }
}
