using System.Linq;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CapabilityModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.CapabilityModels;

public class EditCapabilitiesModelHelper
{
    public static EditCapabilitiesModel GetPostableEditCapabilitiesModel(EditCapabilitiesModel model)
    {
        return new()
        {
            BackLink = model.BackLink,
            BackLinkText = model.BackLinkText,
            SolutionName = model.SolutionName,
            Title = model.Title,
            CapabilityCategories = [.. model.CapabilityCategories.Select(cc => new CapabilityCategoryModel
        {
            Id = cc.Id,
            Capabilities = [.. cc.Capabilities.Select(c => new CapabilityModel
            {
                Id = c.Id,
                CapabilityRef = c.CapabilityRef,
                Status = c.Status,
                MustEpics = [.. c.MustEpics.Select(e => new CapabilityEpicModel
                {
                    Id = e.Id,
                    Selected = e.Selected,
                    IsActive = e.IsActive,
                })],
                MayEpics = [.. c.MayEpics.Select(e => new CapabilityEpicModel
                {
                    Id = e.Id,
                    Selected = e.Selected,
                    IsActive = e.IsActive,
                })],
            })],
        })],
        };
    }
}
