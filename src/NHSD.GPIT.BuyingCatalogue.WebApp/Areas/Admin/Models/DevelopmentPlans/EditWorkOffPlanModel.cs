using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.DevelopmentPlans
{
    public sealed class EditWorkOffPlanModel : NavBaseModel
    {
        public EditWorkOffPlanModel()
        {
        }

        public EditWorkOffPlanModel(CatalogueItem catalogueItem, WorkOffPlan workOffPlan)
        {
            SolutionName = catalogueItem.Name;
            SolutionId = catalogueItem.Id;
            WorkOffPlanId = workOffPlan.Id;
        }

        public EditWorkOffPlanModel(CatalogueItem catalogueItem, IList<Standard> standards)
        {
            SolutionName = catalogueItem.Name;
            SolutionId = catalogueItem.Id;
            Standards = standards;
        }

        public EditWorkOffPlanModel(CatalogueItem catalogueItem, IList<Standard> standards, WorkOffPlan workOffPlan)
            : this(catalogueItem, standards)
        {
            WorkOffPlanId = workOffPlan.Id;
            Details = workOffPlan.Details;
            SelectedStandard = workOffPlan.StandardId;
            Day = workOffPlan.CompletionDate.Day.ToString("00");
            Month = workOffPlan.CompletionDate.Month.ToString("00");
            Year = workOffPlan.CompletionDate.Year.ToString("0000");
        }

        public int? WorkOffPlanId { get; init; }

        public string SolutionName { get; init; }

        public CatalogueItemId SolutionId { get; init; }

        public string SelectedStandard { get; init; }

        public IList<Standard> Standards { get; set; }

        [StringLength(300)]
        public string Details { get; init; }

        public string Day { get; set; }

        public string Month { get; set; }

        public string Year { get; set; }

        public DateTime? CompletionDate =>
            DateTime.TryParseExact($"{Day}/{Month}/{Year}", "d/M/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTime parsedDate)
                ? parsedDate : null;
    }
}
