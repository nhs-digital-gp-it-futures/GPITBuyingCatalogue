using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models
{
    public sealed class ItemUnitModel
    {
        [Required(ErrorMessage = "ItemUnitNameRequired")]
        [MaxLength(20, ErrorMessage = "ItemUnitNameTooLong")]
        public string Name { get; set; }

        [Required(ErrorMessage = "ItemUnitDescriptionRequired")]
        [MaxLength(40, ErrorMessage = "ItemUnitDescriptionTooLong")]
        public string Description { get; set; }
    }
}
