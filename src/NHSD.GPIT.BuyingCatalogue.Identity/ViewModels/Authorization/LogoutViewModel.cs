using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace NHSD.GPIT.BuyingCatalogue.Identity.ViewModels.Authorization;

public class LogoutViewModel
{
    [BindNever]
    public string RequestId { get; set; }
}
