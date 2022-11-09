using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.OrganisationModels
{
    public sealed class AddUserModel : NavBaseModel
    {
        public AddUserModel()
        {
        }

        public AddUserModel(Organisation organisation)
        {
            OrganisationName = organisation.Name;
        }

        public string OrganisationName { get; set; }

        [StringLength(100)]
        public string FirstName { get; set; }

        [StringLength(100)]
        public string LastName { get; set; }

        [StringLength(256)]
        public string EmailAddress { get; set; }

        public string SelectedAccountType { get; set; }

        public IList<SelectListItem> AccountTypeOptions => new List<SelectListItem>
        {
            new(OrganisationFunction.Buyer.DisplayName, $"{OrganisationFunction.Buyer.Name}"),
            new(OrganisationFunction.AccountManager.DisplayName, $"{OrganisationFunction.AccountManager.Name}"),
        };

        public string ControllerName { get; set; }
    }
}
