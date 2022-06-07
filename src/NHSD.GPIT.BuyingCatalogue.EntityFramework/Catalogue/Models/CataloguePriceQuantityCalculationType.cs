using System.ComponentModel.DataAnnotations;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

public enum CataloguePriceQuantityCalculationType
{
    [Display(Name = "Per solution or service")]
    PerSolutionOrService = 1,
    [Display(Name = "Per Service Recipient")]
    PerServiceRecipient = 2,
}
