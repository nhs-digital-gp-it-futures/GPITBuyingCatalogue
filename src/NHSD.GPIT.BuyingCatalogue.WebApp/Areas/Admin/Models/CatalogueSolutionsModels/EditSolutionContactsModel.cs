﻿using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CatalogueSolutionsModels
{
    public sealed class EditSolutionContactsModel : NavBaseModel
    {
        public EditSolutionContactsModel()
        {
        }

        public EditSolutionContactsModel(CatalogueItem catalogueItem)
        {
            SupplierName = catalogueItem.Supplier.Name;
            SolutionName = catalogueItem.Name;
            SupplierSummary = catalogueItem.Supplier.Summary;

            var supplierContactsList = catalogueItem.Supplier.SupplierContacts.Select(sc => CreateAvailableSupplierContact(sc, catalogueItem)).OrderBy(c => c.DisplayName);
            AvailableSupplierContacts = supplierContactsList.ToList();
        }

        public string SupplierName { get; init; }

        public string SolutionName { get; init; }

        public string SupplierSummary { get; init; }

        public IReadOnlyList<AvailableSupplierContact> AvailableSupplierContacts { get; init; }

        private static AvailableSupplierContact CreateAvailableSupplierContact(SupplierContact supplierContact, CatalogueItem catalogueItem)
        {
            var availableSupplierContact = new AvailableSupplierContact
            {
                Id = supplierContact.Id,
                DisplayName = $"{supplierContact.FirstName} {supplierContact.LastName}",
                Selected = catalogueItem.CatalogueItemContacts.Any(c => c.Id == supplierContact.Id),
            };

            if (!string.IsNullOrWhiteSpace(supplierContact.Department))
                availableSupplierContact.DisplayName += $" ({supplierContact.Department})";

            return availableSupplierContact;
        }
    }
}
