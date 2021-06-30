﻿using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.Services.Csv
{
    public sealed class FullOrderCsvModel
    {
        public CallOffId CallOffId { get; set; }

        public string OdsCode { get; set; }

        public string OrganisationName { get; set; }

        public DateTime? CommencementDate { get; set; }

        public string ServiceRecipientId { get; set; }

        public string ServiceRecipientName { get; set; }

        public string ServiceRecipientItemId { get; set; }

        public string SupplierId { get; set; }

        public string SupplierName { get; set; }

        public string ProductId { get; set; }

        public string ProductName { get; set; }

        public string ProductType { get; set; }

        public int QuantityOrdered { get; set; }

        public string UnitOfOrder { get; set; }

        public string UnitTime { get; set; }

        public string EstimationPeriod { get; set; }

        public decimal Price { get; set; }

        public int OrderType { get; set; }

        public string FundingType { get; set; } = "Central";

        public DateTime? M1Planned { get; set; }

        public DateTime? ActualM1Date { get; set; }

        public DateTime? VerficationDate { get; set; }

        public DateTime? CeaseDate { get; set; }
    }
}
