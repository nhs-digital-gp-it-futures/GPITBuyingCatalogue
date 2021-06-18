﻿using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;

using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models
{
    public class CatalogueSolutionsModel : NavBaseModel
    {
        public List<PublicationStatus> PublicationStatuses { get; set; }

        public List<CatalogueItem> CatalogueItems { get; set; }

        public string GetStatusClass(int statusId)
        {
            string colour = "white";

            if (PublicationStatus.Published.Id == statusId)
                colour = "white";
            if (PublicationStatus.Unpublished.Id == statusId)
                colour = "yellow";
            if (PublicationStatus.Published.Id == statusId)
                colour = "green";
            if (PublicationStatus.Withdrawn.Id == statusId)
                colour = "red";

            return $"nhsuk-tag--{colour}";

            // this doesn't work... WHY ?
            // switch (statusId)
            // {
            //    case PublicationStatus.Draft.Id: colour = "white"; break;
            //    case PublicationStatus.Unpublished.Id: colour = "yellow"; break;
            //    case PublicationStatus.Published.Id: colour = "green"; break;
            //    case PublicationStatus.Withdrawn.Id: colour = "red"; break;
            //    default: colour = "white"; break;
            // }
        }
    }
}
