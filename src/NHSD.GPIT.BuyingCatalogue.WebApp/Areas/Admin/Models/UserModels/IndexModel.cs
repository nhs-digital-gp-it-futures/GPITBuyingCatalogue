using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.UserModels
{
    public class IndexModel
    {
        public string SearchTerm { get; set; }

        public IEnumerable<AspNetUser> Users { get; set; }

        public PageOptions PageOptions { get; set; }
    }
}
