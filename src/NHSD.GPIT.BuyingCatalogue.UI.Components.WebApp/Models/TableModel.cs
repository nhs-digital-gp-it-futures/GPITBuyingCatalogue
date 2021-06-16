using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.WebApp.Models
{
    public sealed class TableModel
    {
        public List<TableRowModel> Rows { get; set; }

        public string SelectableString { get; set; }

        public sealed class TableRowModel
        {
            public string FirstColumn { get; set; }

            public string SecondColumn { get; set; }

            public string ThirdColumn { get; set; }

            public string FourthColumn { get; set; }

            public string FifthColumn { get; set; }

            public string SixthColumn { get; set; }

            public string SeventhColumn { get; set; }
        }
    }
}
