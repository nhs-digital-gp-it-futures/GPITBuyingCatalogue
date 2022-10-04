using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.ManageOrders
{
    public static class ViewOrderObjects
    {
        public static By FrameworkSection => ByExtensions.DataTestId("view-order-framework");

        public static By OrganisationSection => ByExtensions.DataTestId("view-order-organisation");

        public static By DescriptionSection => ByExtensions.DataTestId("view-order-description");

        public static By LastUpdatedBySection => ByExtensions.DataTestId("view-order-lastupdatedby");

        public static By SupplierSection => ByExtensions.DataTestId("view-order-supplier");

        public static By StatusSection => ByExtensions.DataTestId("view-order-status");

        public static By DownloadPdf => By.LinkText("Download order summary PDF");

        public static By FullOrderCsv => By.LinkText("Download full order CSV");

        public static By PatientOnlyCsv => By.LinkText("Download patient only CSV");

        public static By OrdersItemsTable => ByExtensions.DataTestId("order-items-table");

        public static By NoOrderItems => ByExtensions.DataTestId("no-order-items");

        public static By FundingType => ByExtensions.DataTestId("funding-type");
    }
}
