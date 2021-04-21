using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using System.ComponentModel.DataAnnotations;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models
{
    public class ContactDetailsModel
    { 
        public ContactDetailsModel()
        {
            Contact1 = new Contact();
            Contact2 = new Contact();
        }

        public ContactDetailsModel(CatalogueItem catalogueItem)
        {
            SolutionId = catalogueItem.CatalogueItemId;
        }

        public string SolutionId { get; set; }

        public Contact Contact1 { get; set; }

        public Contact Contact2 { get; set; }
    }

    public class Contact
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Department { get; set; }

        public string PhoneNumber { get; set; }

        [EmailAddress(ErrorMessage = ErrorMessages.EmailAddressInvalid)]
        public string EmailAddress { get; set; }
    }

    public static class ErrorMessages
    {        
        public const string EmailAddressInvalid = "Enter a valid email address";        
    }
}
