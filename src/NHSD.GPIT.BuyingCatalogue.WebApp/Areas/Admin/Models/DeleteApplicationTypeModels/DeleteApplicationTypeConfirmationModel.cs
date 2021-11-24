﻿using EnumsNET;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.DeleteApplicationTypeModels
{
    public class DeleteApplicationTypeConfirmationModel : NavBaseModel
    {
        public DeleteApplicationTypeConfirmationModel()
        {
        }

        public DeleteApplicationTypeConfirmationModel(CatalogueItem solution, ClientApplicationType clientApplicationType)
        {
            ApplicationType = clientApplicationType
                .AsString(EnumFormat.DisplayName)
                .ToLowerInvariant();
            SolutionName = solution.Name;
        }

        public string SolutionName { get; }

        public string ApplicationType { get; set; }
    }
}
