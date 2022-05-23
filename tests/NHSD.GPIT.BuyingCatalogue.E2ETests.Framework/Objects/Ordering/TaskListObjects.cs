using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering
{
    public static class TaskListObjects
    {
        public static By SolutionDetails => By.Id("SolutionDetails");

        public static By AdditionalServiceDetails => By.Id("AdditionalServiceDetails");

        public static By AssociatedServiceDetails => By.Id("AssociatedServiceDetails");

        public static By ChangeSolutionLink => By.LinkText("Change Catalogue Solution");

        public static By ChangeAdditionalServicesLink => By.LinkText("Change Additional Services");

        public static By ChangeAssociatedServicesLink => By.LinkText("Change Associated Services");

        public static By Name(CatalogueItemId id) => By.Id($"Name_{id}");

        public static By ServiceRecipientsLink(CatalogueItemId id) => By.Id($"ServiceRecipients_{id}");

        public static By PriceLink(CatalogueItemId id) => By.Id($"Price_{id}");

        public static By QuantityLink(CatalogueItemId id) => By.Id($"Quantity_{id}");

        public static By ContinueButton => By.LinkText("Continue");
    }
}
