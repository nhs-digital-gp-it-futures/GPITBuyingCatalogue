using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

public class Competition
{
    public Competition()
    {
        CompetitionSolutions = new HashSet<CompetitionSolution>();
        Recipients = new HashSet<OdsOrganisation>();
    }

    public int Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public int FilterId { get; set; }

    public int OrganisationId { get; set; }

    public DateTime Created { get; set; }

    public DateTime LastUpdated { get; set; }

    public DateTime? Completed { get; set; }

    public DateTime? ShortlistAccepted { get; set; }

    public bool HasReviewedCriteria { get; set; }

    public int? LastUpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public int? ContractLength { get; set; }

    public bool? IncludesNonPrice { get; set; }

    public string FrameworkId { get; set; }

    public bool IsShortlistAccepted => ShortlistAccepted.HasValue;

    public AspNetUser LastUpdatedByUser { get; set; }

    public Filter Filter { get; set; }

    public Organisation Organisation { get; set; }

    public Weightings Weightings { get; set; }

    public NonPriceElements NonPriceElements { get; set; }

    public Framework Framework { get; set; }

    public ICollection<CompetitionSolution> CompetitionSolutions { get; set; }

    public ICollection<OdsOrganisation> Recipients { get; set; }

    public ICollection<Order> Orders { get; set; }
}
