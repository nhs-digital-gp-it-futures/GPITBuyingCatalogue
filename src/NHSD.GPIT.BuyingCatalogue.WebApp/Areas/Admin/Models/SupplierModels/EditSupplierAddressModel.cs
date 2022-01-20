using System;
using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierModels
{
    public sealed class EditSupplierAddressModel : NavBaseModel
    {
        public EditSupplierAddressModel()
        {
        }

        public EditSupplierAddressModel(Supplier supplier)
        {
            if (supplier is null)
                throw new ArgumentNullException(nameof(supplier));

            AddressLine1 = supplier.Address?.Line1;
            AddressLine2 = supplier.Address?.Line2;
            AddressLine3 = supplier.Address?.Line3;
            AddressLine4 = supplier.Address?.Line4;
            AddressLine5 = supplier.Address?.Line5;
            Town = supplier.Address?.Town;
            County = supplier.Address?.County;
            PostCode = supplier.Address?.Postcode;
            Country = supplier.Address?.Country;

            SupplierName = supplier.Name;
        }

        // All in all the below gives us 405 characters max, with the DB column only taking 500 including the JSON notation. should help us not blow the buffers.
        [StringLength(50)] // longest road name in the uk is 39 characters = Queen Margaret’s Road Industrial Estate.
        public string AddressLine1 { get; init; }

        [StringLength(50)]
        public string AddressLine2 { get; init; }

        [StringLength(50)]
        public string AddressLine3 { get; init; }

        [StringLength(50)]
        public string AddressLine4 { get; init; }

        [StringLength(50)]
        public string AddressLine5 { get; init; }

        [StringLength(60)] // longest town name in the uk is 58 characters = Llanfairpwllgwyngyllgogerychwyrndrobwllllantysiliogogogoch.
        public string Town { get; init; }

        [StringLength(25)] // longest county name in the UK is 20 characters - Llanfairpwllgwyngyll.
        public string County { get; init; }

        [StringLength(10)] // postcode is up to 7 characters, so put in 10 just for safety.
        public string PostCode { get; init; }

        [StringLength(60)]// longest country name in the world is 56 characters - The United Kingdom of Great Britain and Northern Ireland.
        public string Country { get; init; }

        public string SupplierName { get; }
    }
}
