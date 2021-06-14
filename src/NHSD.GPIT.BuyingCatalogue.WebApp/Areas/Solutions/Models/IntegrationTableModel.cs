using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public class IntegrationTableModel
    {
        public string Name { get; set; }

        public List<string> Headings { get; set; } = new();

        public List<string[]> Rows { get; set; } = new();
    }
}
