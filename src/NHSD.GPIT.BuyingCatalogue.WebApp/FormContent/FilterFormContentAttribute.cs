using Microsoft.AspNetCore.Mvc;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.FormContent;

public class FilteredFormContentAttribute() : ModelBinderAttribute(typeof(FilteredFormContentBinder));
