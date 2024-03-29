﻿using System;
using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierModels
{
    public sealed class EditSupplierDetailsModel : NavBaseModel
    {
        public EditSupplierDetailsModel()
        {
        }

        public EditSupplierDetailsModel(Supplier supplier)
        {
            if (supplier is null)
                throw new ArgumentNullException(nameof(supplier));

            SupplierId = supplier.Id;
            SupplierName = supplier.Name;
            SupplierLegalName = supplier.LegalName;
            AboutSupplier = supplier.Summary;
            SupplierWebsite = supplier.SupplierUrl;
            SupplierDisplayName = supplier.Name;
        }

        public int? SupplierId { get; init; }

        [StringLength(255)]
        public string SupplierName { get; init; }

        [StringLength(255)]
        public string SupplierLegalName { get; init; }

        [StringLength(1100)]
        public string AboutSupplier { get; init; }

        [StringLength(1000)]
        public string SupplierWebsite { get; init; }

        public string SupplierDisplayName { get; }
    }
}
