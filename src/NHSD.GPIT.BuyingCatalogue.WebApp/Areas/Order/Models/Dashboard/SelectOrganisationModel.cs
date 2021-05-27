﻿using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Identity;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Dashboard
{
    public class SelectOrganisationModel : OrderingBaseModel
    {
        public SelectOrganisationModel()
        {
        }

        public SelectOrganisationModel(string currentOdsCode, List<Organisation> organisations)
        {
            BackLinkText = "Go back";
            BackLink = $"/order/organisation/{currentOdsCode}";
            Title = "Which organisation are you looking for?";
            OdsCode = currentOdsCode;
            AvailableOrganisations = organisations;
            SelectedOrganisation = organisations.Single(x => x.OdsCode.EqualsIgnoreCase(currentOdsCode)).OdsCode;
        }

        public List<Organisation> AvailableOrganisations { get; set; }

        public string SelectedOrganisation { get; set; }
    }
}
