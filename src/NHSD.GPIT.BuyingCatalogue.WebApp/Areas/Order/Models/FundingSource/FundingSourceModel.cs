﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.FundingSource
{
    public class FundingSourceModel : NavBaseModel
    {
        public FundingSourceModel()
        {
        }

        public FundingSourceModel(Organisation organisation)
        {
            OrganisationName = organisation.Name;
        }

        public string OrganisationName { get; set; }

        public ServiceContracts.Enums.FundingSource? SelectedFundingSource { get; set; }

        public IList<SelectListItem> AvailableFundingSources { get; } = new List<SelectListItem>
        {
            new("Central funding", ServiceContracts.Enums.FundingSource.Central.ToString()),
            new("Local funding", ServiceContracts.Enums.FundingSource.Local.ToString()),
        };
    }
}
