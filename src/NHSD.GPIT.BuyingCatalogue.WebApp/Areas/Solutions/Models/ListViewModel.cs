using System.ComponentModel.DataAnnotations;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public class ListViewModel
    {
        [UIHint("ListCellList")]
        public string[] List { get; set; }

        [UIHint("ListCellText")]
        public string Text { get; set; }
    }
}
