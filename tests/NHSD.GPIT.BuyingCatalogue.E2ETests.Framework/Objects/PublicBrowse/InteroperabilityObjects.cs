using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.PublicBrowse
{
    public static class InteroperabilityObjects
    {
        public static By TableBulk => ByExtensions.DataTestId("table-integrations-bulk");

        public static By TableTransactional => ByExtensions.DataTestId("table-integrations-transactional");

        public static By TablePatientFacing => ByExtensions.DataTestId("table-integrations-patient-facing");

        public static By TableHtmlView => ByExtensions.DataTestId("table-gpconnect-html-view");

        public static By TableAppointmentBooking => ByExtensions.DataTestId("table-gpconnect-appointment-booking");

        public static By TableStructuredRecord => ByExtensions.DataTestId("table-gpconnect-structured-record");

        public static By IMIntegrationsHeading => By.XPath("//h3[text()='IM1 integrations']");

        public static By GpConnectHeading => By.XPath("//h3[text()='GP Connect integrations']");
    }
}
