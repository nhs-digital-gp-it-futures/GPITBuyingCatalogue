using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

public class Competition
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public int FilterId { get; set; }

    public int OrganisationId { get; set; }

    public DateTime LastUpdated { get; set; }

    public DateTime? Completed { get; set; }

    public int? LastUpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual AspNetUser LastUpdatedByUser { get; set; }

    public virtual Filter Filter { get; set; }

    public virtual Organisation Organisation { get; set; }

    public ICollection<CompetitionSolution> CompetitionSolutions { get; set; }
}
