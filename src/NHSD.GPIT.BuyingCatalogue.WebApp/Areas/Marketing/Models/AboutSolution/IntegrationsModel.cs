﻿using System;
using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.WebApp.ViewModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.AboutSolution
{
    public class IntegrationsModel : MarketingBaseModel
    {
        public IntegrationsModel() : base(null)
        {
        }

        public IntegrationsModel(CatalogueItem catalogueItem) : base(catalogueItem)
        {
            if (catalogueItem is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            BackLink = $"/marketing/supplier/solution/{CatalogueItem.CatalogueItemId}";                        
            Link = CatalogueItem.Solution.IntegrationsUrl;
        }

        public override bool? IsComplete => !string.IsNullOrWhiteSpace(Link);

        [StringLength(1000)]
        [Url]
        public string Link { get; set; }

        public PageTitleViewModel PageTitle() =>
            new()
            {
                Advice = "Describe any systems your Catalogue Solution integrates with to exchange data.",
                Title = "Catalogue Solution integrations",
            };
    }
}
