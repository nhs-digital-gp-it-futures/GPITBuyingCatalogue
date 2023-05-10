using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.FrameworkModels;

public class FrameworksDashboardModel
{
    public FrameworksDashboardModel(
        IList<EntityFramework.Catalogue.Models.Framework> frameworks)
    {
        Frameworks = frameworks;
    }

    public IList<EntityFramework.Catalogue.Models.Framework> Frameworks { get; set; }
}
