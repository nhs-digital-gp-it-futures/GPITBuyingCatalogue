using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.IntegrationsModels;

public class ManageIntegrationsModel(IEnumerable<Integration> integrations)
{
    public ICollection<Integration> Integrations { get; set; } = integrations.ToList();
}
