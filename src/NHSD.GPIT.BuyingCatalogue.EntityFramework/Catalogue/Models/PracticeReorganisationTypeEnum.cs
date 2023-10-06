using System;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    [Flags]
    public enum PracticeReorganisationTypeEnum
    {
        None = 0,
        Merger = 1,
        Split = 2,
    }
}
