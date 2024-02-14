using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Models
{
    public abstract class YourAccountBaseModel : NavBaseModel
    {
        private IList<SectionModel> sections;

        protected YourAccountBaseModel()
        {
            SetSections();
        }

        public abstract int Index { get; }

        public virtual string Section => sections[Index].Name;

        public virtual IList<SectionModel> GetSections()
        {
            var sectionsToShow = new List<SectionModel>(sections.Where(s => s.Show));

            if (sectionsToShow.FirstOrDefault(s => s.Name.EqualsIgnoreCase(Section)) is { } sectionModel)
                sectionModel.Selected = true;

            return sectionsToShow;
        }

        private void SetSections()
        {
            sections = new List<SectionModel>
            {
                new()
                {
                    Action = nameof(YourAccountController.Index),
                    Controller = typeof(YourAccountController).ControllerName(),
                    Name = "Your account",
                    Show = true,
                },
                new()
                {
                    Action = nameof(AccountController.Logout),
                    Controller = typeof(AccountController).ControllerName(),
                    Name = "Log out",
                    Show = true,
                },
            };
        }
    }
}
