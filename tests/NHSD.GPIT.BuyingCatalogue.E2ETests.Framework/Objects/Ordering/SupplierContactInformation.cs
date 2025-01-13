using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering;

public static class SupplierContactInformation
{
    public static By FirstNameInput => By.Id("FirstName");

    public static By LastNameInput => By.Id("LastName");

    public static By DepartmentInput => By.Id("Department");

    public static By PhoneNumberInput => By.Id("PhoneNumber");

    public static By EmailAddressInput => By.Id("Email");
}
