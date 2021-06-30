using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Order
{
    public sealed class OrderDescriptionModel : OrderingBaseModel
    {
        public OrderDescriptionModel()
        {
        }

        public OrderDescriptionModel(string odsCode, EntityFramework.Ordering.Models.Order order)
        {
            BackLinkText = "Go back";
            Title = "Order description";
            OdsCode = odsCode;
            Description = order?.Description;
        }

        [Required(ErrorMessage = "Enter a description")]
        [StringLength(100)]
        public string Description { get; set; }
    }
}
