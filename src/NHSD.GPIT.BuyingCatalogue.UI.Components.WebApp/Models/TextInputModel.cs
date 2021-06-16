using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.UI.Components.DataAttributes;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.WebApp.Models
{
    public sealed class TextInputModel
    {
        public string TextInputNoLength { get; set; }

        [StringLength(100)]
        public string TextInput100Character { get; set; }

        [Password]
        public string TextInputPassword { get; set; }
    }
}
